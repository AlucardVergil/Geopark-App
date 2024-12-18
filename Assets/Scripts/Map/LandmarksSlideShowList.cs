using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LandmarksSlideShowList : MonoBehaviour
{
    [HideInInspector] public string[] landmarkUUIDs;

    private BeaconManager _beaconManager;
    private Dictionary<string, BeaconScannerItem> landmarkItems;

    public GameObject landmarkItemPrefab;
    public Transform landmarkPanel;    

    public MapScrollViewCenterer mapScrollViewCenterer;
    public GameObject mapGameobject;

    public Transform verticalLandmarkPanel;
    public GameObject horizontalSlideShowScrollView;

    public GameObject verticalListLandmarkItemPrefab;

    public RectTransform verticalListScrollViewSidePanel; // Assign the SidePanel RectTransform here in Inspector
    public float slideSpeed = 15f; // Adjust for animation speed

    private Vector2 hiddenPosition;
    private Vector2 visiblePosition;


    private void Start()
    {
        _beaconManager = GameObject.FindGameObjectWithTag("BLEManager").GetComponent<BeaconManager>();

        landmarkItems = new Dictionary<string, BeaconScannerItem>();



        // Set the hidden and visible positions
        hiddenPosition = new Vector2(1082f, -161.01f);
        visiblePosition = new Vector2(-10f, -161.01f);

        // Start the panel in the hidden position
        verticalListScrollViewSidePanel.anchoredPosition = hiddenPosition;


    }   



    public void InstantiateMapLandmarkSlides()
    {
        landmarkUUIDs = _beaconManager.GetAllNormalizedUUIDs();

        for (int i = 0; i< landmarkUUIDs.Length; i++) 
        {
            var newItem = Instantiate(landmarkItemPrefab);
            if (newItem != null)
            {
                newItem.GetComponent<PrefabLanguage>().greekTitle = _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).Title;
                newItem.GetComponent<PrefabLanguage>().englishTitle = _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).TitleEnglish;

                newItem.transform.SetParent(landmarkPanel);
                newItem.transform.localScale = Vector3.one;

                newItem.transform.GetChild(0).GetComponent<Image>().sprite = _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).ImageSprite;

                newItem.GetComponentInChildren<TMP_Text>().text = (!_beaconManager.isEnglish ? _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).Title : _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).TitleEnglish);

                // Create a local copy of the index
                /*When the onClick listener is assigned inside the loop, the lambda captures the variable i by reference, not its value at the time of the loop. 
                As a result, when the listener executes, it uses the last value of i after the loop finishes.
                To fix this, I created a local copy of the i variable inside the loop. */
                int index = i;

                newItem.GetComponent<Button>().onClick.AddListener(() =>
                {
                    mapScrollViewCenterer.CenterOnLandmark(mapGameobject.transform.GetChild(index).GetComponent<RectTransform>());
                });
            }   
            
        } 
        
        GetComponentInChildren<HorizontalSlideshow>().InitializeAfterLandmarksInstantiate();

    }


    public void OpenPanel()
    {
        StopAllCoroutines();
        StartCoroutine(OpenSlidePanel());
    }

    private IEnumerator OpenSlidePanel()
    {
        while (Vector2.Distance(verticalListScrollViewSidePanel.anchoredPosition, visiblePosition) > 0.1f)
        {
            verticalListScrollViewSidePanel.anchoredPosition = Vector2.Lerp(verticalListScrollViewSidePanel.anchoredPosition, visiblePosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
        verticalListScrollViewSidePanel.anchoredPosition = visiblePosition;
    }


    public void ClosePanel()
    {
        StopAllCoroutines();
        StartCoroutine(CloseSlidePanel());
    }

    private IEnumerator CloseSlidePanel()
    {
        while (Vector2.Distance(verticalListScrollViewSidePanel.anchoredPosition, hiddenPosition) > 0.1f)
        {
            verticalListScrollViewSidePanel.anchoredPosition = Vector2.Lerp(verticalListScrollViewSidePanel.anchoredPosition, hiddenPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
        verticalListScrollViewSidePanel.anchoredPosition = hiddenPosition;
    }



    public void InstantiateMapLandmarkVerticalMapList()
    {
        landmarkUUIDs = _beaconManager.GetAllNormalizedUUIDs();

        for (int i = 0; i < landmarkUUIDs.Length; i++)
        {
            var newItem = Instantiate(verticalListLandmarkItemPrefab);
            if (newItem != null)
            {
                newItem.GetComponent<PrefabLanguage>().greekTitle = _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).Title;
                newItem.GetComponent<PrefabLanguage>().englishTitle = _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).TitleEnglish;

                newItem.transform.SetParent(verticalLandmarkPanel);
                newItem.transform.localScale = Vector3.one;

                newItem.transform.GetChild(0).GetComponent<Image>().sprite = _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).ImageSprite;

                newItem.GetComponentInChildren<TMP_Text>().text = (!_beaconManager.isEnglish ? _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).Title : _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).TitleEnglish);

                // Create a local copy of the index
                /*When the onClick listener is assigned inside the loop, the lambda captures the variable i by reference, not its value at the time of the loop. 
                As a result, when the listener executes, it uses the last value of i after the loop finishes.
                To fix this, I created a local copy of the i variable inside the loop. */
                int index = i;

                newItem.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ClosePanel();
                    horizontalSlideShowScrollView.GetComponent<HorizontalSlideshow>().currentPanelIndex = index;
                    horizontalSlideShowScrollView.GetComponent<HorizontalSlideshow>().SnapToPanel(index);
                    mapScrollViewCenterer.CenterOnLandmark(mapGameobject.transform.GetChild(index).GetComponent<RectTransform>());
                });
            }

        }

        //GetComponentInChildren<HorizontalSlideshow>().InitializeAfterLandmarksInstantiate();

    }

}
