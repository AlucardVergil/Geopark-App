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
    [HideInInspector] public int currentPanelIndex = 0; // Current panel index

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

        if (currentPanelIndex == 0)
            leftButton.gameObject.SetActive(false);

        // Adjust the layout spacing based on screen width.
        // This is calculated based on the width of the layout group - 700(which is the width of each landmark prefab) and then divided by 2 to get the padding amount for both sides.
        int paddingAmount = Mathf.RoundToInt((scrollRect.GetComponent<RectTransform>().rect.width - 700) / 2); 

        content.GetComponent<HorizontalLayoutGroup>().padding.left = paddingAmount;
        content.GetComponent<HorizontalLayoutGroup>().padding.right = paddingAmount;
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


    public void ResetAndSnapToPanel()
    {
        currentPanelIndex = 0;

        SnapToPanel(0);
    }




    public void SnapToPanel(int index)
    {
        // Hide left button when reaching 1st panel
        if (currentPanelIndex == 0)
            leftButton.gameObject.SetActive(false);
        else if (!leftButton.gameObject.activeSelf)
            leftButton.gameObject.SetActive(true);

        // Hide right button when reaching last panel
        if (currentPanelIndex == panelPositions.Length - 1)
            rightButton.gameObject.SetActive(false);
        else if (!rightButton.gameObject.activeSelf)
            rightButton.gameObject.SetActive(true);



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
