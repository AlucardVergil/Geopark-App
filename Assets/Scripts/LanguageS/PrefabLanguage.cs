using TMPro;
using UnityEngine;

public class PrefabLanguage : MonoBehaviour
{
    private BeaconManager _beaconManager;

    public TMP_Text titleText;

    public string greekTitle;
    public string englishTitle;

    private bool previousIsEnglish = false;

    // Start is called before the first frame update
    void Start()
    {
        _beaconManager = GameObject.FindGameObjectWithTag("BLEManager").GetComponent<BeaconManager>();

        InvokeRepeating(nameof(CheckIfLanguageChanged), 0f, 2f); // Start immediately, repeat every 2 second
    }


    void CheckIfLanguageChanged()
    {
        if (previousIsEnglish != _beaconManager.isEnglish)
        {
            previousIsEnglish = _beaconManager.isEnglish;

            titleText.text = (!_beaconManager.isEnglish ? greekTitle : englishTitle);
        }
        
    }
}
