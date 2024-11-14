using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BeaconScanner3 : MonoBehaviour
{
    public TMP_Text debugText;

    // Set your iBeacon UUID here (for comparison).
    private string iBeaconUUID = "e2c56db5dffb48d2b060d0f5a71096e0";
    private List<string> detectedDevices = new List<string>();

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
        // Start scanning for peripherals
        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {
            if (!detectedDevices.Contains(address))
            {
                detectedDevices.Add(address);
                Debug.Log("Discovered device: " + name + " at address: " + address);
                debugText.text += "\nDiscovered device: " + name + "\nAddress: " + address;

                // Attempt to connect to the device
                BluetoothLEHardwareInterface.ConnectToPeripheral(address,
                    (addr) => {
                        Debug.Log("Connected to device: " + addr);
                        debugText.text += "\nConnected to device: " + addr;
                    },
                    (addr, serviceUUID) => {
                        Debug.Log("Discovered service: " + serviceUUID);
                    },
                    (addr, serviceUUID, characteristicUUID) => {
                        Debug.Log("Discovered characteristic: " + characteristicUUID);
                        debugText.text += "\nDiscovered characteristic: " + characteristicUUID;
                    },
                    (addr) => {
                        Debug.Log("Disconnected from device: " + addr);
                        debugText.text += "\nDisconnected from device: " + addr;
                    }
                );
            }
        },
        (address, name, rssi, advertisingData) => {
            // Parse advertising data here, since ConnectToPeripheral does not directly provide it.
            string uuid = ParseUUIDFromAdvertisingData(advertisingData);
            if (uuid == iBeaconUUID)
            {
                Debug.Log("iBeacon detected with UUID: " + uuid);
                debugText.text += "\niBeacon detected with UUID: " + uuid;
            }
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

    string ParseUUIDFromAdvertisingData(byte[] data)
    {
        // Implement parsing logic based on iBeacon data format
        if (data.Length >= 25)
        {
            byte[] uuidBytes = new byte[16];
            System.Array.Copy(data, 9, uuidBytes, 0, 16);
            return System.BitConverter.ToString(uuidBytes).Replace("-", "").ToLower();
        }
        return string.Empty;
    }
}
