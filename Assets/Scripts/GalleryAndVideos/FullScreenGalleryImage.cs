using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenGalleryImage : MonoBehaviour
{
    public RectTransform imageContainer; // The container holding the image
    private GameObject fullScreenOverlay; // Optional: An overlay panel for full-screen
    public Button fullScreenButton; // Button to toggle full-screen mode
    private bool isFullScreen = false; // Track whether the image is in full-screen mode

    private Vector2 originalSize;
    private Vector3 originalPosition;

    private Transform canvas;
    private Transform originalParent;

    private GameObject[] fullScreenToHideObjects;

    private AspectRatioFitter aspectRatioFitter; // To maintain aspect ratio of the image

    private bool isPortraitMode = false; // To track if it's currently portrait mode

    private int galleryImageIndex = 0;

    void Start()
    {
        if (fullScreenButton != null)
        {
            fullScreenButton.onClick.AddListener(ToggleFullScreen);
        }

        // Store the original size and position of the image container
        originalSize = imageContainer.sizeDelta;
        originalPosition = imageContainer.position;
        originalParent = imageContainer.transform.parent;

        canvas = GameObject.FindGameObjectWithTag("Canvas").transform;

        fullScreenOverlay = GameObject.FindGameObjectWithTag("BLEManager").GetComponent<BeaconManager>().fullScreenOverlay;

        fullScreenToHideObjects = GameObject.FindGameObjectsWithTag("FullScreenToHideObjects");

        // Initialize AspectRatioFitter to maintain image aspect ratio
        aspectRatioFitter = imageContainer.GetComponent<AspectRatioFitter>();
        if (aspectRatioFitter == null)
        {
            aspectRatioFitter = imageContainer.gameObject.AddComponent<AspectRatioFitter>();
        }
        // aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
    }

    // Toggle full-screen mode
    public void ToggleFullScreen()
    {
        StartCoroutine(FullScreen());
    }

    // Fullscreen logic to change image size and position
    IEnumerator FullScreen()
    {
        if (isFullScreen)
        {
            // Wait briefly before exiting full screen
            yield return new WaitForSeconds(0.5f);

            // Exit full screen
            imageContainer.transform.parent = originalParent;

            imageContainer.SetSiblingIndex(galleryImageIndex);

            imageContainer.sizeDelta = originalSize;
            imageContainer.position = originalPosition;
            fullScreenOverlay?.SetActive(false);

            // Restore visibility of hidden objects
            for (int i = 0; i < fullScreenToHideObjects.Length; i++)
            {
                fullScreenToHideObjects[i].SetActive(true);
            }
        }
        else
        {
            // Wait briefly before entering full screen
            yield return new WaitForSeconds(0.5f);

            galleryImageIndex = imageContainer.GetSiblingIndex();

            // Enter full screen
            imageContainer.transform.parent = canvas;

            // Make sure the image is not stretched but scaled properly
            UpdateImageSize();

            fullScreenOverlay?.SetActive(true);

            // Hide other objects
            for (int i = 0; i < fullScreenToHideObjects.Length; i++)
            {
                fullScreenToHideObjects[i].SetActive(false);
            }
        }

        isFullScreen = !isFullScreen;
    }

    // Update image size while maintaining aspect ratio, adjusting for portrait and landscape
    private void UpdateImageSize()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Get image's aspect ratio
        float imageAspectRatio = (float)GetComponent<Image>().mainTexture.width / GetComponent<Image>().mainTexture.height;

        // Check if the screen is in portrait or landscape
        if (screenWidth < screenHeight) // Portrait Mode
        {
            float targetHeight = screenHeight;
            float targetWidth = targetHeight * imageAspectRatio;

            // Ensure the image fits within the screen in portrait mode, leave space if necessary
            if (targetWidth > screenWidth)
            {
                targetWidth = screenWidth;
                targetHeight = targetWidth / imageAspectRatio;
            }

            imageContainer.sizeDelta = new Vector2(targetWidth, targetHeight);
            imageContainer.position = new Vector3(screenWidth / 2, screenHeight / 2, 0);

            isPortraitMode = false;
        }
        else // Landscape Mode
        {
            float targetWidth = screenWidth;
            float targetHeight = targetWidth / imageAspectRatio;

            // Ensure the image fits within the screen in landscape mode, leave space if necessary
            if (targetHeight > screenHeight)
            {
                targetHeight = screenHeight;
                targetWidth = targetHeight * imageAspectRatio;
            }

            imageContainer.sizeDelta = new Vector2(targetWidth, targetHeight);
            imageContainer.position = new Vector3(screenWidth / 2, screenHeight / 2, 0);

            isPortraitMode = true;
        }
    }


    void Update()
    {
        // Check if the orientation has changed (portrait/landscape)
        if ((Screen.orientation == ScreenOrientation.Portrait && !isPortraitMode) ||
            (Screen.orientation == ScreenOrientation.LandscapeLeft && isPortraitMode))
        {
            // Orientation has changed, so update the image size
            UpdateImageSize();
        }
    }



}
