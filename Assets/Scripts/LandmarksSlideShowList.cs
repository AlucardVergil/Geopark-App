using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BluetoothLEHardwareInterface;

public class LandmarksSlideShowList : MonoBehaviour
{
    [HideInInspector] public string[] landmarkUUIDs;

    private BeaconManager _beaconManager;
    private Dictionary<string, BeaconScannerItem> landmarkItems;

    public GameObject landmarkItemPrefab;
    public Transform landmarkPanel;

    public MapScrollViewCenterer mapScrollViewCenterer;
    public GameObject mapGameobject;


    private void Start()
    {
        _beaconManager = GameObject.FindGameObjectWithTag("BLEManager").GetComponent<BeaconManager>();

        landmarkItems = new Dictionary<string, BeaconScannerItem>();

        
    }



    public void InstantiateMapLandmarkSlides()
    {
        landmarkUUIDs = _beaconManager.GetAllNormalizedUUIDs();

        for (int i = 0; i< landmarkUUIDs.Length; i++) 
        {
            var newItem = Instantiate(landmarkItemPrefab);
            if (newItem != null)
            {
                newItem.transform.SetParent(landmarkPanel);
                newItem.transform.localScale = Vector3.one;

                newItem.transform.GetChild(0).GetComponent<Image>().sprite = _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).ImageSprite;

                newItem.GetComponentInChildren<TMP_Text>().text = _beaconManager.GetBeaconDetails(landmarkUUIDs[i]).Title;

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

}
