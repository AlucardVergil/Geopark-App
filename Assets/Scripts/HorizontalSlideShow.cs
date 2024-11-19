using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class HorizontalSlideshow : MonoBehaviour, IEndDragHandler
{
    public ScrollRect scrollRect; // Reference to the ScrollRect
    public RectTransform content; // The content RectTransform
    public Button leftButton, rightButton; // Navigation buttons

    private float[] panelPositions; // Positions for snapping
    private int currentPanelIndex = 0; // Current panel index

    public MapScrollViewCenterer mapScrollViewCenterer;
    public GameObject mapGameobject;


    public void InitializeAfterLandmarksInstantiate()
    {
        // Calculate panel positions based on content
        int childCount = content.childCount;
        panelPositions = new float[childCount];
        for (int i = 0; i < childCount; i++)
        {
            panelPositions[i] = (float)i / (childCount - 1);
        }

        // Add listeners to buttons
        leftButton.onClick.AddListener(ScrollLeft);
        rightButton.onClick.AddListener(ScrollRight);
    }

    public void ScrollLeft()
    {
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            SnapToPanel(currentPanelIndex);
            mapScrollViewCenterer.CenterOnLandmark(mapGameobject.transform.GetChild(currentPanelIndex).GetComponent<RectTransform>());
        }
    }

    public void ScrollRight()
    {
        if (currentPanelIndex < panelPositions.Length - 1)
        {
            currentPanelIndex++;
            SnapToPanel(currentPanelIndex);
            mapScrollViewCenterer.CenterOnLandmark(mapGameobject.transform.GetChild(currentPanelIndex).GetComponent<RectTransform>());
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Find the closest panel based on current scroll position
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < panelPositions.Length; i++)
        {
            float distance = Mathf.Abs(scrollRect.horizontalNormalizedPosition - panelPositions[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentPanelIndex = i;
                mapScrollViewCenterer.CenterOnLandmark(mapGameobject.transform.GetChild(currentPanelIndex).GetComponent<RectTransform>());
            }
        }
        SnapToPanel(currentPanelIndex);
    }

    private void SnapToPanel(int index)
    {
        StartCoroutine(SmoothScrollTo(panelPositions[index]));
    }

    IEnumerator SmoothScrollTo(float targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 0.3f; // Smooth scroll duration
        float startPosition = scrollRect.horizontalNormalizedPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startPosition, targetPosition, elapsedTime / duration);
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = targetPosition;
    }
}
