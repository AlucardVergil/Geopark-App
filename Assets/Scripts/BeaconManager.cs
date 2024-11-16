using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    private int filesDownloaded; // Track downloaded files

    void Start()
    {
        landmarkDetails.SetActive(false);
        loadingScreen.SetActive(true);
        progressBar.value = 0f;

        localFilePath = Path.Combine(Application.persistentDataPath, "BeaconData.json");

        // Start downloading the JSON file; if it fails, load the local copy
        StartCoroutine(TryDownloadOrLoadLocalJSON());
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

            totalFiles += beaconDetailsList.Beacons.Count;

            filesDownloaded++;
            UpdateProgressBar(filesDownloaded, totalFiles);

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

                totalFiles += beaconDetailsList.Beacons.Count;

                filesDownloaded++;
                UpdateProgressBar(filesDownloaded, totalFiles);

                yield return StartCoroutine(DownloadImages());
                yield return StartCoroutine(DownloadVideos());
            }
            else
            {
                Debug.LogError("No local JSON file found. The app needs an internet connection to download the JSON initially.");
            }
        }

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

        Debug.Log("Beacon data loaded successfully.");
    }


    private IEnumerator DownloadImages()
    {
        int filesDownloaded = 0;
        int totalFiles = beaconDetailsList.Beacons.Count + beaconDetailsList.Beacons.Sum(b => b.GalleryImages.Count);

        foreach (var beacon in beaconDetailsList.Beacons)
        {
            // Download the main image
            string mainImagePath = Path.Combine(Application.persistentDataPath, $"{beacon.UUID}_main.png");
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
                string galleryImagePath = Path.Combine(Application.persistentDataPath, $"{beacon.UUID}_gallery_{i}.png");
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

        // Hide loading screen when all files are downloaded
        loadingScreen.SetActive(false);
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

                // Load the main image
                string mainImagePath = Path.Combine(Application.persistentDataPath, $"{beacon.UUID}_main.png");
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
                    string galleryImagePath = Path.Combine(Application.persistentDataPath, $"{beacon.UUID}_gallery_{i}.png");
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



    private IEnumerator DownloadVideos()
    {
        foreach (var beacon in beaconDetailsList.Beacons)
        {
            for (int i = 0; i < beacon.VideoURLs.Count; i++)
            {
                string videoPath = Path.Combine(Application.persistentDataPath, $"{beacon.UUID}_video_{i}.mp4");

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
                UpdateProgressBar(filesDownloaded, beaconDetailsList.Beacons.Count);
            }
        }
    }

    public void PlayVideo(string uuid, int videoIndex, VideoPlayer videoPlayer)
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            string videoPath = Path.Combine(Application.persistentDataPath, $"{uuid}_video_{videoIndex}.mp4");

            if (File.Exists(videoPath))
            {
                if (string.IsNullOrEmpty(videoPlayer.url))
                    videoPlayer.url = videoPath; // Set the video path to the VideoPlayer's URL                

                videoPlayer.Play();
                Debug.Log($"Playing video {videoIndex + 1} for UUID {uuid}");
            }
            else
            {
                Debug.LogError($"Video {videoIndex + 1} for UUID {uuid} not found locally.");
            }
        }
        
    }
}
