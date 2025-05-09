using System;
using System.Collections.Generic;
using Core;
using Services;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(LineRenderer))]
public class RopeSimulator2D_V2 : BaseMonoBehaviour
{
    private IPlayerStatsService _playerStatsService;
    
    [Header("Rope Settings")]
    [SerializeField] private HookController hookController;
    [SerializeField] private FishingRodController fishingRodController;
    [SerializeField] private float segmentLength = 0.15f;
    [SerializeField] private int constraintIterations = 7;
    [SerializeField] private int maxConstraintIterations = 14;
    [SerializeField] private float gravity = -2.25f;
    [SerializeField] private float baseDrag = 0.25f;
    [SerializeField] private float growThresholdFactor = 0.05f;
    [SerializeField] private int maxPinnedSegments = 200;
    [SerializeField] private float reelingSpeed = 4f;
    [SerializeField] private float maxTotalBendAngle = 15f;
    [SerializeField] private bool startOnAwake = false;

    [Header("Water Settings")]
    [SerializeField] private Transform waterLevel;
    [SerializeField] private float underwaterDragMultiplier = 3f;
    [SerializeField] private float gravityScaleBelowWater = 0.3f;

    [Header("HUD")]
    [SerializeField] private TMP_Text ropeLengthText;
    
    protected override HashSet<Type> RequiredServices => new() 
    {
        typeof(IPlayerStatsService)
    };
    
    private Transform hookTransform;
    private LineRenderer lineRenderer;
    private List<RopeSegment> segments;
    private float referenceDeltaTime = 1f / 60f;
    private bool simulationPlaying = false;
    private bool isReeling = false;
    private float maxDistanceReached = 0f;
    private Vector2 renderedRopeTopPosition;
    private float virtualSegmentOffset = 0;
    
    protected override void OnServicesInitialized()
    {
        _playerStatsService = ServiceLocator.Instance.GetService<IPlayerStatsService>();
    }

    protected override void Awake()
    {
        base.Awake();
        
        lineRenderer = GetComponent<LineRenderer>();
        segments = new List<RopeSegment>();
        hookTransform = hookController.transform;

        if (startOnAwake)
        {
            StartSimulation(transform.position);
        }
    }

    void FixedUpdate()
    {
        if (!_isInitialized || !simulationPlaying) return;

        var ropeLength = _playerStatsService.RopeLength;
        
        var verticalDistance = Mathf.Abs(hookTransform.position.y - renderedRopeTopPosition.y);
        if (verticalDistance > maxDistanceReached)
        {
            maxDistanceReached = verticalDistance;
        }

        if (ropeLengthText != null)
        {
            ropeLengthText.text = $"Rope left - {Mathf.Max(0, ropeLength - maxDistanceReached):F2} M";
        }
        
        var reelingDistance = Vector3.Distance(fishingRodController.CurrentActiveHookPivotPosition, hookController.transform.position);
        if (isReeling && reelingDistance <= 0.1f)
        {
            StopReeling();
            return;
        }

        if (!isReeling && verticalDistance >= ropeLength)
        {
            StartReeling();
            return;
        }

        GrowRopeIfNeeded();
        Simulate();
        ApplyConstraints();
        DrawRope();
    }

    [ContextMenu("Start Reeling")]
    public void StartReeling()
    {
        isReeling = true;
        hookController.IsReeling = true;
    }

    [ContextMenu("Stop Reeling")]
    public void StopReeling()
    {
        // TODO: add eventsSystemService.Subscribe(ProjectConstants.Events.HOOK_RETRACTED, ResetCasting); for the rope
        
        isReeling = false;
        ResetRope();
        
        hookController.IsReeling = false;
        hookController.OnRetractComplete();
        hookController.ResetHookCast();
    }

    public void StartSimulation(Vector2 startPos)
    {
        segments.Clear();
        segments.Add(new RopeSegment(startPos));
        segments.Add(new RopeSegment(startPos - Vector2.up * segmentLength));
        lineRenderer.positionCount = segments.Count;
        simulationPlaying = true;
        maxDistanceReached = 0f;
        renderedRopeTopPosition = startPos;
        ropeLengthText.gameObject.SetActive(true);
    }

    void GrowRopeIfNeeded()
    {
        if (isReeling) return;
        if (segments[^1].posNow.y < hookTransform.position.y) return;
        if (CalculateTotalRopeBend() > maxTotalBendAngle) return;
        if (segments.Count >= maxPinnedSegments + 1) return;

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

            segA = segments[^2];
            segB = segments[^1];

            float error = (segA.posNow - segB.posNow).magnitude - segmentLength;
            Vector2 correction = (segA.posNow - segB.posNow).normalized * error * 0.5f;

            segA.posNow -= correction * 0.25f;
            segB.posNow += correction * 0.75f;

            segments[^2] = segA;
            segments[^1] = segB;

            if (segments.Count > maxPinnedSegments)
            {
                renderedRopeTopPosition = segments[1].posNow;
                segments.RemoveAt(0);
                lineRenderer.positionCount = segments.Count;
                virtualSegmentOffset++;
            }
        }
    }

    void Simulate()
    {
        float dt = Mathf.Min(Time.deltaTime, 1f / 30f);

        if (isReeling)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                float t = (float)i / (segments.Count - 1);
                Vector2 target = Vector2.Lerp(hookTransform.position, fishingRodController.CurrentActiveHookPivotPosition, t);
                RopeSegment seg = segments[i];
                seg.posNow = Vector2.MoveTowards(seg.posNow, target, reelingSpeed * dt);
                seg.posOld = seg.posNow;
                segments[i] = seg;
            }

            hookTransform.position = segments[^1].posNow;
            return;
        }

        for (int i = 1; i < segments.Count; i++)
        {
            RopeSegment seg = segments[i];

            Vector2 velocity = seg.posNow - seg.posOld;
            bool isAboveWater = seg.posNow.y > waterLevel.position.y;
            float dragScale = isAboveWater ? baseDrag : baseDrag * underwaterDragMultiplier;
            float gravityScale = isAboveWater ? 1f : gravityScaleBelowWater;

            float dragFactor = Mathf.Lerp(1f - dragScale, 1f, (float)i / (segments.Count - 1));
            velocity *= dragFactor;

            seg.posOld = seg.posNow;
            seg.posNow += velocity;
            seg.posNow += Vector2.up * gravity * gravityScale * dt * dt;

            segments[i] = seg;
        }
    }

    void ApplyConstraints()
    {
        if (isReeling) return;

        Vector2 pinPos = virtualSegmentOffset > 0 
            ? renderedRopeTopPosition 
            : fishingRodController.CurrentActiveHookPivotPosition;

        RopeSegment pin = segments[0];
        pin.posNow = pinPos;
        segments[0] = pin;

        int dynamicIterations = Mathf.Clamp(
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

            RopeSegment tail = segments[^1];
            tail.posNow = Vector2.Lerp(tail.posNow, hookTransform.position, 0.85f);
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
        lineRenderer.positionCount = 0;
        simulationPlaying = false;
        maxDistanceReached = 0f;
        virtualSegmentOffset = 0;
        ropeLengthText.gameObject.SetActive(false);
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

            totalAngle += Vector2.Angle(dirA, dirB);
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
