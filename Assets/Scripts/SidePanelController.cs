using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SidePanelController : MonoBehaviour
{
    public RectTransform sidePanel; // Assign the SidePanel RectTransform here in Inspector
    public float slideSpeed = 50f; // Adjust for animation speed

    private Vector2 hiddenPosition;
    private Vector2 visiblePosition;
    private bool isPanelVisible = false;


    public List<Image> tabButtons = new List<Image>();
    public Color selectedTabColor;

    void Start()
    {
        // Set the hidden and visible positions
        hiddenPosition = new Vector2(-540f, 0);
        visiblePosition = new Vector2(540f, 0);

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


    public void QuitApplication()
    {
        Application.Quit();
    }


    public void ChangeTabButtonColor(int index)
    {
        for (int i = 0; i < tabButtons.Count; i++)
        {
            if (i == index)
            {
                tabButtons[i].GetComponent<Image>().color = selectedTabColor;
                tabButtons[i].GetComponentInChildren<TMP_Text>().color = selectedTabColor;
            }
            else
            {
                tabButtons[i].GetComponent<Image>().color = Color.white;
                tabButtons[i].GetComponentInChildren<TMP_Text>().color = Color.white;
            }

        }

    }
}
