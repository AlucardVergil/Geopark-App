using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class PermissionAndServiceChecker : MonoBehaviour
{
    public GameObject warningPanel;
    public TMP_Text warningText;


    void Start()
    {
        warningPanel.SetActive(false);

        // Check and request permissions
        CheckPermissions();

        // Check Bluetooth and GPS status
        CheckBluetoothAndGPS();
    }

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
        // Check Bluetooth status
        using (var bluetoothAdapter = new AndroidJavaObject("android.bluetooth.BluetoothAdapter"))
        {
            if (bluetoothAdapter != null && !bluetoothAdapter.Call<bool>("isEnabled"))
            {
                Debug.Log("Bluetooth is disabled. Prompting user to enable it...");

                warningPanel.SetActive(true);
                warningText.text += "\n\nBluetooth is disabled. Prompting user to enable it...";

                OpenBluetoothSettings();
            }
        }

        // Check GPS status
        using (var settingsClass = new AndroidJavaClass("android.location.LocationManager"))
        using (var activity = new AndroidJavaObject("com.unity3d.player.UnityPlayer"))
        {
            var locationManager = activity.Call<AndroidJavaObject>("getSystemService", settingsClass.GetStatic<string>("LOCATION_SERVICE"));
            bool isGPSEnabled = locationManager.Call<bool>("isProviderEnabled", "gps");

            if (!isGPSEnabled)
            {
                Debug.Log("GPS is disabled. Prompting user to enable it...");

                warningPanel.SetActive(true);
                warningText.text += "\n\nGPS is disabled. Prompting user to enable it...";

                OpenGPSSettings();
            }
        }
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
}
