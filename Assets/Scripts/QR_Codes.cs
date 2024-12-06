using Easy_QRCode.Scanner;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: I added some code for QR in QRCode_Scanner.cs and BluetoothHardwareInterface.cs (the bluetooth file is where i added camera permissions)
public class QR_Codes : MonoBehaviour
{
    public QRCode_Scanner scanner;
    private LandmarkMapPoints landmarkPoints;

    private bool doOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        landmarkPoints = GetComponent<LandmarkMapPoints>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scanner.QRCode_Found)
        {
            landmarkPoints.UUID = scanner.SavedQRCodeScannedText;

            landmarkPoints.DisplayLandmarkDetails();

            // Disable QR Codes component if it exists after DisplayLandmarkDetails is executed. This is only for the LandmarkMapPoints instance that is attached to
            //BLEManager object and it's in order to stop the update method from executing for no reason when QR scanner is disabled
            enabled = false;
        }
    }
}
