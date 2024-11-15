using UnityEngine;

public class LockXAxis : MonoBehaviour
{
    private RectTransform rectTransform;
    private float initialX;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialX = rectTransform.anchoredPosition.x; // Store the initial X position
    }

    void Update()
    {
        // Restrict X position
        Vector2 position = rectTransform.anchoredPosition;
        position.x = initialX; // Lock the X position to the initial value
        rectTransform.anchoredPosition = position;
    }
}
