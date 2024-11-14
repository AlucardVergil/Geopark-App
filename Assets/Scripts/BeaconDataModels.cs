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
    public string ImagePath;
    public string AdditionalInfo;
}

[Serializable]
public class BeaconDetailsList
{
    public List<BeaconDetails> Beacons;
}
