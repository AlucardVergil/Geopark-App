using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEditor;
using TMPro;
using static BluetoothLEHardwareInterface;
using System.Linq;

public class BeaconScanner : MonoBehaviour
{
	public GameObject iBeaconItemPrefab;

	private float _timeout = 0f;
	private float _startScanTimeout = 10f;
	private float _startScanDelay = 0.5f;
	private bool _startScan = true;
	private Dictionary<string, BeaconScannerItem> _iBeaconItems;

	private string[] iBeaconUUIDs; // = { "E2C56DB5-DFFB-48D2-B060-D0F5A71096E0:Pit01" };

	private BeaconManager _beaconManager;

    private Dictionary<string, float> _undetectedTimers; // Track undetected timers for each beacon
    private float undetectedDelay = 1f; // Delay in seconds before destroying undetected beacons

    private HashSet<string> detectedUUIDs = new HashSet<string>(); // Tracks currently detected UUIDs

    bool isBluetoothEnabled = false;
    bool isGPSEnabled = false;



    // Use this for initialization
    void Start ()
	{
        // Invoke every one sec until bluetooth and gps are opened
        InvokeRepeating(nameof(BLEScannerInitialize), 0f, 1f); // Start immediately, repeat every 1 second
    }




    public void BLEScannerInitialize()
    {
        isBluetoothEnabled = PermissionAndServiceChecker.IsBluetoothEnabled();
        isGPSEnabled = PermissionAndServiceChecker.IsGPSEnabled();

        if (isBluetoothEnabled && isGPSEnabled)
        {
            _beaconManager = GameObject.FindGameObjectWithTag("BLEManager").GetComponent<BeaconManager>();

            _iBeaconItems = new Dictionary<string, BeaconScannerItem>();

            _undetectedTimers = new Dictionary<string, float>();

            BluetoothLEHardwareInterface.Initialize(true, false, () =>
            {
                _timeout = _startScanDelay;

                BluetoothLEHardwareInterface.BluetoothScanMode(BluetoothLEHardwareInterface.ScanMode.LowLatency);
                BluetoothLEHardwareInterface.BluetoothConnectionPriority(BluetoothLEHardwareInterface.ConnectionPriority.High);
            },
            (error) =>
            {

                BluetoothLEHardwareInterface.Log("Error: " + error);

                if (error.Contains("Bluetooth LE Not Enabled"))
                    BluetoothLEHardwareInterface.BluetoothEnable(true);
            }, true);   // for beacon scanning we need to ask for location services on Android.
                        // IMPORTANT: REMOVE android:usesPermissionFlags="neverForLocation" AND android:maxSdkVersion="30"
                        //            from AndroidManifest.xml file      

            CancelInvoke(nameof(BLEScannerInitialize));
        }
    }




    public float Distance (float signalPower, float rssi, float nValue)
	{
		return (float)Math.Pow (10, ((signalPower - rssi) / (10 * nValue)));
	}



    string FormatUUID(string uuid)
    {
        if (uuid.Length != 32)
        {
            Debug.LogError("Invalid UUID length.");
            return uuid;
        }

        // Insert dashes at specified positions
        uuid = uuid.Insert(8, "-");
        uuid = uuid.Insert(13, "-");
        uuid = uuid.Insert(18, "-");
        uuid = uuid.Insert(23, "-");

        return uuid;
    }


#if !UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        iBeaconUUIDs = _beaconManager.GetAllUUIDs();

        if (iBeaconUUIDs.Length == 1) return;

