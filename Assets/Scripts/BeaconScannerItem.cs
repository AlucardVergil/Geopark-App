using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEditorInternal.Profiling.Memory.Experimental;
using System.Collections;
using Unity.XR.CoreUtils;

public class BeaconScannerItem : MonoBehaviour
{
	public TMP_Text TextTitleFromUUID;
	public TMP_Text TextRSSIValue;
	public TMP_Text TextAndroidSignalPower;
	public TMP_Text TextDistance;
	public TMP_Text TextiOSProximity;

	[HideInInspector] public string UUID;


    private BeaconManager _beaconManager;
    private GameObject landmarkDetails;
    private GameObject mainMenu;


    private void Start()
    {
        _beaconManager = GameObject.Find("BLEManager").GetComponent<BeaconManager>();

        landmarkDetails = _beaconManager.landmarkDetails;

        mainMenu = GameObject.Find("MainMenu");

        StartCoroutine(AddClickEvent());
    }



    IEnumerator AddClickEvent()
    {
        yield return new WaitForSeconds(0.5f);

        GetComponent<Button>().onClick.AddListener(() =>
        {
            // Get Beacon Details
            BeaconDetails details = _beaconManager.GetBeaconDetails(UUID);

            // Check if details are found
            if (details != null)
            {
                Debug.Log("Beacon info: " + details.Info);

                // Show the details UI
                landmarkDetails.SetActive(true);

                landmarkDetails.GetNamedChild("Title").GetComponent<TMP_Text>().text = details.Title;

                landmarkDetails.GetNamedChild("ContentText").GetComponent<TMP_Text>().text = details.Info;

                landmarkDetails.GetNamedChild("Image").GetComponent<Image>().sprite = details.ImageSprite;

                mainMenu.SetActive(false);

            }
            else
            {
                Debug.LogWarning("No details found for this beacon.");
            }
        });

    }
}
