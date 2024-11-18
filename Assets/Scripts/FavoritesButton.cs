using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FavoritesButton : MonoBehaviour
{
    private FavoritesManager _favoritesManager;

    [HideInInspector] public string UUID;

    private bool addedToFavorites = false;


    // Start is called before the first frame update
    void Start()
    {
        _favoritesManager = GameObject.FindGameObjectWithTag("BLEManager").GetComponent<FavoritesManager>();


        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (addedToFavorites)
            {
                addedToFavorites = false;

                GetComponent<Image>().color = Color.white;

                _favoritesManager.RemoveFromFavorites(UUID);
            }
            else
            {
                addedToFavorites = true;

                GetComponent<Image>().color = Color.red;

                _favoritesManager.AddToFavorites(UUID);
            }
        });
    }


    void OnEnable()
    {
        if (_favoritesManager != null)
        {            
            StartCoroutine(ToggleFavoritesButton());
        }

    }


    // Make favorites button red if it was already added to favorites 
    IEnumerator ToggleFavoritesButton()
    {
        yield return new WaitForEndOfFrame();

        if (_favoritesManager.favoriteUUIDs.Contains(UUID))
        {
            addedToFavorites = true;
            GetComponent<Image>().color = Color.red;
        }
        else
        {
            addedToFavorites = false;
            GetComponent<Image>().color = Color.white;
        }
    }


}
