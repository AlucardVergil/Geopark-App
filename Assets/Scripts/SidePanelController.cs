using System.Collections;
using UnityEngine;

public class SidePanelController : MonoBehaviour
{
    public RectTransform sidePanel; // Assign the SidePanel RectTransform here in Inspector
    public float slideSpeed = 50f; // Adjust for animation speed

    private Vector2 hiddenPosition;
    private Vector2 visiblePosition;
    private bool isPanelVisible = false;

    void Start()
    {
        // Set the hidden and visible positions
        hiddenPosition = new Vector2(-338.83f, 0);
        visiblePosition = new Vector2(338.83f, 0);

        // Start the panel in the hidden position
        sidePanel.anchoredPosition = hiddenPosition;
    }

    public void TogglePanel()
    {
        StopAllCoroutines();
        StartCoroutine(SlidePanel());
    }

    private IEnumerator SlidePanel()
    {
        Vector2 targetPosition = isPanelVisible ? hiddenPosition : visiblePosition;
        while (Vector2.Distance(sidePanel.anchoredPosition, targetPosition) > 0.1f)
        {
            sidePanel.anchoredPosition = Vector2.Lerp(sidePanel.anchoredPosition, targetPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
        sidePanel.anchoredPosition = targetPosition;
        isPanelVisible = !isPanelVisible;
    }
}
