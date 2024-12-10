using System;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class PermissionAndServiceChecker : MonoBehaviour
{
    public GameObject warningPanel;

    bool isBluetoothEnabled = false;
    bool isGPSEnabled = false;
    bool doOnce = true;

    public BeaconScanner scanner;


    void Start()
    {
        warningPanel.SetActive(false);
#if !UNITY_EDITOR
        // Check and request permissions
        CheckPermissions();

        // Check Bluetooth and GPS status
        CheckBluetoothAndGPS();
#endif
    }

#if !UNITY_EDITOR
    // Check if Bluetooth and GPS are enabled in order to initialize the scanner
    private void Update()
    {
        if (doOnce)
        {
            isBluetoothEnabled = IsBluetoothEnabled();
            isGPSEnabled = IsGPSEnabled();

            if (isBluetoothEnabled && isGPSEnabled)
            {
                scanner.BLEScannerInitialize();
                doOnce = false;
            }
        }

        
    }
#endif




    void CheckPermissions()
    {
        // Request Location permission
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        // Request Bluetooth permissions (Android 12+)
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
        {
            Permission.RequestUserPermission("android.permission.BLUETOOTH_CONNECT");
        }
#endif
    }




    void CheckBluetoothAndGPS()
    {

#if UNITY_ANDROID
        bool isBluetoothEnabled = IsBluetoothEnabled();
        bool isGPSEnabled = IsGPSEnabled();

        if (!isBluetoothEnabled || !isGPSEnabled)
        {
            warningPanel.SetActive(true);
            //OpenBluetoothSettings();
        }
#endif


    }




    public static bool IsBluetoothEnabled()
    {
        bool isEnabled = false;

        try
        {
            using (AndroidJavaClass bluetoothAdapterClass = new AndroidJavaClass("android.bluetooth.BluetoothAdapter"))
            {
                AndroidJavaObject bluetoothAdapter = bluetoothAdapterClass.CallStatic<AndroidJavaObject>("getDefaultAdapter");
                if (bluetoothAdapter != null)
                {
                    isEnabled = bluetoothAdapter.Call<bool>("isEnabled");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error checking Bluetooth status: " + ex.Message);
        }

        return isEnabled;
    }



    public static bool IsGPSEnabled()
    {
        bool isEnabled = false;

        try
        {
            using (AndroidJavaClass locationManagerClass = new AndroidJavaClass("android.location.LocationManager"))
            {
                using (AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                        .GetStatic<AndroidJavaObject>("currentActivity")
                        .Call<AndroidJavaObject>("getApplicationContext"))
                {
                    using (AndroidJavaObject locationManager = context.Call<AndroidJavaObject>("getSystemService", "location"))
                    {
                        isEnabled = locationManager.Call<bool>("isProviderEnabled", "gps");
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error checking GPS status: " + ex.Message);
        }

        return isEnabled;
    }



    public void OpenBluetoothSettings()
    {
        using (var intentClass = new AndroidJavaClass("android.content.Intent"))
        using (var settingsClass = new AndroidJavaClass("android.provider.Settings"))
        using (var activity = new AndroidJavaObject("com.unity3d.player.UnityPlayer"))
        {
            var intent = new AndroidJavaObject("android.content.Intent", settingsClass.GetStatic<string>("ACTION_BLUETOOTH_SETTINGS"));
            activity.GetStatic<AndroidJavaObject>("currentActivity").Call("startActivity", intent);
        }
    }

    public void OpenGPSSettings()
    {
        using (var intentClass = new AndroidJavaClass("android.content.Intent"))
        using (var settingsClass = new AndroidJavaClass("android.provider.Settings"))
        using (var activity = new AndroidJavaObject("com.unity3d.player.UnityPlayer"))
        {
            var intent = new AndroidJavaObject("android.content.Intent", settingsClass.GetStatic<string>("ACTION_LOCATION_SOURCE_SETTINGS"));
            activity.GetStatic<AndroidJavaObject>("currentActivity").Call("startActivity", intent);
        }
    }



    public void OpenSettings()
    {
        if (!isBluetoothEnabled)
            OpenBluetoothSettings();

        if (!isGPSEnabled)
            OpenGPSSettings();
    }

}
