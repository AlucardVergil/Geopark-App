using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenVideo : MonoBehaviour
{
    public RectTransform videoPlayerContainer; // The container holding the video
    private GameObject fullScreenOverlay;      // Optional: An overlay panel for full-screen
    public Button fullScreenButton;
    private bool isFullScreen = false;

    private Vector2 originalSize;
    private Vector3 originalPosition;

    private Transform canvas;
    private Transform originalParent;

    public Sprite fullScreenIcon; // Icon for play
    public Sprite minimizeScreenIcon; // Icon for pause

    private GameObject [] fullScreenToHideObjects;

    private GameObject closeFullScreenButton;



    void Start()
    {
        if (fullScreenButton != null)
        {
            fullScreenButton.onClick.AddListener(ToggleFullScreen);
        }

        // Store the original size and position of the video container
        originalSize = videoPlayerContainer.sizeDelta;
        originalPosition = videoPlayerContainer.position;
        originalParent = videoPlayerContainer.transform.parent;

        canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
        fullScreenOverlay = GameObject.FindGameObjectWithTag("BLEManager").GetComponent<BeaconManager>().fullScreenOverlay;

        fullScreenToHideObjects = GameObject.FindGameObjectsWithTag("FullScreenToHideObjects");

        closeFullScreenButton = fullScreenOverlay.transform.GetChild(0).gameObject;
    }

    public void ToggleFullScreen()
    {
        StartCoroutine(FullScreen());
    }


    IEnumerator FullScreen()
    {
        if (isFullScreen)
        {
            closeFullScreenButton.SetActive(true);

            Screen.orientation = ScreenOrientation.Portrait;

            yield return new WaitForSeconds(0.5f);

            // Exit full screen
            videoPlayerContainer.transform.parent = originalParent;

            videoPlayerContainer.sizeDelta = originalSize;
            videoPlayerContainer.position = originalPosition;
            fullScreenOverlay?.SetActive(false);
            fullScreenButton.GetComponent<Image>().sprite = fullScreenIcon;

            GetComponent<VideoPlayerUI>().playPauseButton.image.sprite = GetComponent<VideoPlayerUI>().playIcon;

            //GetComponent<VideoPlayerUI>().TogglePlayPause();

            for (int i = 0; i < fullScreenToHideObjects.Length; i++)
            {
                fullScreenToHideObjects[i].SetActive(true);
            }                           
        }
        else
        {
            closeFullScreenButton.SetActive(false);

            Screen.orientation = ScreenOrientation.LandscapeLeft;

            yield return new WaitForSeconds(0.5f);

            // Enter full screen
            videoPlayerContainer.transform.parent = canvas;

            videoPlayerContainer.sizeDelta = new Vector2(Screen.width, Screen.height);
            videoPlayerContainer.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            fullScreenOverlay?.SetActive(true);
            fullScreenButton.GetComponent<Image>().sprite = minimizeScreenIcon;

            GetComponent<VideoPlayerUI>().TogglePlayPause();

            for (int i = 0; i < fullScreenToHideObjects.Length; i++)
            {
                fullScreenToHideObjects[i].SetActive(false);
            }
        }

        isFullScreen = !isFullScreen;
    }
}
