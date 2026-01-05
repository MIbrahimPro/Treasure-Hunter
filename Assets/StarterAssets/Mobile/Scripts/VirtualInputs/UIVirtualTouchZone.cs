using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIVirtualTouchZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [System.Serializable]
    public class Event : UnityEvent<Vector2> { }

    [Header("Settings")]
    public float sensitivity = 1f; // Start at 1 with the new scaling
    public bool invertX;
    public bool invertY;

    [Tooltip("Minimum pixels to move before registering")]
    public float deadZone = 1f;

    [Header("Output")]
    public Event touchZoneOutputEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Output zero on initial touch to clear previous input
        touchZoneOutputEvent.Invoke(Vector2.zero);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Use delta directly. 
        // IMPORTANT: In the receiving script, DO NOT multiply by Time.deltaTime.
        Vector2 delta = eventData.delta;

        if (delta.magnitude < deadZone)
        {
            // Don't invoke zero here; just ignore the tiny movement 
            // to prevent "stuttering" during slow drags.
            return;
        }

        if (invertX) delta.x = -delta.x;
        if (invertY) delta.y = -delta.y;

        // Apply sensitivity and send
        touchZoneOutputEvent.Invoke(delta * sensitivity);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touchZoneOutputEvent.Invoke(Vector2.zero);
    }
}