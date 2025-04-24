using UnityEngine;

public class CameraFollowYOnly : MonoBehaviour
{
    public Transform target;         // The hook or object to follow
    public float smoothSpeed = 5f;   // Higher = faster camera catch-up

    private float fixedX;
    private float fixedZ;

    void Start()
    {
        fixedX = transform.position.x;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        Vector3 currentPosition = transform.position;
        float targetY = Mathf.Lerp(currentPosition.y, target.position.y, smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(fixedX, targetY, fixedZ);
    }
}