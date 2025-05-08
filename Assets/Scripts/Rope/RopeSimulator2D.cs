using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeSimulator2D : MonoBehaviour
{
    [SerializeField] private bool startOnAwake = false;
    
    [Header("Rope Settings")]
    public Transform hook;
    public float segmentLength = 0.15f;
    public int constraintIterations = 6;
    public float gravity = -9.81f;
    public float baseDrag = 0.075f;
    public float growThresholdFactor = 0.25f;

    [Header("Dynamic Pin Settings")]
    public int maxPinnedSegments = 25;

    private LineRenderer lineRenderer;
    private List<RopeSegment> segments;
    private int pinIndex = 0;
    private float totalRopeLength = 0f;
    
    private bool simulationPlaying = false;
    private Vector2 headOrigin;
    
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        segments = new List<RopeSegment>();

        if (startOnAwake)
        {
            StartSimulation(transform.position);
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

    public void StartSimulation(Vector2 newStartPos)
    {
        segments.Clear();
        
        headOrigin = newStartPos;
        segments.Add(new RopeSegment(headOrigin));
        segments.Add(new RopeSegment(headOrigin - Vector2.up * segmentLength));

        lineRenderer.positionCount = segments.Count;
        totalRopeLength = segments.Count * segmentLength;

        simulationPlaying = true;
    }
    
    void GrowRopeIfNeeded()
    {
        var verticalDistance = Mathf.Abs(headOrigin.y - hook.position.y);
        var growThreshold = totalRopeLength + segmentLength * growThresholdFactor;

        if (verticalDistance > growThreshold)
        {
            RopeSegment last = segments[^1];
            Vector2 dir = ((Vector2)hook.position - last.posNow).normalized;
            Vector2 newPos = last.posNow + dir * segmentLength;

            RopeSegment newSeg = new RopeSegment(newPos);
            Vector2 velocity = last.posNow - last.posOld;
            newSeg.posOld = newSeg.posNow - velocity * 0.9f;

            segments.Add(newSeg);
            lineRenderer.positionCount = segments.Count;
            totalRopeLength += segmentLength;

            RopeSegment segA = segments[^2];
            RopeSegment segB = segments[^1];

            float dist = (segA.posNow - segB.posNow).magnitude;
            float error = dist - segmentLength;
            Vector2 changeDir = (segA.posNow - segB.posNow).normalized;
            Vector2 change = changeDir * error;

            segA.posNow -= change * 0.5f;
            segB.posNow += change * 0.5f;

            segments[^2] = segA;
            segments[^1] = segB;

            if (segments.Count > maxPinnedSegments && pinIndex < segments.Count - 2)
            {
                pinIndex++;
                headOrigin = segments[pinIndex].posNow;
                segments.RemoveRange(0, pinIndex);
                pinIndex = 0;
                lineRenderer.positionCount = segments.Count;
            }
        }
    }

    void Simulate()
    {
        for (var i = 1; i < segments.Count; i++)
        {
            var seg = segments[i];

            var velocity = seg.posNow - seg.posOld;
            var dragFactor = Mathf.Lerp(1f - baseDrag, 1f, (float)i / (segments.Count - 1));
            velocity *= dragFactor;

            seg.posOld = seg.posNow;
            seg.posNow += velocity;
            seg.posNow += Vector2.up * (gravity * Time.deltaTime * Time.deltaTime);

            segments[i] = seg;
        }
    }

    void ApplyConstraints()
    {
        var pin = segments[pinIndex];
        pin.posNow = headOrigin;
        segments[pinIndex] = pin;

        for (var k = 0; k < constraintIterations; k++)
        {
            var i = 0;
            for (; i < segments.Count - 1; i++)
            {
                RopeSegment segA = segments[i];
                RopeSegment segB = segments[i + 1];

                float dist = (segA.posNow - segB.posNow).magnitude;
                float error = dist - segmentLength;
                Vector2 dir = (segA.posNow - segB.posNow).normalized;
                Vector2 change = dir * error;

                if (i != pinIndex)
                    segA.posNow -= change * 0.5f;

                segB.posNow += change * 0.5f;

                segments[i] = segA;
                segments[i + 1] = segB;
            }

            // Rope end tries to stay locked to the hook position
            RopeSegment tail = segments[^1];
            tail.posNow = Vector2.Lerp(tail.posNow, (Vector2)hook.position, 0.85f); // tight follow
            segments[^1] = tail;


        }
    }

    private void DrawRope()
    {
        for (var i = 0; i < segments.Count; i++)
        {
            lineRenderer.SetPosition(i, segments[i].posNow);
        }
    }
    
    public void ResetRope()
    {
        segments.Clear();
        pinIndex = 0;
        totalRopeLength = 0f;
        lineRenderer.positionCount = segments.Count;

        simulationPlaying = false;
    
        Debug.Log("Rope has been reset.");
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
