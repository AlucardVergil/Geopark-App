using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomMap : MonoBehaviour
{
    private RectTransform map;
    private int zoomIndex = 0;
    private int prevZoomIndex = 0;

    private Vector2 originalSize;
    private float aspectRatio;

    [Header("Set the width increase for zoom in and zoom out. The \nheight is automatically calculated based on the aspect ratio.")]
    public float widthIncreaseAmount = 500f;
    private float heightIncreaseAmount;

    Vector2[] landmarkOriginalPositions;

    // Start is called before the first frame update
    void Start()
    {
        map = GetComponent<RectTransform>();

        originalSize = map.sizeDelta;

        aspectRatio = map.rect.width / map.rect.height;

        heightIncreaseAmount = (aspectRatio > 1 ? widthIncreaseAmount / aspectRatio : widthIncreaseAmount * aspectRatio);

        landmarkOriginalPositions = new Vector2[map.childCount];

        for (int i = 0; i < map.childCount; i++)
        {
            landmarkOriginalPositions[i] = map.GetChild(i).GetComponent<RectTransform>().anchoredPosition;            
        }
    }

    

    public void ZoomIn()
    {
        prevZoomIndex = zoomIndex;
        zoomIndex++;
        zoomIndex = Math.Clamp(zoomIndex, -3, 12);

        if (zoomIndex <= 12 && prevZoomIndex != zoomIndex)
        {
            UpdateChildrenPosition();
        }        
    }



    public void ZoomOut()
    {
        prevZoomIndex = zoomIndex;
        zoomIndex--;
        zoomIndex = Math.Clamp(zoomIndex, -3, 12);

        if (zoomIndex > -4 && prevZoomIndex != zoomIndex)
        {            
            UpdateChildrenPosition();
        }
    }


    private void OnDisable()
    {
        map.sizeDelta = originalSize;
        zoomIndex = 0;

        for (int i = 0; i < map.childCount; i++)
        {
            map.GetChild(i).GetComponent<RectTransform>().anchoredPosition = landmarkOriginalPositions[i];
        }
    }





    void UpdateChildrenPosition()
    {
        Vector2[] normalizedPosition = new Vector2[map.childCount];

        for (int i = 0; i < map.childCount; i++)
        {
            normalizedPosition[i] = new Vector2(
                map.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x / map.sizeDelta.x,
                map.GetChild(i).GetComponent<RectTransform>().anchoredPosition.y / map.sizeDelta.y
            );
        }

        if (prevZoomIndex < zoomIndex)
            map.sizeDelta += new Vector2(widthIncreaseAmount, heightIncreaseAmount);
        else
            map.sizeDelta -= new Vector2(widthIncreaseAmount, heightIncreaseAmount);


        for (int i = 0; i < map.childCount; i++)
        {
            // Update the new size
            map.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(
                normalizedPosition[i].x * map.sizeDelta.x,
                normalizedPosition[i].y * map.sizeDelta.y
            );
        }          

            
        
    }

}
