using System;
using System.Collections.Generic;
using Core;
using EventsSystem;
using Services;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : BaseMonoBehaviour
{
    [Header("Aim")]
    [SerializeField] private Transform aimTransform;
    [SerializeField] private float aimAngleRange = 60f;
    [SerializeField] private float aimRadiusDistance;
    [Header("Power Bar")]
    [SerializeField] private GameObject powerBarHolder;
    [SerializeField] private Image powerBar;
    [SerializeField] private float powerThrowMaxSeconds = 2.25f;
    [SerializeField] private float powerBarBasePower = 3.5f;
    [SerializeField] private float powerBarMaxMultiplier = 3.5f;
    [Header("Controllers")]
    [SerializeField] private PlayerAnimationsController playerAnimationsController;
    [SerializeField] private HookController hookController;
    [SerializeField] private FishingRodController fishingRodController;
    [SerializeField] private RopeSimulator2D_V2 ropeSimulator2D;
    [SerializeField] private EyesFollowController eyesFollowController;
    
    private Vector3 clampedAimDir;
    private bool isCast = false;
    private bool isResetting = false;
    private bool _isPaused = false;
    private bool _clicksBlocked = false;
    private Transform hookOriginParent;
    private IEventsSystemService eventsSystemService;
    private float _powerBarTimer;
    
    private float CurrentPowerNormalized => Mathf.PingPong(_powerBarTimer, powerThrowMaxSeconds) / powerThrowMaxSeconds;
    
    protected void Start()
    {
        hookOriginParent = hookController.transform.parent;
        hookController.transform.position = fishingRodController.CurrentActiveHookPivotPosition;
        
        Init();
    }

    protected override HashSet<Type> RequiredServices => new HashSet<Type>()
    {
        typeof(IEventsSystemService),
    };
    
    protected override void OnServicesInitialized()
    {
        eventsSystemService = ServiceLocator.Instance.GetService<IEventsSystemService>();
        eventsSystemService.Subscribe(ProjectConstants.Events.HOOK_RETRACTED, ResetCasting); 
        eventsSystemService.Subscribe(ProjectConstants.Events.GAME_PAUSED, OnGamePause);
        eventsSystemService.Subscribe(ProjectConstants.Events.GAME_RESUMED, OnGameResume);
        eventsSystemService.Subscribe<ClicksBlockedData>(ProjectConstants.Events.CLICKS_BLOCKED, OnClicksBlocked);
    }

    protected void Update()
    {
        if (_isPaused || _clicksBlocked) return;
        
        if (Input.GetKeyDown(KeyCode.R) && hookController.IsInWater)
        {
            ropeSimulator2D.StartReeling();
        }

        if (isCast) return;
        UpdateAimThrowPosition();
        UpdateCastHook();
    }
    
    private void OnGamePause()
    {
        _isPaused = true;
    }

    private void OnGameResume()
    {
        _isPaused = false;
        WaitForFrame( () => _clicksBlocked = false);
    }
    
    private void OnClicksBlocked(ClicksBlockedData obj)
    {
        MgLogger.Log("Clicks Blocked: " + obj.IsBlocked);
        _clicksBlocked = obj.IsBlocked;
    }
    
    private void Init()
    {
        isCast = false;
        _powerBarTimer = 0;
        eyesFollowController.Target = aimTransform;
        ropeSimulator2D.ResetRope();
        powerBarHolder.SetActive(false);
        hookController.ResetHookCast();
    }
    
    public void ResetCasting()
    {
        if (isResetting) return;
        
        isResetting = true;
        Init();
        eventsSystemService?.Publish(ProjectConstants.Events.PLAY_ENDED);

        // TODO: Decide what to do with the view of the hook until its' position is reset
        hookController.SetVisibility(false);
        playerAnimationsController.ResetFishingRod(() =>
        {
            hookController.ResetHookPosition(fishingRodController.CurrentActiveHookPivotPosition);
            WaitForFrame(() =>
            {
                hookController.SetVisibility(true);
                isResetting = false;
            });
        });
    }

    private void UpdateCastHook()
    {
        if (isCast) return;

        ThrowFunctionalityAimAndReleaseToThrow();
    }
    
    private void ThrowFunctionalityAimAndReleaseToThrow()
    {
        if (isResetting) return;
        
        if (Input.GetButton("Fire1"))
        {
            powerBarHolder.SetActive(true);
            _powerBarTimer += Time.deltaTime;
            powerBar.fillAmount = CurrentPowerNormalized;
        }
        else if (Input.GetButtonUp("Fire1") && !_clicksBlocked)
        {
            isCast = true;
            var aimSideSign = Mathf.Sign(aimTransform.position.x - transform.position.x);
            var isAimingLeft = aimSideSign < 0;
            
            hookController.transform.parent = fishingRodController.CurrentActiveHookPivot;
            
            playerAnimationsController.PlayHookCastAnimationSequence(
                fishingRodController.CurrentActiveRodHolder,
                fishingRodController.CurrentActiveRodPivot,
                isAimingLeft,
                CurrentPowerNormalized,
                CastHook);
        }
    }

    private void CastHook()
    {
        var castPower = CurrentPowerNormalized;

        var hookPosBefore = hookController.transform.position;
        WaitForFrame(() =>
        {
            var hookMovementDelta = hookController.transform.position - hookPosBefore;
            
            hookController.transform.parent = hookOriginParent;
            powerBarHolder.SetActive(false);
        
            hookController.CastHook((hookMovementDelta + clampedAimDir) * castPower * powerBarBasePower * powerBarMaxMultiplier, powerBar.fillAmount);
            
            ropeSimulator2D.transform.parent = fishingRodController.CurrentActiveHookPivot;
            ropeSimulator2D.transform.position = Vector3.zero;
            ropeSimulator2D.StartSimulation(fishingRodController.CurrentActiveHookPivotPosition);
            
            eyesFollowController.Target = hookController.transform;
            eventsSystemService?.Publish(ProjectConstants.Events.PLAY_STARTED);
        });
    }

    private void UpdateAimThrowPosition()
    {
        // TODO: Support gamepad inputs later by using the new input system instead
        // TODO: use the camera from the cached camera in the BaseMonoBehaviour
        var screenPos = Input.mousePosition;
        screenPos.z = Mathf.Abs(MainCamera.transform.position.z - transform.position.z);
        var aimPos = MainCamera.ScreenToWorldPoint(screenPos);
        
        var aimDir = (aimPos - transform.position).normalized;
        var rawAngle = Mathf.Atan2(aimDir.x, aimDir.y) * Mathf.Rad2Deg + 90;
        var minAngle = 90 - aimAngleRange / 2;
        var maxAngle = 90 + aimAngleRange / 2;
        var clampedAngle = Mathf.Clamp(rawAngle, minAngle, maxAngle);
        // Debug.Log($"Aim Direction: {aimDir}, Raw Angle: {rawAngle}, Min Angle: {minAngle}, Max Angle: {maxAngle}, Clamped Angle: {clampedAngle}");
    
        clampedAimDir = new Vector3(-Mathf.Cos(clampedAngle * Mathf.Deg2Rad), Mathf.Sin(clampedAngle * Mathf.Deg2Rad), 0f);
    
        if (fishingRodController.TrySetFishingRodStateAccordingToAngle(Mathf.InverseLerp(minAngle, maxAngle, clampedAngle)))
        {
            var rodHookPosition = fishingRodController.CurrentActiveHookPivotPosition;
            hookController.transform.position = rodHookPosition;
            ropeSimulator2D.transform.position = rodHookPosition;
        }
        
        aimTransform.position = transform.position + clampedAimDir * aimRadiusDistance;
        aimTransform.up = clampedAimDir;
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        var center = transform.position;

        var baseAngle = 90 * Mathf.Deg2Rad;

        var leftAngle = (90 - aimAngleRange / 2) * Mathf.Deg2Rad;
        var rightAngle = (90 + aimAngleRange / 2) * Mathf.Deg2Rad;

        var offsetDir = new Vector3(Mathf.Cos(baseAngle), Mathf.Sin(baseAngle), 0f);
        var leftDir = new Vector3(Mathf.Cos(leftAngle), Mathf.Sin(leftAngle), 0f);
        var rightDir = new Vector3(Mathf.Cos(rightAngle), Mathf.Sin(rightAngle), 0f);

        Gizmos.DrawLine(center, center + offsetDir * aimRadiusDistance);

        Gizmos.DrawLine(center, center + leftDir * aimRadiusDistance);

        Gizmos.DrawLine(center, center + rightDir * aimRadiusDistance);

        DrawArc(center, aimRadiusDistance, leftAngle, rightAngle);
    }

    // Helper function to draw an arc between two angles in radians
    private void DrawArc(Vector3 center, float radius, float startAngle, float endAngle)
    {
        const int numSegments = 100; // More segments = smoother arc
        var angleStep = (endAngle - startAngle) / numSegments;

        var prevPoint = center + new Vector3(Mathf.Cos(startAngle), Mathf.Sin(startAngle), 0f) * radius;

        for (var i = 1; i <= numSegments; i++)
        {
            var angle = startAngle + i * angleStep;
            var newPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

    #endregion
}