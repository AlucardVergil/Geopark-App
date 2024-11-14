using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class BeaconManager : MonoBehaviour
{
    [HideInInspector] public BeaconDetailsList beaconDetailsList; // Stores the list of beacon details
    private Dictionary<string, BeaconDetails> beaconDetailsDictionary; // Dictionary for quick lookup

    public GameObject landmarkDetails;


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
        }
        else
        {
            Debug.LogWarning("Failed to download JSON, loading local file if it exists.");

            // If download failed, load from the local file if available
            if (File.Exists(localFilePath))
            {
                string json = File.ReadAllText(localFilePath);
                LoadBeaconData(json);
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





    // Display the details of a specific beacon by UUID
    public BeaconDetails GetBeaconDetails(string uuid)
    {
        if (beaconDetailsDictionary.TryGetValue(uuid, out BeaconDetails details))
        {
            // Display details (replace this with actual UI code)
            Debug.Log("Info: " + details.Info);
            Debug.Log("Additional Info: " + details.AdditionalInfo);
            // Load and display image from details.ImagePath if needed
            return details;
        }
        else
        {
            Debug.LogWarning("Beacon with UUID " + uuid + " not found.");
            return null;
        }
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
