using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UIElements;

public class PermissionAndServiceChecker : MonoBehaviour
{
    public GameObject warningPanel;

    bool isBluetoothEnabled = false;
    bool isGPSEnabled = false;

    public GameObject QRCodeScanner;



    void Start()
    {
        warningPanel.SetActive(false);
#if !UNITY_EDITOR
        // Check and request permissions
        CheckPermissions();

        // Check Bluetooth and GPS status
        //CheckBluetoothAndGPS();
#endif
    }




    void CheckPermissions()
    {
        // Request Location permission
        //if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        //{
        //    Permission.RequestUserPermission(Permission.FineLocation);
        //}

// Request Bluetooth permissions (Android 12+)
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
        {
            Permission.RequestUserPermission("android.permission.BLUETOOTH_CONNECT");
        }

        StartCoroutine(CheckPermissionsAndroid());
#endif

#if UNITY_IOS && !UNITY_EDITOR
        StartCoroutine(CheckPermissionsIOS());
#endif
    }



    IEnumerator CheckPermissionsAndroid()
    {
        bool scanAsked = false;
        bool locationAsked = false;
        bool connectAsked = false;        


        if (!Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN"))
        {
            if (!scanAsked)
            {
                Permission.RequestUserPermission("android.permission.BLUETOOTH_SCAN");
                scanAsked = true;
            }
        }


        yield return new WaitUntil(() => scanAsked);

        if (!Permission.HasUserAuthorizedPermission("android.permission.ACCESS_FINE_LOCATION"))
        {
            if (!locationAsked)
            {
                Permission.RequestUserPermission("android.permission.ACCESS_FINE_LOCATION");
                locationAsked = true;
            }
        }

        yield return new WaitUntil(() => locationAsked);


        if (!Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
        {
            if (!connectAsked)
            {
                Permission.RequestUserPermission("android.permission.BLUETOOTH_CONNECT");
                connectAsked = true;
            }
        }

        yield return new WaitUntil(() => connectAsked);        
    }



    IEnumerator CheckPermissionsIOS()
{
    // Check if location services are enabled by the user
    if (!Input.location.isEnabledByUser)
    {
        Debug.Log("Location services are not enabled by the user. Please enable them in settings.");
        yield break; // Exit the coroutine as we cannot proceed without user enabling location
    }
    else
    {
        // Start the location service
        Input.location.Start();

        // Wait for location service to initialize
        int maxWait = 20; // seconds
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Check the status of the location service
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Failed to start location service. Check device settings.");
        }
        else if (Input.location.status == LocationServiceStatus.Running)
        {
            Debug.Log("Location service started successfully.");
        }
        else
        {
            Debug.Log("Location service initialization timed out.");
        }
    }
}



    public void AskCameraPermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(AskCameraPermissionAndEnableScannerAndroid());
#endif

#if UNITY_IOS && !UNITY_EDITOR
        StartCoroutine(AskCameraPermissionAndEnableScannerIOS());
#endif


    }


    IEnumerator AskCameraPermissionAndEnableScannerAndroid()
    {
        bool cameraPermissionAsked = false;

        if (!Permission.HasUserAuthorizedPermission("android.permission.CAMERA"))
        {
            if (!cameraPermissionAsked)
            {
                Permission.RequestUserPermission("android.permission.CAMERA");
                cameraPermissionAsked = true;
            }

            yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission("android.permission.CAMERA"));            
        }

        yield return new WaitForSeconds(0.2f);

        QRCodeScanner.SetActive(true);
        GetComponent<QR_Codes>().enabled = true;
    }




    IEnumerator AskCameraPermissionAndEnableScannerIOS()
    {
        bool cameraAsked = false;

        // Check and request Camera Permission
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            if (!cameraAsked)
            {
                Application.RequestUserAuthorization(UserAuthorization.WebCam);
                cameraAsked = true;
            }
        }

        yield return new WaitUntil(() => Application.HasUserAuthorization(UserAuthorization.WebCam));

        yield return new WaitForSeconds(0.2f);

        QRCodeScanner.SetActive(true);
        GetComponent<QR_Codes>().enabled = true;
    }




    public void CheckBluetoothAndGPS()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        isBluetoothEnabled = IsBluetoothEnabled();
        isGPSEnabled = IsGPSEnabled();

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
        isBluetoothEnabled = IsBluetoothEnabled();
        isGPSEnabled = IsGPSEnabled();

        if (!isBluetoothEnabled)
            OpenBluetoothSettings();

        if (!isGPSEnabled)
            OpenGPSSettings();
    }

}
