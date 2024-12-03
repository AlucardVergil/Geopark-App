using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AdjustMainImageContainer : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(AdjustContainerSize());            
    }


    IEnumerator AdjustContainerSize()
    {
        yield return new WaitForSeconds(0.1f);

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float imageAspectRatio = (float)GetComponent<Image>().mainTexture.width / GetComponent<Image>().mainTexture.height;

        float targetWidth = screenWidth;
        float targetHeight = targetWidth / imageAspectRatio;

        var rectTransform = GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
    }
}
