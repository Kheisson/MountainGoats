using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeSimulator2D_V2 : MonoBehaviour
{
    [SerializeField] private float fixedDeltaTime = 1f / 60f;
    [SerializeField] private bool startOnAwake = false;

    [Header("Rope Settings")]
    public Transform head;
    public Transform hook;
    public float segmentLength = 0.15f;
    public int constraintIterations = 7;
    public float gravity = -2.25f;
    public float baseDrag = 0.25f;
    public float growThresholdFactor = 0.05f;
    public int maxPinnedSegments = 200;

    [Header("Water Settings")]
    public Transform waterLevel;
    public float underwaterDragMultiplier = 3f;
    public float gravityScaleBelowWater = 0.3f;

    private float accumulatedTime = 0f;
    
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

        accumulatedTime += Time.deltaTime;
        
        var maxSteps = 5;
        var stepCount = 0;
        while (accumulatedTime >= fixedDeltaTime && stepCount++ < maxSteps)
        {
            Simulate();
            ApplyConstraints();
            accumulatedTime -= fixedDeltaTime;
        }

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
        if (segments.Count >= maxPinnedSegments + 1) // allow space for new segment before trimming
            return;

        RopeSegment segA = segments[^2];
        RopeSegment segB = segments[^1];
        float dist = (segA.posNow - segB.posNow).magnitude;

        float growThreshold = segmentLength + segmentLength * growThresholdFactor;

        if (dist > growThreshold)
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

            // Trim top if over limit
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

        for (int k = 0; k < constraintIterations; k++)
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
