using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class BeaconManager1 : MonoBehaviour
{
    [HideInInspector] public BeaconDetailsList beaconDetailsList; // Stores the list of beacon details
    private Dictionary<string, BeaconDetails> beaconDetailsDictionary; // Dictionary for quick lookup

    public GameObject landmarkDetails;
    public TMP_Text temp;

    // Replace with your actual Google Drive file ID
    private string fileId = "1qTmB5Z3HHHe_ue2LRL66YgwntuK-s-w6";
    private string localFilePath;

    void Start()
    {
        landmarkDetails.SetActive(false);

        // Set up the path to save the file locally
        localFilePath = Path.Combine(Application.persistentDataPath, "BeaconData.json");

        // Try to download the JSON file; if it fails, load the local copy
        StartCoroutine(TryDownloadOrLoadLocalJSON());
    }

    private IEnumerator TryDownloadOrLoadLocalJSON()
    {
        string url = $"https://drive.google.com/uc?id={fileId}&export=download";

        // Start downloading the file
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            // Save downloaded JSON to local file for offline access
            File.WriteAllText(localFilePath, json);
            Debug.Log("Downloaded JSON and saved to: " + localFilePath);

            // Load data from the downloaded JSON
            LoadBeaconData(json);

            // Start downloading images
            StartCoroutine(DownloadImages());
        }
        else
        {
            Debug.LogWarning("Failed to download JSON, loading local file if it exists.");

            // If download failed, load from the local file if available
            if (File.Exists(localFilePath))
            {
                string json = File.ReadAllText(localFilePath);
                LoadBeaconData(json);

                // Start downloading images (if JSON file was loaded locally)
                StartCoroutine(DownloadImages());
            }
            else
            {
                Debug.LogError("No local JSON file found. The app needs an internet connection to download the JSON initially.");
            }
        }
    }

    private void LoadBeaconData(string json)
    {
        // Deserialize JSON data
        beaconDetailsList = JsonUtility.FromJson<BeaconDetailsList>(json);

        // Populate the dictionary for quick lookup
        beaconDetailsDictionary = new Dictionary<string, BeaconDetails>();
        foreach (var beacon in beaconDetailsList.Beacons)
        {
            beaconDetailsDictionary[beacon.UUID] = beacon;
        }

        Debug.Log("Beacon data loaded successfully.");
    }

    private IEnumerator DownloadImages()
    {
        foreach (var beacon in beaconDetailsList.Beacons)
        {
            string imagePath = Path.Combine(Application.persistentDataPath, $"{beacon.UUID}.png");

            // Only download the image if it doesn't already exist locally
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
        }
    }

    // Display the details of a specific beacon by UUID
    public BeaconDetails GetBeaconDetails(string uuid)
    {
        BeaconDetails details = new BeaconDetails();

        foreach (var beacon in beaconDetailsList.Beacons)
        {
            temp.text = "test " + beacon.UUID + "\n" + uuid;
            if (beacon.UUID == uuid)
            {
                details = beacon;

                // Load image if it exists locally
                string imagePath = Path.Combine(Application.persistentDataPath, $"{beacon.UUID}.png");
                if (File.Exists(imagePath))
                {
                    byte[] imageData = File.ReadAllBytes(imagePath);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);

                    // Display the texture in UI or any other part where you want to use it
                    // For example, set it as sprite for an Image component
                    // imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
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



    // Method to get all UUIDs from the JSON file and return them as a string array
    public string[] GetAllUUIDs()
    {
        List<string> uuidsList = new List<string>();

        foreach (var beacon in beaconDetailsList.Beacons)
        {
            uuidsList.Add(beacon.UUID.ToUpper() + ":Pit01");
        }

        return uuidsList.ToArray(); // Converts the List<string> to a string array
    }
}
