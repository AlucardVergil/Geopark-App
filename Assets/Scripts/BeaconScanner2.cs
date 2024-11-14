using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BeaconScanner2 : MonoBehaviour
{
    public TMP_Text debugText;

    // Set your iBeacon UUID here. You can also use major/minor if needed.
    //private string[] iBeaconUUIDs = { "e2c56db5dffb48d2b060d0f5a71096e0", "e2c56db5-dffb-48d2-b060-d0f5a71096e0",
    //    "E2C56DB5-DFFB-48D2-B060-D0F5A71096E0", "E2C56DB5DFFB48D2B060D0F5A71096E0" };

    private string[] iBeaconUUIDs = { "e2c56db5-dffb-48d2-b060-d0f5a71096e0:Pit01" };

    void Start()
    {
        // Initialize Bluetooth Low Energy
        BluetoothLEHardwareInterface.Initialize(true, false, () => {
            Debug.Log("BLE Initialized");
            debugText.text = "BLE Initialized";
            StartScanningForBeacons();
        }, (error) => {
            Debug.LogError("BLE Initialization Error: " + error);
            debugText.text = "BLE Initialization Error: " + error;
        });
    }

    void StartScanningForBeacons()
    {
        // Scan for iBeacons with the given UUID (you can also add major/minor if needed)
        BluetoothLEHardwareInterface.ScanForBeacons(iBeaconUUIDs, (beaconData) => {
            Debug.Log("Found iBeacon: " + beaconData.UUID); // Use UUID property
            // Display the beacon's data (UUID, Major, Minor, etc.)
            debugText.text = "Found iBeacon: " + beaconData.UUID + "\n" +
                             "Major: " + beaconData.Major + "\n" +
                             "Minor: " + beaconData.Minor;
        });
    }

    void OnDestroy()
    {
        // Deinitialize Bluetooth when the script is destroyed
        BluetoothLEHardwareInterface.DeInitialize(() => {
            Debug.Log("BLE Deinitialized");
            debugText.text = "BLE Deinitialized";
        });
    }
}
