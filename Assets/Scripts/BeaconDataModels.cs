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
}

[Serializable]
public class BeaconDetailsList
{
    public List<BeaconDetails> Beacons;
}