        if (_timeout > 0f)
        {
            _timeout -= Time.deltaTime;
            if (_timeout <= 0f)
            {
                if (_startScan)
                {
                    _startScan = false;
                    _timeout = _startScanTimeout;

#if UNITY_EDITOR

                    // Reset undetected timer if the beacon is detected
                    if (_undetectedTimers.ContainsKey("e2c56db5-dffb-48d2-b060-d0f5a71096e0"))
                        _undetectedTimers.Remove("e2c56db5-dffb-48d2-b060-d0f5a71096e0");

                    if (!_iBeaconItems.ContainsKey("e2c56db5-dffb-48d2-b060-d0f5a71096e0"))
                    {
                        var newItem = Instantiate(iBeaconItemPrefab);
                        if (newItem != null)
                        {

                            newItem.transform.SetParent(transform);
                            newItem.transform.localScale = new Vector3(1f, 1f, 1f);

                            var iBeaconItem = newItem.GetComponent<BeaconScannerItem>();
                            if (iBeaconItem != null)
                            {
                                _iBeaconItems["e2c56db5-dffb-48d2-b060-d0f5a71096e0"] = iBeaconItem;
                                iBeaconItem.UUID = "e2c56db5-dffb-48d2-b060-d0f5a71096e0";
                            }
                        }
                    }

                    if (_iBeaconItems.ContainsKey("e2c56db5-dffb-48d2-b060-d0f5a71096e0"))
                    {
                        var iBeaconItem = _iBeaconItems["e2c56db5-dffb-48d2-b060-d0f5a71096e0"];
                        iBeaconItem.TextTitleFromUUID.text = (!_beaconManager.isEnglish ? _beaconManager.GetBeaconDetails("e2c56db5-dffb-48d2-b060-d0f5a71096e0".ToLower()).Title : _beaconManager.GetBeaconDetails("e2c56db5-dffb-48d2-b060-d0f5a71096e0".ToLower()).TitleEnglish);
                        iBeaconItem.TextRSSIValue.text = "rssi";

                        // Android returns the signal power or measured power, iOS hides this and there is no way to get it
                        iBeaconItem.TextAndroidSignalPower.text = "AndroidSignalPower";

                        // iOS returns an enum of unknown, far, near, immediate, Android does not return this
                        iBeaconItem.TextiOSProximity.text = "iOSProximity";


                        iBeaconItem.GetComponent<Image>().sprite = _beaconManager.GetBeaconDetails("e2c56db5-dffb-48d2-b060-d0f5a71096e0").ImageSprite;
                    }
#endif

                    // scanning for iBeacon devices requires that you know the Proximity UUID and provide an Identifier
                    BluetoothLEHardwareInterface.ScanForBeacons(iBeaconUUIDs, (iBeaconData) =>
                    {
                        string UUID = FormatUUID(iBeaconData.UUID.ToLower());

                        // Update the detected UUIDs
                        detectedUUIDs.Add(UUID);

                        // Reset undetected timer if the beacon is detected
                        if (_undetectedTimers.ContainsKey(UUID))
                            _undetectedTimers.Remove(UUID);

                        // Handle new beacon
                        if (!_iBeaconItems.ContainsKey(UUID))
                        {
                            BluetoothLEHardwareInterface.Log("item new: " + iBeaconData.UUID);
                            var newItem = Instantiate(iBeaconItemPrefab);
                            if (newItem != null)
                            {
                                BluetoothLEHardwareInterface.Log("item created: " + iBeaconData.UUID);
                                newItem.transform.SetParent(transform);
                                newItem.transform.localScale = Vector3.one;

                                var iBeaconItem = newItem.GetComponent<BeaconScannerItem>();
                                if (iBeaconItem != null)
                                {
                                    _iBeaconItems[UUID] = iBeaconItem;
                                    iBeaconItem.UUID = UUID;
                                }
                            }
                        }

                        // Update existing beacon data
                        if (_iBeaconItems.ContainsKey(UUID))
                        {
                            var iBeaconItem = _iBeaconItems[UUID];
                            if (iBeaconItem.TextTitleFromUUID.text == "")
                                iBeaconItem.TextTitleFromUUID.text = (!_beaconManager.isEnglish ? _beaconManager.GetBeaconDetails(UUID).Title : _beaconManager.GetBeaconDetails(UUID).TitleEnglish);

                            iBeaconItem.TextRSSIValue.text = iBeaconData.RSSI.ToString();

                            // Android returns the signal power or measured power, iOS hides this and there is no way to get it
                            iBeaconItem.TextAndroidSignalPower.text = iBeaconData.AndroidSignalPower.ToString();

                            // iOS returns an enum of unknown, far, near, immediate, Android does not return this
                            iBeaconItem.TextiOSProximity.text = iBeaconData.iOSProximity.ToString();

                            // we can only calculate a distance if we have the signal power which iOS does not provide
                            if (iBeaconData.AndroidSignalPower != 0)
                                iBeaconItem.TextDistance.text = Distance(iBeaconData.AndroidSignalPower, iBeaconData.RSSI, 2.5f).ToString();

                            if (iBeaconItem.GetComponent<Image>().sprite == null)
                                iBeaconItem.GetComponent<Image>().sprite = _beaconManager.GetBeaconDetails(UUID).ImageSprite;
                        }
                    });

#if !UNITY_EDITOR
                    // Handle undetected beacons with delay
                    HandleUndetectedBeacons();
#endif
                }
                else
                {
                    // Stop scanning and process missing UUIDs
                    BluetoothLEHardwareInterface.StopScan();

                    // Compare detected UUIDs with existing items and clean up missing ones
                    var itemsToDestroy = _iBeaconItems.Keys.Except(detectedUUIDs).ToList();
                    foreach (var uuid in itemsToDestroy)
                    {
                        Destroy(_iBeaconItems[uuid].gameObject);
                        _iBeaconItems.Remove(uuid);
                    }

                    // Reset for the next scan
                    detectedUUIDs.Clear();
                    _startScan = true;
                    _timeout = _startScanDelay;
                }
            }
        }
    }

#endif

    void HandleUndetectedBeacons()
    {
        // Update timers for undetected beacons
        foreach (var uuid in _iBeaconItems.Keys.ToList())
        {
            if (!detectedUUIDs.Contains(uuid))
            { 
                if (!_undetectedTimers.ContainsKey(uuid))
                    _undetectedTimers[uuid] = undetectedDelay;

                _undetectedTimers[uuid] -= Time.deltaTime;

                if (_undetectedTimers[uuid] <= 0f)
                {
                    Destroy(_iBeaconItems[uuid].gameObject);
                    _iBeaconItems.Remove(uuid);
                    _undetectedTimers.Remove(uuid);
                }
            }
        }

        // Clean up any expired timers for removed beacons
        foreach (var uuid in _undetectedTimers.Keys.ToList())
        {
            if (!_iBeaconItems.ContainsKey(uuid))
                _undetectedTimers.Remove(uuid);
        }
    }
}
