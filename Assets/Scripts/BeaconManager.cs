using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

            yield return StartCoroutine(DownloadImages(totalFiles));
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

                yield return StartCoroutine(DownloadImages(totalFiles));
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

    private IEnumerator DownloadImages(int totalFiles)
    {
        foreach (var beacon in beaconDetailsList.Beacons)
        {
            string imagePath = Path.Combine(Application.persistentDataPath, $"{beacon.UUID}.png");

            if (!File.Exists(imagePath))
            {
                using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(beacon.ImageURL))
                {
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(request);
                        byte[] imageData = texture.EncodeToPNG();
                        File.WriteAllBytes(imagePath, imageData);
                        Debug.Log($"Downloaded and saved image for UUID {beacon.UUID}");
                    }
                    else
                    {
                        Debug.LogError($"Failed to download image for UUID {beacon.UUID}");
                    }
                }
            }

            filesDownloaded++;
            UpdateProgressBar(filesDownloaded, totalFiles);
        }
    }

    private void UpdateProgressBar(int completed, int total)
    {
        float progress = (float)completed / total;
        progressBar.value = progress;
    }

    public BeaconDetails GetBeaconDetails(string uuid)
    {
        BeaconDetails details = new BeaconDetails();

        foreach (var beacon in beaconDetailsList.Beacons)
        {
            if (beacon.UUID == uuid)
            {
                details = beacon;

                string imagePath = Path.Combine(Application.persistentDataPath, $"{beacon.UUID}.png");
                if (File.Exists(imagePath))
                {
                    byte[] imageData = File.ReadAllBytes(imagePath);
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
                    Debug.LogWarning($"Image for UUID {uuid} not found locally.");
                }

                break;
            }
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
}
