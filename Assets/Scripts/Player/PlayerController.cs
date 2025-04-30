using Core;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : BaseMonoBehaviour
{
    [SerializeField] private Transform aimTransform;
    [SerializeField] private float aimAngleRange = 60f;
    [SerializeField] private float aimRadiusDistance;
    [SerializeField] private HookController hookController;
    [SerializeField] private FishingRodController fishingRodController;
    [SerializeField] private RopeSimulator2D ropeSimulator2D;
    [SerializeField] private GameObject powerBarHolder;
    [SerializeField] private Image powerBar;
    [SerializeField] private float powerThrowMaxSeconds = 2.25f;
    [SerializeField] private float powerBarMaxMultiplier = 3.5f;
    
    private Vector3 clampedAimDir;
    private bool isCast = false;
    private float currentHoldTime = 0;
    
    protected override void Awake()
    {
        base.Awake();
        
        powerBarHolder.SetActive(false);
    }

    protected void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            hookController.CheatReset(fishingRodController.CurrentActiveHookPivot.position);
            ropeSimulator2D.ResetRope();
            isCast = false;
        }
#endif

        if (isCast) return;
        UpdateAimThrowPosition();
        UpdateCastHook();
    }

    private void UpdateCastHook()
    {
        if (isCast) return;

        ThrowFunctionalityAimAndReleaseToThrow();
    }

    // Option A - Aim and release to throw
    private void ThrowFunctionalityAimAndReleaseToThrow()
    {
        if (Input.GetButton("Fire1"))
        {
            powerBarHolder.SetActive(true);
            
            currentHoldTime += Time.deltaTime;
            powerBar.fillAmount = currentHoldTime / powerThrowMaxSeconds;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            powerBarHolder.SetActive(false);
            
            currentHoldTime = 0;
            isCast = true;
            hookController.CastHook(clampedAimDir, powerBar.fillAmount);
            ropeSimulator2D.StartSimulation(fishingRodController.CurrentActiveHookPivot.position);
        }
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
            var rodHookPosition = fishingRodController.CurrentActiveHookPivot.position;
            hookController.transform.position = rodHookPosition;
            ropeSimulator2D.transform.position = rodHookPosition;
        }
        
        aimTransform.position = transform.position + clampedAimDir * aimRadiusDistance;
        aimTransform.up = clampedAimDir;
    }
    
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
}