using UnityEngine;

public class TestHookMover : MonoBehaviour
{
    public float fallSpeed = 2f;
    public float horizontalSpeed = 5f;

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows
        float newX = transform.position.x + horizontalInput * horizontalSpeed * Time.deltaTime;
        float newY = transform.position.y - fallSpeed * Time.deltaTime;

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}