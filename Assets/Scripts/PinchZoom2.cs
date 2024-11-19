using UnityEngine;
using UnityEngine.EventSystems;

public class PinchZoom2 : MonoBehaviour
{
    public RectTransform mapImage;  // Reference to the map image's RectTransform
    public float zoomSpeed = 0.1f;  // Speed of zooming
    public float minZoom = 0.5f;    // Minimum zoom scale
    public float maxZoom = 3f;      // Maximum zoom scale

    private Vector2 prevTouchDelta; // Previous distance between two touches
    private bool isPinching = false;

    void Update()
    {
        if (Input.touchCount == 2) // Check for two-finger touch
        {
            // Get the touches
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Calculate the current distance between the two touches
            Vector2 touch1Pos = touch1.position;
            Vector2 touch2Pos = touch2.position;
            Vector2 currentTouchDelta = touch1Pos - touch2Pos;

            if (!isPinching) // Initialize the pinch
            {
                prevTouchDelta = currentTouchDelta;
                isPinching = true;
            }
            else
            {
                // Calculate the scale factor based on the change in distance
                float prevDistance = prevTouchDelta.magnitude;
                float currentDistance = currentTouchDelta.magnitude;
                float zoomFactor = (currentDistance - prevDistance) * zoomSpeed;

                // Adjust the scale of the map image
                Vector3 newScale = mapImage.localScale + Vector3.one * zoomFactor;
                newScale.x = Mathf.Clamp(newScale.x, minZoom, maxZoom);
                newScale.y = Mathf.Clamp(newScale.y, minZoom, maxZoom);
                newScale.z = 1f; // Keep Z scale constant

                mapImage.localScale = newScale;

                // Update previous touch delta
                prevTouchDelta = currentTouchDelta;
            }
        }
        else
        {
            isPinching = false; // Reset pinch state when not touching
        }
    }
}
