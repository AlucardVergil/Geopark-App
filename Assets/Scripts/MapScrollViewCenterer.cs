using UnityEngine.UI;
using UnityEngine;

public class MapScrollViewCenterer : MonoBehaviour
{
    public ScrollRect scrollRect; // Reference to the ScrollRect
    public RectTransform mapContent; // The RectTransform of the map image
    public RectTransform viewport; // The RectTransform of the ScrollRect's viewport

    // Centers the scroll view on a specific landmark.
    public void CenterOnLandmark(RectTransform landmark)
    {
        // Get the position of the landmark in the content's local space
        Vector2 contentLocalPosition = mapContent.InverseTransformPoint(landmark.position);

        // Get the position of the viewport's center in content's local space
        Vector2 viewportLocalPosition = mapContent.InverseTransformPoint(viewport.position);

        // Calculate the offset to move the content
        Vector2 offset = viewportLocalPosition - contentLocalPosition;

        // Adjust the content position, respecting its size limits
        Vector2 newContentPosition = mapContent.anchoredPosition + offset;

        newContentPosition.x = Mathf.Clamp(newContentPosition.x, -mapContent.rect.width + viewport.rect.width, newContentPosition.x);
        newContentPosition.y = Mathf.Clamp(newContentPosition.y, -mapContent.rect.height + viewport.rect.height, 0);

        // Set the content position
        mapContent.anchoredPosition = newContentPosition;
    }
}