using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class iBeaconExampleScript : MonoBehaviour
{
	public GameObject iBeaconItemPrefab;

	private float _timeout = 0f;
	private float _startScanTimeout = 10f;
	private float _startScanDelay = 0.5f;
	private bool _startScan = true;
	private Dictionary<string, iBeaconItemScript> _iBeaconItems;


    string[] tempUUIDs = {"E2C56DB5-DFFB-48D2-B060-D0F5A71096E0:Pit01",
"A4A70900-24E1-11D6-9B20-000629DC0A52:Pit01",
"D8A01E22-D16E-11EB-B8BC-0242AC130003:Pit01",
"B5B182C7-EAB1-4988-AA99-B5C1517008D9:Pit01",
"C9407F30-9622-11E3-BF4B-0002A5D5C51B:Pit01",
"E3B0C442-98FC-1C14-9AFB-A21A23A8910A:Pit01",
"A564A6F7-79E3-435E-832F-17277FD6F6B1:Pit01",
"D0D3FA86-CA76-4417-8B52-BBD9F5ED8FDB:Pit01",
"C5A7D7C3-23C2-49F4-8F8B-F4F30861FB6B:Pit01",
"2F234454-CF6D-4A0F-ADF2-F4911BA9FFA6:Pit01",
"F3077ABE-0937-40A0-8F45-C6BDA3C4B4EF:Pit01",
"3852364B-1F6B-49F5-BB84-69EB06DA8BFE:Pit01",
"77E7D68E-798A-4FF5-BFCD-23C201D65D8B:Pit01",
"99A09892-9BA3-49C4-B540-B784682EF41D:Pit01",
"23A01AF0-232A-4518-9C0E-323FB773F5EF:Pit01",
"74278BDA-B644-4520-8F0C-720EAF059935:Pit01",
"F5C0F1C8-88E0-4D53-A164-6A4704B6DC25:Pit01",
"64E6E50D-FD56-4E3A-9C2E-94CBB3ACB12F:Pit01",
"4F5AC5EF-FDE4-4F3F-A3B3-FB55F2AA3C35:Pit01",
"B36D5A4C-CB57-487C-B4C8-B78E0C2F8740:Pit01",
"74A87FE8-5F74-41C8-9C3E-3F81C8E31A6F:Pit01",
"C34A8911-6D8C-4BDF-BF0B-184787946B7A:Pit01",
"569CD5A8-D2C3-44F3-8369-1847F9C431DA:Pit01",
"6B4863A1-BB0A-423E-B6A3-DF59D7A78F93:Pit01",
"5F330394-9F0E-4022-A4F4-38C847102EE3:Pit01",
"CDFB6145-928A-49B9-AE9C-0AC1E9AD7E83:Pit01",
"71E508C8-84C2-451D-8FB5-6C292B8E3A94:Pit01",
"E5A842DC-86D3-4353-8B4F-8415EB57CAFD:Pit01",
"392ED78C-13B9-4B2E-B34B-7B89D4DF918C:Pit01",
"B1CF7B09-2CD1-470D-9334-9E8D2FA5C27D:Pit01",
"F8CC6BB7-94B8-4652-8C5A-379B2C4E458E:Pit01",
"D15D35BC-5A39-4C80-930B-5428C2891C60:Pit01",
"C24E64AB-5211-48D1-B8CF-E9F7165D8BB2:Pit01",
"BFC5C7DD-DC56-496C-9174-10D8F84F9632:Pit01",
"8A4823B3-2493-4E24-BDDC-B74909D4A9A6:Pit01",
"90BCB5CE-7B24-4D98-B993-0E257AC6AFCD:Pit01",
"A6F8D1E2-65B4-4B5A-8ABF-D2BCE92A4CD3:Pit01",
"F41C437A-9DF8-4B82-B4DB-6E92D4C347DE:Pit01",
"73891F2B-F4AE-4861-B5C8-D9B273CC15AB:Pit01",
"6B40D63C-7DF7-45C6-8B37-9D72D93D74A7:Pit01",
"E2FC0E8C-7521-44F2-846E-F7F9A6B4C837:Pit01",
"85F1B3A9-4BDF-46A9-A3C7-7A4D9F5B1E62:Pit01",
"1ADF0B7A-2734-4B8C-9B3D-592D84E7D8C7:Pit01",
"B9E73C54-4737-4C2D-B9F7-2A9D3C7F4B5E:Pit01",
"738F2D47-37C9-4C5B-BD84-6B7A94E4C6F7:Pit01",
"A4B9D75F-9B2D-4E3C-8F7A-2D6C7A948F52:Pit01",
"53E2A4D9-47C7-4B9F-B5D7-A9F4C7E6B5A2:Pit01",
"92B7F4E5-43A9-4F3B-9D7C-84E2C5B7A9D3:Pit01",
"3F2D7A94-47C8-4D9B-B5F3-E7A2C4D9B6F7:Pit01",
"D7E4C9B3-8A5F-4B9A-BD7C-2A9F4E7C5B6A:Pit01"
};


    // Use this for initialization
    void Start ()
	{
		_iBeaconItems = new Dictionary<string, iBeaconItemScript> ();

		BluetoothLEHardwareInterface.Initialize (true, false, () => {

			_timeout = _startScanDelay;

			BluetoothLEHardwareInterface.BluetoothScanMode (BluetoothLEHardwareInterface.ScanMode.LowLatency);
			BluetoothLEHardwareInterface.BluetoothConnectionPriority (BluetoothLEHardwareInterface.ConnectionPriority.High);
		}, 
		(error) => {
			
			BluetoothLEHardwareInterface.Log ("Error: " + error);

			if (error.Contains ("Bluetooth LE Not Enabled"))
				BluetoothLEHardwareInterface.BluetoothEnable (true);
		}, true);   // for beacon scanning we need to ask for location services on Android.
                    // IMPORTANT: REMOVE android:usesPermissionFlags="neverForLocation" AND android:maxSdkVersion="30"
                    //            from AndroidManifest.xml file
    }

    public float Distance (float signalPower, float rssi, float nValue)
	{
		return (float)Math.Pow (10, ((signalPower - rssi) / (10 * nValue)));
	}

	// Update is called once per frame
	void Update ()
	{
		if (_timeout > 0f)
		{
			_timeout -= Time.deltaTime;
			if (_timeout <= 0f)
			{
				if (_startScan)
				{
					_startScan = false;
					_timeout = _startScanTimeout;

					// scanning for iBeacon devices requires that you know the Proximity UUID and provide an Identifier
					BluetoothLEHardwareInterface.ScanForBeacons (tempUUIDs, (iBeaconData) => {

						if (!_iBeaconItems.ContainsKey (iBeaconData.UUID))
						{
							BluetoothLEHardwareInterface.Log ("item new: " + iBeaconData.UUID);
							var newItem = Instantiate (iBeaconItemPrefab);
							if (newItem != null)
							{
								BluetoothLEHardwareInterface.Log ("item created: " + iBeaconData.UUID);
								newItem.transform.SetParent (transform);
								newItem.transform.localScale = new Vector3 (1f, 1f, 1f);

								var iBeaconItem = newItem.GetComponent<iBeaconItemScript> ();
								if (iBeaconItem != null)
									_iBeaconItems[iBeaconData.UUID] = iBeaconItem;
							}
						}

						if (_iBeaconItems.ContainsKey (iBeaconData.UUID))
						{
							var iBeaconItem = _iBeaconItems[iBeaconData.UUID];
							iBeaconItem.TextUUID.text = iBeaconData.UUID;
							iBeaconItem.TextRSSIValue.text = iBeaconData.RSSI.ToString ();

							// Android returns the signal power or measured power, iOS hides this and there is no way to get it
							iBeaconItem.TextAndroidSignalPower.text = iBeaconData.AndroidSignalPower.ToString ();

							// iOS returns an enum of unknown, far, near, immediate, Android does not return this
							iBeaconItem.TextiOSProximity.text = iBeaconData.iOSProximity.ToString ();

							// we can only calculate a distance if we have the signal power which iOS does not provide
							if (iBeaconData.AndroidSignalPower != 0)
								iBeaconItem.TextDistance.text = Distance (iBeaconData.AndroidSignalPower, iBeaconData.RSSI, 2.5f).ToString ();
						}
					});
				}
				else
				{
					BluetoothLEHardwareInterface.StopScan ();
					_startScan = true;
					_timeout = _startScanDelay;
				}
			}
		}
	}
}
