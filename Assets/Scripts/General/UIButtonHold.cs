using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent onClick;
    public UnityEvent onHold;

    private bool isPointerDown = false;
    private bool holdStarted = false;

    void Update()
    {
        if (!isPointerDown) return;

        holdStarted = true;
        onHold.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        holdStarted = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!holdStarted)
        {
            onClick.Invoke();
        }

        isPointerDown = false;
    }
}