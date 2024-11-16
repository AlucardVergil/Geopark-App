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
        StartCoroutine(FullScreen());
    }


    IEnumerator FullScreen()
    {
        if (isFullScreen)
        {
            Screen.orientation = ScreenOrientation.Portrait;

            yield return new WaitForSeconds(0.5f);

            // Exit full screen
            videoPlayerContainer.transform.parent = originalParent;

            videoPlayerContainer.sizeDelta = originalSize;
            videoPlayerContainer.position = originalPosition;
            fullScreenOverlay?.SetActive(false);
            fullScreenButton.GetComponent<Image>().sprite = fullScreenIcon;
        }
        else
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            yield return new WaitForSeconds(0.5f);

            // Enter full screen
            videoPlayerContainer.transform.parent = canvas;

            videoPlayerContainer.sizeDelta = new Vector2(Screen.width, Screen.height);
            videoPlayerContainer.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            fullScreenOverlay?.SetActive(true);
            fullScreenButton.GetComponent<Image>().sprite = minimizeScreenIcon;

            GetComponent<VideoPlayerUI>().TogglePlayPause();
        }

        isFullScreen = !isFullScreen;
    }
}
