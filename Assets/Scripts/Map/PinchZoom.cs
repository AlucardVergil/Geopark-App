using UnityEngine;
using UnityEngine.UI;

public class PinchZoom : MonoBehaviour
{
    public RectTransform mapContent; // The RectTransform of the map image
    public float zoomSpeed = 0.1f; // Speed of zooming
    public float minZoom = 0.5f; // Minimum zoom level
    public float maxZoom = 2.0f; // Maximum zoom level

    private Vector2 lastTouchPos;
    private Vector2 prevTouchPos;

    void Update()
    {
        // Check if there are two touches on the device
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Get the current and previous positions of the touches
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            // Get the distance between the touches in each frame
            float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
            float touchDeltaMag = (touch1.position - touch2.position).magnitude;

            // Calculate the difference in distances between each frame
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Apply zoom
            Zoom(deltaMagnitudeDiff * zoomSpeed);
        }
    }

    void Zoom(float increment)
    {
        Vector3 newScale = mapContent.localScale - new Vector3(increment, increment, increment);
        newScale.x = Mathf.Clamp(newScale.x, minZoom, maxZoom);
        newScale.y = Mathf.Clamp(newScale.y, minZoom, maxZoom);
        newScale.z = 1f; // Keep the z scale at 1

        mapContent.localScale = newScale;
    }
}

