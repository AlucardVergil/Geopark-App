using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MapScrollViewCenterer : MonoBehaviour
{
    public ScrollRect scrollRect; // Reference to the ScrollRect
    public RectTransform mapContent; // The RectTransform of the map image
    public RectTransform viewport; // The RectTransform of the ScrollRect's viewport
    public Transform mapGameobject;

    // Centers the scroll view on a specific landmark.
    public void CenterOnLandmark(RectTransform landmark)
    {
        // Add pulsating effect on selected landmark map icon and remove it when you switch to a different point
        PulsateEffect[] components = mapGameobject.GetComponentsInChildren<PulsateEffect>();
        foreach (PulsateEffect component in components)
        {
            if (component.gameObject != landmark.gameObject)             
                Destroy(component); // Remove the component
        }
        if (!landmark.TryGetComponent<PulsateEffect>(out _))
            landmark.AddComponent<PulsateEffect>();


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
        StartCoroutine(SmoothCentering(newContentPosition));
    }


    IEnumerator SmoothCentering(Vector2 targetPosition)
    {
        float duration = 0.5f; // Animation duration
        Vector2 initialPosition = mapContent.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mapContent.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        mapContent.anchoredPosition = targetPosition;
    }

}