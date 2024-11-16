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
    }

    public void ToggleFullScreen()
    {
        if (isFullScreen)
        {
            // Exit full screen
            videoPlayerContainer.transform.parent = originalParent;

            videoPlayerContainer.sizeDelta = originalSize;
            videoPlayerContainer.position = originalPosition;
            fullScreenOverlay?.SetActive(false);
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else
        {
            // Enter full screen
            videoPlayerContainer.transform.parent = canvas;

            videoPlayerContainer.sizeDelta = new Vector2(Screen.width, Screen.height);
            videoPlayerContainer.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            fullScreenOverlay?.SetActive(true);
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            GetComponent<VideoPlayerUI>().TogglePlayPause();
        }

        isFullScreen = !isFullScreen;
    }
}
