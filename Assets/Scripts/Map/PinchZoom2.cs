using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PinchZoom2 : MonoBehaviour
{
    public ScrollRect scrollRect; // Reference to the ScrollRect
    public RectTransform mapImage; // Reference to the map image RectTransform
    public float zoomSpeed = 0.1f; // Speed of zooming
    public float minZoom = 0.5f; // Minimum zoom scale
    public float maxZoom = 2.0f; // Maximum zoom scale

    private bool isZooming = false;

    public TMP_Text debugtext;

    void Update()
    {
        debugtext.text = "Touch " + Input.touchCount + " => " + Input.GetTouch(0) + "\n => " + Input.GetTouch(1);

        // Check if there are two touches
        if (Input.touchCount == 2)
        {
            // Disable scrolling while zooming
            if (!isZooming)
            {
                scrollRect.enabled = false;
                isZooming = true;
            }

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Calculate the current and previous distances between the two touches
            float prevDistance = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
            float currentDistance = (touch0.position - touch1.position).magnitude;

            // Calculate the scale factor
            float scaleChange = (currentDistance - prevDistance) * zoomSpeed * Time.deltaTime;

            // Apply zoom
            float newScale = Mathf.Clamp(mapImage.localScale.x + scaleChange, minZoom, maxZoom);
            mapImage.localScale = new Vector3(newScale, newScale, 1);
        }
        else if (isZooming)
        {
            // Re-enable scrolling after zooming
            scrollRect.enabled = true;
            isZooming = false;
        }
    }
}
