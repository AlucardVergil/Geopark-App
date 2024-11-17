using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FavoritesManager : MonoBehaviour
{
    private string favoritesPath;

    public List<string> favoriteUUIDs = new List<string>();

    void Awake()
    {
        // Define a path to save the favorites list
        favoritesPath = Path.Combine(Application.persistentDataPath, "favoritesList.json");
        LoadFavorites();
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
}
