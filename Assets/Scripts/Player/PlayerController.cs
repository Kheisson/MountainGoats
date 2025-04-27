using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform aimTransform;
    [SerializeField] private float maxAimAngle = 60f;
    [SerializeField] private float aimOffset = -90f;
    [SerializeField] private float aimRadiusDistance;
    [SerializeField] private HookController hookController;
    [SerializeField] private GameObject powerBarHolder;
    [SerializeField] private Image powerBar;
    [SerializeField] private float powerThrowMaxSeconds = 2.25f;
    [SerializeField] private float powerBarMaxMultiplier = 3.5f;
    
    private Camera mainCam;
    private Vector3 clampedAimDir;
    private bool isCast = false;
    private Vector3 hookOffset;
    private float currentHoldTime = 0; 
    
    private void Awake()
    {
        powerBarHolder.SetActive(false);
        
        mainCam = Camera.main;
        hookOffset = hookController.transform.position - aimTransform.position;
    }

    protected void Update()
    {
        UpdateAimThrowPosition();
        UpdateCastHook();
    }

    private void UpdateCastHook()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            hookController.CheatReset();
            isCast = false;
            hookController.transform.position = clampedAimDir * (aimRadiusDistance + hookOffset.magnitude);
        }
#endif
        
        if (isCast) return;
        
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
        }
    }

    private void UpdateAimThrowPosition()
    {
        // TODO: Support gamepad inputs later by using the new input system instead
        // TODO: use the camera from the cached camera in the BaseMonoBehaviour
        var screenPos = Input.mousePosition;
        screenPos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
        var aimPos = mainCam.ScreenToWorldPoint(screenPos);

        var aimDir = (aimPos - transform.position).normalized;
        var rawAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        var relativeAngle = rawAngle - aimOffset;
        var clampedRelativeAngle = Mathf.Clamp(relativeAngle, -maxAimAngle, maxAimAngle);
        var finalAngle = clampedRelativeAngle + aimOffset;

        clampedAimDir = new Vector3(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad), 0f);

        aimTransform.position = transform.position + clampedAimDir * aimRadiusDistance;
        aimTransform.up = clampedAimDir;
        if (!isCast)
        {
            hookController.transform.position = clampedAimDir * (aimRadiusDistance + hookOffset.magnitude);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!mainCam) mainCam = Camera.main;

        Gizmos.color = Color.yellow;

        var center = transform.position;
        var offsetDir = new Vector3(Mathf.Cos(aimOffset * Mathf.Deg2Rad), Mathf.Sin(aimOffset * Mathf.Deg2Rad), 0f);

        // Draw center line (offset direction)
        Gizmos.DrawLine(center, center + offsetDir * aimRadiusDistance);

        // Draw left limit line
        var leftDir = new Vector3(Mathf.Cos((aimOffset - maxAimAngle) * Mathf.Deg2Rad), Mathf.Sin((aimOffset - maxAimAngle) * Mathf.Deg2Rad), 0f);
        Gizmos.DrawLine(center, center + leftDir * aimRadiusDistance);

        // Draw right limit line
        var rightDir = new Vector3(Mathf.Cos((aimOffset + maxAimAngle) * Mathf.Deg2Rad), Mathf.Sin((aimOffset + maxAimAngle) * Mathf.Deg2Rad), 0f);
        Gizmos.DrawLine(center, center + rightDir * aimRadiusDistance);

        // Optionally draw an arc between
        DrawArc(center, aimRadiusDistance, aimOffset - maxAimAngle, aimOffset + maxAimAngle);
    }

    // Utility method to draw an arc manually
    private void DrawArc(Vector3 center, float radius, float startAngle, float endAngle, int segments = 30)
    {
        var angleStep = (endAngle - startAngle) / segments;
        var prevPoint = center + new Vector3(Mathf.Cos(startAngle * Mathf.Deg2Rad), Mathf.Sin(startAngle * Mathf.Deg2Rad), 0f) * radius;

        for (var i = 1; i <= segments; i++)
        {
            var currentAngle = startAngle + angleStep * i;
            var nextPoint = center + new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0f) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }

}