using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class BeaconManager : MonoBehaviour
{
    [HideInInspector] public BeaconDetailsList beaconDetailsList; // Stores the list of beacon details
    private Dictionary<string, BeaconDetails> beaconDetailsDictionary; // Dictionary for quick lookup

    public GameObject landmarkDetails;
    public GameObject loadingScreen;
    public Slider progressBar;

    private string fileId = "1qTmB5Z3HHHe_ue2LRL66YgwntuK-s-w6";
    private string localFilePath;

    private int filesDownloaded = 0; // Track downloaded files
    private int totalFiles = 0;

    public GameObject fullScreenOverlay;

    public GameObject favoritesPanel;

    public GameObject mapPanel;

    public GameObject infoPanel;

    HashSet<string> validFiles = new HashSet<string>();

    GameObject scrollViewGallery;
    GameObject galleryScrollViewContent;

    GameObject scrollViewVideos;
    GameObject videosScrollViewContent;

    public Transform servicesList;

    public bool isEnglish = false;

    public Image languageButton;
    public Sprite greekFlagSprite;
    public Sprite englishFlagSprite;


    void Start()
    {
        fullScreenOverlay.SetActive(false);

        landmarkDetails.SetActive(false);
        favoritesPanel.SetActive(false);
        infoPanel.SetActive(false);
        loadingScreen.SetActive(true);
        

        progressBar.value = 0f;

        localFilePath = Path.Combine(Application.persistentDataPath, "BeaconData.json");

        // Start downloading the JSON file; if it fails, load the local copy
        StartCoroutine(TryDownloadOrLoadLocalJSON());
    }



    public void SwitchLanguage()
    {
        isEnglish = !isEnglish;

        if (isEnglish)
            languageButton.sprite = greekFlagSprite;
        else
            languageButton.sprite = englishFlagSprite;
    }


    private IEnumerator TryDownloadOrLoadLocalJSON()
    {
        string url = $"https://drive.google.com/uc?id={fileId}&export=download";
        int totalFiles = 1; // JSON file counts as 1 file initially

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            File.WriteAllText(localFilePath, json);
            Debug.Log("Downloaded JSON and saved to: " + localFilePath);

            LoadBeaconData(json);
                      
            yield return StartCoroutine(DownloadImages());
            yield return StartCoroutine(DownloadVideos());
        }
        else
        {
            Debug.LogWarning("Failed to download JSON, loading local file if it exists.");

            if (File.Exists(localFilePath))
            {
                string json = File.ReadAllText(localFilePath);
                LoadBeaconData(json);

                yield return StartCoroutine(DownloadImages());
                yield return StartCoroutine(DownloadVideos());
            }
            else
            {
                Debug.LogError("No local JSON file found. The app needs an internet connection to download the JSON initially.");
            }
        }

        mapPanel.GetComponent<LandmarksSlideShowList>().InstantiateMapLandmarkSlides();
        mapPanel.GetComponent<LandmarksSlideShowList>().InstantiateMapLandmarkVerticalMapList();

        mapPanel.SetActive(false);

        loadingScreen.SetActive(false); // Hide loading screen after all downloads are complete
    }

    private void LoadBeaconData(string json)
    {
        beaconDetailsList = JsonUtility.FromJson<BeaconDetailsList>(json);

        beaconDetailsDictionary = new Dictionary<string, BeaconDetails>();
        foreach (var beacon in beaconDetailsList.Beacons)
        {
            beaconDetailsDictionary[beacon.UUID] = beacon;
        }

        totalFiles = beaconDetailsList.Beacons.Count + beaconDetailsList.Beacons.Sum(b => b.GalleryImages.Count + b.VideoURLs.Count);

        filesDownloaded++;
        UpdateProgressBar(filesDownloaded, totalFiles);

        Debug.Log("Beacon data loaded successfully.");
    }




    // Extract the unique ID from a Google Drive URL or similar URL formats
    private string ExtractUniqueID(string url)
    {
        // Check if the URL contains the "id=" parameter
        if (url.Contains("id="))
        {
            // Extract the part after "id="
            var uri = new Uri(url);
            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return queryParams["id"]; // Returns the unique ID
        }

        // If no "id=" parameter, use a fallback like the last segment
        return Path.GetFileNameWithoutExtension(url);
    }




    private IEnumerator DownloadImages()
    {
        //HashSet<string> validFiles = new HashSet<string>();

        foreach (var beacon in beaconDetailsList.Beacons)
        {
            string mainImageID = ExtractUniqueID(beacon.ImageURL);

            // Download the main image
            string mainImagePath = Path.Combine(Application.persistentDataPath, $"{mainImageID}.png");
            validFiles.Add(mainImageID + ".png");

            if (!File.Exists(mainImagePath))
            {
                yield return StartCoroutine(DownloadImage(beacon.ImageURL, mainImagePath, sprite =>
                {
                    beacon.ImageSprite = sprite;
                    Debug.Log($"Downloaded main image for UUID {beacon.UUID}");
                }));
                filesDownloaded++;
                UpdateProgressBar(filesDownloaded, totalFiles);
            }

            // Download gallery images
            for (int i = 0; i < beacon.GalleryImages.Count; i++)
            {
                string galleryImageID = ExtractUniqueID(beacon.GalleryImages[i]);
                string galleryImagePath = Path.Combine(Application.persistentDataPath, $"{galleryImageID}.png");
                validFiles.Add(galleryImageID + ".png");

                if (!File.Exists(galleryImagePath))
                {
                    string galleryImageUrl = beacon.GalleryImages[i];
                    yield return StartCoroutine(DownloadImage(galleryImageUrl, galleryImagePath, sprite =>
                    {
                        beacon.GallerySprites.Add(sprite);
                        Debug.Log($"Downloaded gallery image {i + 1} for UUID {beacon.UUID}");
                    }));
                    filesDownloaded++;
                    UpdateProgressBar(filesDownloaded, totalFiles);
                }
            }
        }


        // Delete orphaned files
        var existingFiles = Directory.GetFiles(Application.persistentDataPath);
        foreach (var file in existingFiles)
        {
            if (file.Contains(".png") && !validFiles.Contains(Path.GetFileName(file)))
            {
                File.Delete(file);
                Debug.Log($"Deleted orphaned file: {file}");
            }
        }
    }


    private IEnumerator DownloadImage(string url, string savePath, System.Action<Sprite> onDownloaded)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                byte[] imageData = texture.EncodeToPNG();
                File.WriteAllBytes(savePath, imageData);

                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                onDownloaded?.Invoke(sprite);
            }
            else
            {
                Debug.LogError($"Failed to download image from {url}: {request.error}");
            }
        }
    }



    private void UpdateProgressBar(int filesDownloaded, int totalFiles)
    {
        if (progressBar != null)
        {
            progressBar.value = (float)filesDownloaded / totalFiles;
        }
    }



    public BeaconDetails GetBeaconDetails(string uuid)
    {
        BeaconDetails details = null;

        foreach (var beacon in beaconDetailsList.Beacons)
        {
            if (beacon.UUID == uuid)
            {
                details = beacon;

                string mainImageID = ExtractUniqueID(beacon.ImageURL);

                // Load the main image
                string mainImagePath = Path.Combine(Application.persistentDataPath, $"{mainImageID}.png");
                if (File.Exists(mainImagePath))
                {
                    byte[] imageData = File.ReadAllBytes(mainImagePath);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);

                    details.ImageSprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                }
                else
                {
                    Debug.LogWarning($"Main image for UUID {uuid} not found locally.");
                }                

                break;
            }
        }

        if (details == null)
        {
            Debug.LogError($"No beacon found with UUID: {uuid}");
        }

        return details;
    }




    public BeaconDetails GetBeaconGalleryImages(string uuid)
    {
        BeaconDetails details = null;

        foreach (var beacon in beaconDetailsList.Beacons)
        {
            if (beacon.UUID == uuid)
            {
                details = beacon;                

                // Clear any previously loaded gallery sprites to avoid duplicates
                details.GallerySprites.Clear();
                
                // Load gallery images
                for (int i = 0; i < beacon.GalleryImages.Count; i++)
                {
                    string galleryImageID = ExtractUniqueID(beacon.GalleryImages[i]);

                    string galleryImagePath = Path.Combine(Application.persistentDataPath, $"{galleryImageID}.png");
                    if (File.Exists(galleryImagePath))
                    {
                        byte[] imageData = File.ReadAllBytes(galleryImagePath);
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(imageData);

                        Sprite gallerySprite = Sprite.Create(
                            texture,
                            new Rect(0, 0, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f)
                        );

                        details.GallerySprites.Add(gallerySprite);
                    }
                    else
                    {
                        Debug.LogWarning($"Gallery image {i + 1} for UUID {uuid} not found locally.");
                    }
                }

                break;
            }
        }

        if (details == null)
        {
            Debug.LogError($"No beacon found with UUID: {uuid}");
        }

        return details;
    }




    public string[] GetAllUUIDs()
    {
        List<string> uuidsList = new List<string>();

        foreach (var beacon in beaconDetailsList.Beacons)
        {
            uuidsList.Add(beacon.UUID.ToUpper() + ":Pit01");
        }

        return uuidsList.ToArray();
    }



    public string[] GetAllNormalizedUUIDs()
    {
        List<string> uuidsList = new List<string>();

        foreach (var beacon in beaconDetailsList.Beacons)
        {
            uuidsList.Add(beacon.UUID.ToLower());
        }

        return uuidsList.ToArray();
    }



    private IEnumerator DownloadVideos()
    {

        foreach (var beacon in beaconDetailsList.Beacons)
        {
            for (int i = 0; i < beacon.VideoURLs.Count; i++)
            {
                string videoID = ExtractUniqueID(beacon.VideoURLs[i]);
                string videoPath = Path.Combine(Application.persistentDataPath, $"{videoID}.mp4");
                validFiles.Add(videoID + ".mp4");                

                if (!File.Exists(videoPath))
                {
                    string videoUrl = beacon.VideoURLs[i];
                    using (UnityWebRequest request = UnityWebRequest.Get(videoUrl))
                    {
                        request.downloadHandler = new DownloadHandlerFile(videoPath);
                        yield return request.SendWebRequest();

                        if (request.result == UnityWebRequest.Result.Success)
                        {
                            Debug.Log($"Downloaded video {i + 1} for UUID {beacon.UUID}");
                        }
                        else
                        {
                            Debug.LogError($"Failed to download video {i + 1} for UUID {beacon.UUID}: {request.error}");
                        }
                    }
                }

                filesDownloaded++;
                UpdateProgressBar(filesDownloaded, totalFiles);
            }
        }


        // Delete orphaned files
        var existingFiles = Directory.GetFiles(Application.persistentDataPath);
        foreach (var file in existingFiles)
        {
            if (file.Contains(".mp4") && !validFiles.Contains(Path.GetFileName(file)))
            {
                File.Delete(file);
                Debug.Log($"Deleted orphaned file: {file}");
            }
        }
    }

    public void PlayVideo(string uuid, int videoIndex, VideoPlayer videoPlayer)
    {
        if (videoPlayer.isPlaying)
        {
            //videoPlayer.Pause();

            // Make play icon button visible
            //videoPlayer.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            string videoID = "";

            foreach (var beacon in beaconDetailsList.Beacons)
            {
                if (beacon.UUID == uuid)
                    videoID = ExtractUniqueID(beacon.VideoURLs[videoIndex]);
            }

                    
            string videoPath = Path.Combine(Application.persistentDataPath, $"{videoID}.mp4");

            if (File.Exists(videoPath))
            {
                if (string.IsNullOrEmpty(videoPlayer.url))
                    videoPlayer.url = videoPath; // Set the video path to the VideoPlayer's URL                

                //videoPlayer.Play();
                Debug.Log($"Playing video {videoIndex + 1} for UUID {uuid}");

                // Make play icon button hidden
                //videoPlayer.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError($"Video {videoIndex + 1} for UUID {uuid} not found locally.");
            }
        }
        
    }


    // Removes previous gallery images, videos and resets the srvice icons when you press the back button in details panel
    public void ClearGalleryAndVideoContent()
    {
        scrollViewGallery = landmarkDetails.GetNamedChild("Scroll View Gallery");

        galleryScrollViewContent = scrollViewGallery.GetNamedChild("GalleryContent");

        foreach (Transform item in galleryScrollViewContent.transform)
        {
            Destroy(item.gameObject);
        }


        scrollViewVideos = landmarkDetails.GetNamedChild("Scroll View Videos");

        videosScrollViewContent = scrollViewVideos.GetNamedChild("VideosContent");

        foreach (Transform item in videosScrollViewContent.transform)
        {
            Destroy(item.gameObject);
        }

        // Set all service icons to true to fix bug
        foreach (Transform child in servicesList)
            child.gameObject.SetActive(true);
    }
}
