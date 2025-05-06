using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeSimulator2D_V2 : MonoBehaviour
{
    [SerializeField] private float fixedDeltaTime = 1f / 60f;
    [SerializeField] private bool startOnAwake = false;
    [SerializeField] private float maxTotalBendAngle = 15f; // degrees


    [Header("Rope Settings")]
    public Transform head;
    public Transform hook;
    public float segmentLength = 0.15f;
    public int constraintIterations = 7;
    public float gravity = -2.25f;
    public float baseDrag = 0.25f;
    public float growThresholdFactor = 0.05f;
    public int maxPinnedSegments = 200;
    [Range(1, 100)] public int maxConstraintIterations = 14;

    [Header("Water Settings")]
    public Transform waterLevel;
    public float underwaterDragMultiplier = 3f;
    public float gravityScaleBelowWater = 0.3f;

    // Simulation optimizations
    private float accumulatedTime = 0f;
    private float referenceDeltaTime = 1f / 60f;
    private float lastGrowTime = 0f;

    private LineRenderer lineRenderer;
    private List<RopeSegment> segments;

    private bool simulationPlaying = false;
    private float totalRopeLength = 0f;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        segments = new List<RopeSegment>();

        if (startOnAwake)
        {
            StartSimulation(head.position);
        }
    }

    void Update()
    {
        if (!simulationPlaying) return;

        GrowRopeIfNeeded();
        Simulate();
        ApplyConstraints();

        DrawRope();
    }

    public void StartSimulation(Vector2 startPos)
    {
        segments.Clear();
        segments.Add(new RopeSegment(startPos));
        segments.Add(new RopeSegment(startPos - Vector2.up * segmentLength));

        lineRenderer.positionCount = segments.Count;
        totalRopeLength = segments.Count * segmentLength;

        simulationPlaying = true;
    }

    void GrowRopeIfNeeded()
    {
        // Step 1: Don't grow if tail is hanging under the hook
        if (segments[^1].posNow.y < hook.position.y)
            return;

        // Step 2: Only grow if rope can't reach the hook
        float ropeToHookDistance = Vector2.Distance(segments[0].posNow, hook.position);
        float growThreshold = totalRopeLength + segmentLength * growThresholdFactor;
        if (ropeToHookDistance < growThreshold)
            return;

        // Step 3: Prevent growth if rope is visibly curved
        if (CalculateTotalRopeBend() > maxTotalBendAngle)
            return;

        // Step 4: Prevent overgrowth
        if (segments.Count >= maxPinnedSegments + 1)
            return;

        // Step 5: Add new segment smoothly
        RopeSegment segA = segments[^2];
        RopeSegment segB = segments[^1];
        float dist = (segA.posNow - segB.posNow).magnitude;

        if (dist > segmentLength + segmentLength * growThresholdFactor)
        {
            Vector2 dir = (segB.posNow - segA.posNow).normalized;
            Vector2 midPoint = segB.posNow + dir * (segmentLength * 0.9f);

            RopeSegment newSeg = new RopeSegment(midPoint);
            Vector2 velocity = segB.posNow - segB.posOld;
            newSeg.posOld = newSeg.posNow - velocity * 0.8f;

            segments.Add(newSeg);
            lineRenderer.positionCount = segments.Count;
            totalRopeLength += segmentLength;

            segA = segments[^2];
            segB = segments[^1];

            float error = (segA.posNow - segB.posNow).magnitude - segmentLength;
            Vector2 correction = (segA.posNow - segB.posNow).normalized * error * 0.5f;

            segA.posNow -= correction * 0.25f;
            segB.posNow += correction * 0.75f;

            segments[^2] = segA;
            segments[^1] = segB;

            // Step 6: Trim from top if limit reached
            if (segments.Count > maxPinnedSegments)
            {
                head.position = segments[1].posNow;
                segments.RemoveAt(0);
                lineRenderer.positionCount = segments.Count;
            }
        }
    }


    void Simulate()
    {
        for (int i = 1; i < segments.Count; i++)
        {
            RopeSegment seg = segments[i];

            Vector2 velocity = seg.posNow - seg.posOld;

            // Determine water-based drag and gravity scale
            bool isAboveWater = seg.posNow.y > waterLevel.position.y;
            float dragScale = isAboveWater ? baseDrag : baseDrag * underwaterDragMultiplier;
            float gravityScale = isAboveWater ? 1f : gravityScaleBelowWater;

            // Apply drag
            float dragFactor = Mathf.Lerp(1f - dragScale, 1f, (float)i / (segments.Count - 1));
            velocity *= dragFactor;

            seg.posOld = seg.posNow;
            seg.posNow += velocity;

            // Apply scaled gravity
            seg.posNow += Vector2.up * gravity * gravityScale * Time.deltaTime * Time.deltaTime;

            segments[i] = seg;
        }
    }

    void ApplyConstraints()
    {
        RopeSegment pin = segments[0];
        pin.posNow = head.position;
        segments[0] = pin;

        var dynamicIterations = Mathf.Clamp(
            Mathf.RoundToInt(constraintIterations * (referenceDeltaTime / Mathf.Max(Time.deltaTime, 0.001f))),
            constraintIterations,
            maxConstraintIterations
        );

        for (int k = 0; k < dynamicIterations; k++)
        {
            for (int i = 0; i < segments.Count - 1; i++)
            {
                RopeSegment segA = segments[i];
                RopeSegment segB = segments[i + 1];

                float dist = (segA.posNow - segB.posNow).magnitude;
                float error = dist - segmentLength;
                Vector2 dir = (segA.posNow - segB.posNow).normalized;
                Vector2 change = dir * error;

                if (i != 0)
                    segA.posNow -= change * 0.5f;

                segB.posNow += change * 0.5f;

                segments[i] = segA;
                segments[i + 1] = segB;
            }

            // Tight follow of hook
            RopeSegment tail = segments[^1];
            tail.posNow = Vector2.Lerp(tail.posNow, (Vector2)hook.position, 0.85f);
            segments[^1] = tail;
        }
    }

    void DrawRope()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            lineRenderer.SetPosition(i, segments[i].posNow);
        }
    }

    public void ResetRope()
    {
        segments.Clear();
        totalRopeLength = 0f;
        lineRenderer.positionCount = 0;
        simulationPlaying = false;
    }
    
    private float CalculateTotalRopeBend()
    {
        float totalAngle = 0f;

        for (int i = 1; i < segments.Count - 1; i++)
        {
            Vector2 prev = segments[i - 1].posNow;
            Vector2 curr = segments[i].posNow;
            Vector2 next = segments[i + 1].posNow;

            Vector2 dirA = (curr - prev).normalized;
            Vector2 dirB = (next - curr).normalized;

            float angle = Vector2.Angle(dirA, dirB);
            totalAngle += angle;
        }

        return totalAngle;
    }


    private struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }
}
