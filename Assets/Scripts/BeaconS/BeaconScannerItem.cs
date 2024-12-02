using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEditorInternal.Profiling.Memory.Experimental;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.Video;

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

    public GameObject galleryImagePrefab;
    GameObject scrollViewGallery;
    GameObject galleryScrollViewContent;

    public GameObject videoPrefab;
    GameObject scrollViewVideos;
    GameObject videosScrollViewContent;

    GameObject servicesList;

    GameObject accomodation;
    GameObject food;
    GameObject parking;
    GameObject walking;


    private void Start()
    {
        _beaconManager = GameObject.FindGameObjectWithTag("BLEManager").GetComponent<BeaconManager>();

        landmarkDetails = _beaconManager.landmarkDetails;

        mainMenu = GameObject.FindGameObjectWithTag("MainMenu");

        servicesList = landmarkDetails.GetNamedChild("ServicesList");

        accomodation = servicesList.GetNamedChild("Accomodation");
        food = servicesList.GetNamedChild("Food");
        parking = servicesList.GetNamedChild("Parking");
        walking = servicesList.GetNamedChild("Walking");

        StartCoroutine(AddClickEvent());
    }



    IEnumerator AddClickEvent()
    {
        yield return new WaitForSeconds(0.5f);

        GetComponent<Button>().onClick.AddListener(() =>
        {
            // Get Beacon Details
            BeaconDetails details = _beaconManager.GetBeaconGalleryImages(UUID);

            // Check if details are found
            if (details != null)
            {
                Debug.Log("Beacon info: " + details.Info);

                // Show the details UI
                landmarkDetails.SetActive(true);

                landmarkDetails.GetNamedChild("Title").GetComponent<TMP_Text>().text = details.Title;

                landmarkDetails.GetNamedChild("ContentText").GetComponent<TMP_Text>().text = details.Info;

                // Scroll text content to the top when opened
                var rectTransform = landmarkDetails.GetNamedChild("Content").GetComponent<RectTransform>();
                Vector2 currentPosition = rectTransform.anchoredPosition;
                currentPosition.y = 0;
                rectTransform.anchoredPosition = currentPosition;
                

                landmarkDetails.GetNamedChild("MainImage").GetComponent<Image>().sprite = details.ImageSprite;

                landmarkDetails.GetComponentInChildren<FavoritesButton>().UUID = UUID;


                
                scrollViewGallery = landmarkDetails.GetNamedChild("Scroll View Gallery");

                galleryScrollViewContent = scrollViewGallery.GetNamedChild("GalleryContent");                

                GameObject galleryParentGameobject = new GameObject(UUID);
                galleryParentGameobject.transform.parent = galleryScrollViewContent.transform;
                GridLayoutGroup galleryParentLayout = galleryParentGameobject.AddComponent<GridLayoutGroup>();
                galleryParentLayout.padding = new (20,20,20,20);
                galleryParentLayout.cellSize = new Vector2(450,450);
                galleryParentLayout.spacing = new Vector2(20,20);
                galleryParentLayout.childAlignment = TextAnchor.UpperCenter;

                if (galleryParentGameobject.transform.childCount == 0)
                {
                    for (int i = 0; i < details.GallerySprites.Count; i++)
                    {
                        var galleryImage = Instantiate(galleryImagePrefab, galleryParentGameobject.transform);
                        galleryImage.GetComponent<Image>().sprite = details.GallerySprites[i];
                    }
                }


                scrollViewVideos = landmarkDetails.GetNamedChild("Scroll View Videos");

                videosScrollViewContent = scrollViewVideos.GetNamedChild("VideosContent");

                GameObject videoParentGameobject = new GameObject(UUID);
                videoParentGameobject.transform.parent = videosScrollViewContent.transform;
                VerticalLayoutGroup videoParentLayout = videoParentGameobject.AddComponent<VerticalLayoutGroup>();
                videoParentLayout.padding = new(20, 20, 20, 20);
                videoParentLayout.spacing = 50;
                videoParentLayout.childAlignment = TextAnchor.UpperCenter;
                videoParentLayout.childControlHeight = false;
                videoParentLayout.childControlWidth = false;

                if (videoParentGameobject.transform.childCount == 0)
                {
                    for (int i = 0; i < details.VideoURLs.Count; i++)
                    {
                        var video = Instantiate(videoPrefab, videoParentGameobject.transform);
                        VideoPlayer videoPlayer = video.GetComponent<VideoPlayer>();
                        RawImage rawImage = video.GetComponent<RawImage>();

                        // Create a unique RenderTexture for this video
                        RenderTexture renderTexture = new RenderTexture(1920, 1080, 0); // Adjust size as needed
                        videoPlayer.targetTexture = renderTexture;

                        // Assign the RenderTexture to the RawImage
                        rawImage.texture = renderTexture;

                        // Create a local copy of the index
                        /*When the onClick listener is assigned inside the loop, the lambda captures the variable i by reference, not its value at the time of the loop. 
                        As a result, when the listener executes, it uses the last value of i after the loop finishes.
                        To fix this, I created a local copy of the i variable inside the loop. */
                        int index = i;

                        _beaconManager.PlayVideo(UUID, index, videoPlayer);
                        //video.GetComponent<Button>().onClick.AddListener(() =>
                        //{
                        //    _beaconManager.PlayVideo(UUID, index, videoPlayer);


                        //});
                    }
                }


                // Enable and disable sevices icons based on landmark
                foreach (Transform child in servicesList.transform)
                {
                    child.gameObject.SetActive(false);
                }

                if (details.Accomodation == "yes")
                    accomodation.SetActive(true);

                if (details.Food == "yes")
                    food.SetActive(true);

                if (details.Parking == "yes")
                    parking.SetActive(true);

                if (details.Walking == "yes")
                    walking.SetActive(true);





                scrollViewGallery.SetActive(false);

                scrollViewVideos.SetActive(false);

                mainMenu.SetActive(false);

            }
            else
            {
                Debug.LogWarning("No details found for this beacon.");
            }
        });

    }
}
