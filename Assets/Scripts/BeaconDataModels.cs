using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BeaconDetails
{
    public string UUID;
    public string Title;
    public string Info;
    public string ImageURL;
    public string AdditionalInfo;

    public Sprite ImageSprite;

    public List<string> GalleryImages; // List of image URLs for the gallery
    public List<Sprite> GallerySprites = new List<Sprite>(); // Sprites for gallery images

    public List<string> VideoURLs; // List of video URLs
}


[Serializable]
public class BeaconDetailsList
{
    public List<BeaconDetails> Beacons;
}
