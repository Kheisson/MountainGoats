using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeSimulator2D : MonoBehaviour
{
    [Header("Rope Settings")]
    public Transform origin;
    public Transform hook;
    public float segmentLength = 0.15f;
    public float ropeLengthLimit = 20f;
    public int constraintIterations = 6;
    public float gravity = -9.81f;
    public float baseDrag = 0.075f;
    public float growThresholdFactor = 0.25f;

    [Header("Dynamic Pin Settings")]
    public int maxPinnedSegments = 25;
    private int pinIndex = 0;
    private float totalRopeLength = 0f;

    private LineRenderer lineRenderer;
    private List<RopeSegment> segments;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        segments = new List<RopeSegment>();

        Vector2 startPoint = origin.position;
        segments.Add(new RopeSegment(startPoint));
        segments.Add(new RopeSegment(startPoint - Vector2.up * segmentLength));

        lineRenderer.positionCount = segments.Count;
        totalRopeLength = segments.Count * segmentLength;
    }

    void Update()
    {
        GrowRopeIfNeeded();
        Simulate();
        ApplyConstraints();
        DrawRope();
    }

    void GrowRopeIfNeeded()
    {
        float verticalDistance = Mathf.Abs(origin.position.y - hook.position.y);
        float growThreshold = totalRopeLength + segmentLength * growThresholdFactor;

        if (verticalDistance > growThreshold)
        {
            RopeSegment last = segments[segments.Count - 1];
            Vector2 dir = ((Vector2)hook.position - last.posNow).normalized;
            Vector2 newPos = last.posNow + dir * segmentLength;

            RopeSegment newSeg = new RopeSegment(newPos);
            Vector2 velocity = last.posNow - last.posOld;
            newSeg.posOld = newSeg.posNow - velocity * 0.9f;

            segments.Add(newSeg);
            lineRenderer.positionCount = segments.Count;
            totalRopeLength += segmentLength;

            RopeSegment segA = segments[segments.Count - 2];
            RopeSegment segB = segments[segments.Count - 1];

            float dist = (segA.posNow - segB.posNow).magnitude;
            float error = dist - segmentLength;
            Vector2 changeDir = (segA.posNow - segB.posNow).normalized;
            Vector2 change = changeDir * error;

            segA.posNow -= change * 0.5f;
            segB.posNow += change * 0.5f;

            segments[segments.Count - 2] = segA;
            segments[segments.Count - 1] = segB;

            if (segments.Count > maxPinnedSegments && pinIndex < segments.Count - 2)
            {
                pinIndex++;
                origin.position = segments[pinIndex].posNow;
                segments.RemoveRange(0, pinIndex);
                pinIndex = 0;
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
            float dragFactor = Mathf.Lerp(1f - baseDrag, 1f, (float)i / (segments.Count - 1));
            velocity *= dragFactor;

            seg.posOld = seg.posNow;
            seg.posNow += velocity;
            seg.posNow += Vector2.up * gravity * Time.deltaTime * Time.deltaTime;

            segments[i] = seg;
        }
    }

    void ApplyConstraints()
    {
        RopeSegment pin = segments[pinIndex];
        pin.posNow = origin.position;
        segments[pinIndex] = pin;

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

                if (i != pinIndex)
                    segA.posNow -= change * 0.5f;

                segB.posNow += change * 0.5f;

                segments[i] = segA;
                segments[i + 1] = segB;
            }

            // Rope end tries to stay locked to the hook position
            RopeSegment tail = segments[segments.Count - 1];
            tail.posNow = Vector2.Lerp(tail.posNow, (Vector2)hook.position, 0.85f); // tight follow
            segments[segments.Count - 1] = tail;


        }
    }

    void DrawRope()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            lineRenderer.SetPosition(i, segments[i].posNow);
        }
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
