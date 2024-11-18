using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static BluetoothLEHardwareInterface;

public class FavoritesManager : MonoBehaviour
{
    private string favoritesPath;

    [HideInInspector] public List<string> favoriteUUIDs = new List<string>();

    private BeaconManager _beaconManager;
    private Dictionary<string, BeaconScannerItem> favoritesItems;

    public GameObject favoritesItemPrefab;
    public Transform favoritesPanel;


    void Awake()
    {
        // Define a path to save the favorites list
        favoritesPath = Path.Combine(Application.persistentDataPath, "favoritesList.json");
        LoadFavorites();
    }



    private void Start()
    {
        _beaconManager = GameObject.FindGameObjectWithTag("BLEManager").GetComponent<BeaconManager>();

        favoritesItems = new Dictionary<string, BeaconScannerItem>();
    }



    public void AddToFavorites(string uuid)
    {
        if (!favoriteUUIDs.Contains(uuid)) // Avoid duplicates
        {
            favoriteUUIDs.Add(uuid);
            SaveFavorites();
            Debug.Log($"UUID {uuid} added to favorites.");
        }
    }



    public void RemoveFromFavorites(string uuid)
    {
        if (favoriteUUIDs.Contains(uuid))
        {
            favoriteUUIDs.Remove(uuid);
            SaveFavorites();
            Debug.Log($"UUID {uuid} removed from favorites.");
        }
    }



    private void SaveFavorites()
    {
        File.WriteAllText(favoritesPath, JsonUtility.ToJson(new SerializableList<string>(favoriteUUIDs), true));
    }



    private void LoadFavorites()
    {
        if (File.Exists(favoritesPath))
        {
            string json = File.ReadAllText(favoritesPath);
            favoriteUUIDs = JsonUtility.FromJson<SerializableList<string>>(json).items;
        }
    }



    public void DisplayFavorites()
    {
        // Clear existing favorite items
        foreach (Transform child in favoritesPanel)
        {
            favoritesItems.Clear();
            Destroy(child.gameObject);
        }


        foreach (string UUID in favoriteUUIDs)
        {
            if (!favoritesItems.ContainsKey(UUID))
            {
                var newItem = Instantiate(favoritesItemPrefab);
                if (newItem != null)
                {
                    newItem.transform.SetParent(favoritesPanel);
                    newItem.transform.localScale = Vector3.one;

                    var favoriteItem = newItem.GetComponent<BeaconScannerItem>();
                    if (favoriteItem != null)
                    {
                        favoritesItems[UUID] = favoriteItem;
                        favoriteItem.UUID = UUID;
                    }
                }
            }

            // Update existing beacon data
            if (favoritesItems.ContainsKey(UUID))
            {
                var favoriteItem = favoritesItems[UUID];
                if (favoriteItem.TextTitleFromUUID.text == "")
                    favoriteItem.TextTitleFromUUID.text = _beaconManager.GetBeaconDetails(UUID).Title;

                if (favoriteItem.GetComponent<Image>().sprite == null)
                    favoriteItem.GetComponent<Image>().sprite = _beaconManager.GetBeaconDetails(UUID).ImageSprite;
            }
        }        
    }


    public void GoBack()
    {
        if (favoritesPanel.gameObject.activeSelf)
            DisplayFavorites();
    }

}
