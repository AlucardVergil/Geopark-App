using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BeaconManager4 : MonoBehaviour
{
    [HideInInspector] public BeaconDetailsList beaconDetailsList; // Stores the list of beacon details
    private Dictionary<string, BeaconDetails> beaconDetailsDictionary; // Dictionary for quick lookup

    public GameObject landmarkDetails;


    private void Awake()
    {
        LoadBeaconDetails();
    }



    void Start()
    {
        landmarkDetails.SetActive(false);
    }

    private void LoadBeaconDetails()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "BeaconData.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            beaconDetailsList = JsonUtility.FromJson<BeaconDetailsList>(json);

            beaconDetailsDictionary = new Dictionary<string, BeaconDetails>();
            foreach (var beacon in beaconDetailsList.Beacons)
            {
                beaconDetailsDictionary[beacon.UUID] = beacon;
            }
        }
        else
        {
            Debug.LogError("Beacon data file not found.");
        }
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
