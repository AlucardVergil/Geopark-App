using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

namespace Easy_QRCode.Scanner
{
	public class QRCode_Scanner : MonoBehaviour
	{
		//Stores the last scanned QR code text
		public string SavedQRCodeScannedText = "";
		[SerializeField] private Text[] QRCodeDisplayedText;

		//Determines whether to hide the scanner once a QR code is found
		public bool HideWhenQRCodeIsFound = false;

		//Enum to select best performance or best camera quality
		public enum DeviceCamera
		{
			Default_BestPerformances_LowResolution, //It will use the lowest camera resolution for the best performances
			BestMatchToScreenSizeRatio, //It will use the closest camera resolution to fit the screen size
			BestQuality_WorstPerformances //It will use the highest camera resolution but the worst performances
		}
		public DeviceCamera deviceCamera;

		//Private variables for internal processing and state management
		private bool IE_Start_Running = false;
		private BarcodeReader barcodeReader;
		private bool QRCode_Found = false;
		[SerializeField] private RawImage CameraFeed;
		private WebCamTexture VideoStream;
		[SerializeField] private GameObject GO_Reticle;

		//Variables for handling aspect ratio and rotation of the video feed
		private float smallestAspectRatioDiffPortrait = float.MaxValue;
		private float smallestAspectRatioDiffLandscape = float.MaxValue;
		private float aspectRatio = 1.0f;
		private float rotationZ = 0f;

		//Flag to request video stream display update
		private bool Please_UpdateVideoStreamDisplay = true;

		//Variables to save RectTransform properties for comparison and updates
		private Vector3 Saved_Raw_RT_LocalScale = Vector3.zero;
		private Vector2 Saved_Raw_RT_SizeDelta = Vector2.zero;
		private Vector3 Saved_Raw_RT_LocalEulerAngles = Vector3.zero;
		private Vector2 Saved_Raw_RT_AnchoredPosition = Vector2.zero;
		private float Saved_Raw_RT_offsetMinX = 0.0f;
		private float Saved_Raw_RT_offsetMaxX = 0.0f;
		private float Saved_Raw_RT_offsetMaxY = 0.0f;
		private float Saved_Raw_RT_offsetMinY = 0.0f;
		private Vector2 Saved_Raw_RT_Size = Vector2.zero;
		private float Saved_Raw_RT_Width = 0.0f;
		private float Saved_Raw_RT_Height = 0.0f;

		private Vector3 Saved_Parent_RT_LocalScale = Vector3.zero;
		private Vector2 Saved_Parent_RT_SizeDelta = Vector2.zero;
		private Vector3 Saved_Parent_RT_LocalEulerAngles = Vector3.zero;
		private Vector2 Saved_Parent_RT_AnchoredPosition = Vector2.zero;
		private float Saved_Parent_RT_offsetMinX = 0.0f;
		private float Saved_Parent_RT_offsetMaxX = 0.0f;
		private float Saved_Parent_RT_offsetMaxY = 0.0f;
		private float Saved_Parent_RT_offsetMinY = 0.0f;
		private Vector2 Saved_Parent_RT_Size = Vector2.zero;
		private float Saved_Parent_RT_Width = 0.0f;
		private float Saved_Parent_RT_Height = 0.0f;

		private float Saved_VideoStream_RotationAngle = 0.0f;
		private float Saved_VideoStream_Width = 0.0f;
		private float Saved_VideoStream_Height = 0.0f;

		private int Saved_targetFrameRate = -1;

		void OnEnable()
		{
			//Saving the current target frame rate and setting it to 60
			Saved_targetFrameRate = Application.targetFrameRate;
			Application.targetFrameRate = 60;

			//Resetting the QR code found flag and starting the camera initialization coroutine
			QRCode_Found = false;
			StartCoroutine(IE_Start());
		}

		void OnDisable()
		{
			//Restoring the saved target frame rate
			Application.targetFrameRate = Saved_targetFrameRate;
			//We stop the video stream and hide the reticle
			if(VideoStream != null && VideoStream.isPlaying)
			{
				VideoStream.Stop();
			}
			GO_Reticle.SetActive(false);
		}

		//Coroutine to start the QR code scanning process
		IEnumerator IE_Start()
		{
			//Ensuring that the coroutine doesn't run multiple times simultaneously
			if(!IE_Start_Running)
			{
				IE_Start_Running = true;

				Please_UpdateVideoStreamDisplay = true;

				//Requesting user permission to access the webcam
				yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

				//If permission is granted, set up the video stream and start the QR code scanning coroutine
				if(Application.HasUserAuthorization(UserAuthorization.WebCam))
				{
					Debug.Log("WebCam authorization granted, initializing camera");
					barcodeReader = new BarcodeReader();
					SetUpVideoStream();
					GO_Reticle.SetActive(true);
					yield return new WaitForSeconds(1f);
					StartCoroutine(IE_Find_QRCode());
				}
				else
				{
					Debug.LogError("WebCam authorization denied");
				}

				IE_Start_Running = false;
			}
		}

		void OnDestroy()
		{
			//We stop the video stream if it is playing
			if(VideoStream != null && VideoStream.isPlaying)
			{
				VideoStream.Stop();
			}
		}

		//Coroutine to find a QR code in the video stream
		IEnumerator IE_Find_QRCode()
		{
			//Continuously searching for a QR code until one is found
			while(!QRCode_Found)
			{
				//Check if the video stream is playing and if user has webcam authorization
				if (VideoStream != null && VideoStream.isPlaying && !QRCode_Found && Application.HasUserAuthorization(UserAuthorization.WebCam))
				{
					string QRCode_Text = "";
					try
					{
						//Decoding the QR code from the video stream
						var result = barcodeReader.Decode(VideoStream.GetPixels32(), VideoStream.width, VideoStream.height);
						QRCode_Text = result.Text;
					}
					catch
					{
						//If decoding fails, reset the QR code text
						QRCode_Text = "";
					}

					//If a QR code is decoded, log the result, set the flag, and save the text
					if(QRCode_Text.Length > 0)
					{
						Debug.Log("Decoded QR Code: " + QRCode_Text + "");
						QRCode_Found = true;
						SavedQRCodeScannedText = QRCode_Text;
						for(int cpt = 0; cpt < QRCodeDisplayedText.Length; cpt++)
						{
							QRCodeDisplayedText[cpt].text = SavedQRCodeScannedText;
						}

						//If the setting is to hide when a QR code is found, stop the video stream and deactivate the gameObject
						if(HideWhenQRCodeIsFound)
						{
							VideoStream.Stop();

							yield return null;

							gameObject.SetActive(false);
						}
						else //If we want to continuously search for a new QR code, we wait for a second and then reset the QR code found flag
						{
							yield return new WaitForSeconds(1f);

							QRCode_Found = false;
						}
					}
				}

				//Yield until the next frame
				yield return null;
			}
		}

		void FixedUpdate()
		{
			//Check if the video stream display needs updating
			if(!Please_UpdateVideoStreamDisplay)
			{
				//A series of checks for any changes in video stream properties or related UI elements.
				//If any change is detected, set the flag to update video stream display.
				//Each check involves comparing the current property value with a saved value.
				//If a difference is found, the current value is saved, and the flag is set.
				//These checks include rotation angle, video width and height, RectTransform properties, etc.
				//The purpose is to ensure the video stream display is updated when necessary.

				if(VideoStream.videoRotationAngle != Saved_VideoStream_RotationAngle)
				{
					//Debug.Log("Change detected in VideoStream.videoRotationAngle");
					Saved_VideoStream_RotationAngle = VideoStream.videoRotationAngle;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(VideoStream.width != Saved_VideoStream_Width)
				{
					//Debug.Log("Change detected in VideoStream.width");
					Saved_VideoStream_Width = VideoStream.width;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(VideoStream.height != Saved_VideoStream_Height)
				{
					//Debug.Log("Change detected in VideoStream.height");
					Saved_VideoStream_Height = VideoStream.height;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.gameObject.GetComponent<RectTransform>().localScale != Saved_Raw_RT_LocalScale)
				{
					//Debug.Log("Change detected in CameraFeed.gameObject.GetComponent<RectTransform>().localScale");
					Saved_Raw_RT_LocalScale = CameraFeed.gameObject.GetComponent<RectTransform>().localScale;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.gameObject.GetComponent<RectTransform>().sizeDelta != Saved_Raw_RT_SizeDelta)
				{
					//Debug.Log("Change detected in CameraFeed.gameObject.GetComponent<RectTransform>().sizeDelta");
					Saved_Raw_RT_SizeDelta = CameraFeed.gameObject.GetComponent<RectTransform>().sizeDelta;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.gameObject.GetComponent<RectTransform>().localEulerAngles != Saved_Raw_RT_LocalEulerAngles)
				{
					//Debug.Log("Change detected in CameraFeed.gameObject.GetComponent<RectTransform>().localEulerAngles");
					Saved_Raw_RT_LocalEulerAngles = CameraFeed.gameObject.GetComponent<RectTransform>().localEulerAngles;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.gameObject.GetComponent<RectTransform>().anchoredPosition != Saved_Raw_RT_AnchoredPosition)
				{
					//Debug.Log("Change detected in CameraFeed.gameObject.GetComponent<RectTransform>().anchoredPosition");
					Saved_Raw_RT_AnchoredPosition = CameraFeed.gameObject.GetComponent<RectTransform>().anchoredPosition;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.rectTransform.offsetMin.x != Saved_Raw_RT_offsetMinX)
				{
					//Debug.Log("Change detected in CameraFeed.rectTransform.offsetMin.x");
					Saved_Raw_RT_offsetMinX = CameraFeed.rectTransform.offsetMin.x;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.rectTransform.offsetMax.x != Saved_Raw_RT_offsetMaxX)
				{
					//Debug.Log("Change detected in CameraFeed.rectTransform.offsetMax.x");
					Saved_Raw_RT_offsetMaxX = CameraFeed.rectTransform.offsetMax.x;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.rectTransform.offsetMax.y != Saved_Raw_RT_offsetMaxY)
				{
					//Debug.Log("Change detected in CameraFeed.rectTransform.offsetMax.y");
					Saved_Raw_RT_offsetMaxY = CameraFeed.rectTransform.offsetMax.y;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.rectTransform.offsetMin.y != Saved_Raw_RT_offsetMinY)
				{
					//Debug.Log("Change detected in CameraFeed.rectTransform.offsetMin.y");
					Saved_Raw_RT_offsetMinY = CameraFeed.rectTransform.offsetMin.y;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.rectTransform.rect.size != Saved_Raw_RT_Size)
				{
					//Debug.Log("Change detected in CameraFeed.rectTransform.rect.size");
					Saved_Raw_RT_Size = CameraFeed.rectTransform.rect.size;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.rectTransform.rect.width != Saved_Raw_RT_Width)
				{
					//Debug.Log("Change detected in CameraFeed.rectTransform.rect.width");
					Saved_Raw_RT_Width = CameraFeed.rectTransform.rect.width;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.rectTransform.rect.height != Saved_Raw_RT_Height)
				{
					//Debug.Log("Change detected in CameraFeed.rectTransform.rect.height");
					Saved_Raw_RT_Height = CameraFeed.rectTransform.rect.height;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().localScale != Saved_Parent_RT_LocalScale)
				{
					//Debug.Log("Change detected in CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().localScale");
					Saved_Parent_RT_LocalScale = CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().localScale;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta != Saved_Parent_RT_SizeDelta)
				{
					//Debug.Log("Change detected in CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta");
					Saved_Parent_RT_SizeDelta = CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().localEulerAngles != Saved_Parent_RT_LocalEulerAngles)
				{
					//Debug.Log("Change detected in CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().localEulerAngles");
					Saved_Parent_RT_LocalEulerAngles = CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().localEulerAngles;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition != Saved_Parent_RT_AnchoredPosition)
				{
					//Debug.Log("Change detected in CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition");
					Saved_Parent_RT_AnchoredPosition = CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMin.x != Saved_Parent_RT_offsetMinX)
				{
					//Debug.Log("Change detected in CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMin.x");
					Saved_Parent_RT_offsetMinX = CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMin.x;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMax.x != Saved_Parent_RT_offsetMaxX)
				{
					//Debug.Log("Change detected in CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMax.x");
					Saved_Parent_RT_offsetMaxX = CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMax.x;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMax.y != Saved_Parent_RT_offsetMaxY)
				{
					//Debug.Log("Change detected in CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMax.y");
					Saved_Parent_RT_offsetMaxY = CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMax.y;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMin.y != Saved_Parent_RT_offsetMinY)
				{
					//Debug.Log("Change detected in CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMin.y");
					Saved_Parent_RT_offsetMinY = CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().offsetMin.y;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().rect.size != Saved_Parent_RT_Size)
				{
					//Debug.Log("Change detected in CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().rect.size");
					Saved_Parent_RT_Size = CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().rect.size;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().rect.width != Saved_Parent_RT_Width)
				{
					//Debug.Log("Change detected in CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().rect.width");
					Saved_Parent_RT_Width = CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().rect.width;
					Please_UpdateVideoStreamDisplay = true;
				}
				else if(CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().rect.height != Saved_Parent_RT_Height)
				{
					//Debug.Log("Change detected in CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().rect.height");
					Saved_Parent_RT_Height = CameraFeed.transform.parent.gameObject.GetComponent<RectTransform>().rect.height;
					Please_UpdateVideoStreamDisplay = true;
				}
			}
			else
			{
				//If the video stream display needs to be updated and the video stream is available and playing
				if(VideoStream != null && VideoStream.isPlaying && Application.HasUserAuthorization(UserAuthorization.WebCam))
				{
					//Update the video stream display
					UpdateVideoStreamDisplay();
				}
			}
		}

		private void UpdateVideoStreamDisplay()
		{
			//Calculating the aspect ratio of the video stream
			aspectRatio = (float)VideoStream.width / (float)VideoStream.height;
			//Getting the orientation of the video stream and adjusting rotation accordingly
			int orientation = VideoStream.videoRotationAngle;
			orientation = orientation * 3;
			rotationZ = orientation;

			//Platform-specific scale adjustments
			#if UNITY_IOS
				//For iOS, invert the Y-axis scale (to correct for coordinate system differences)
				CameraFeed.rectTransform.localScale = new Vector3(1f, -1f, 1f);
			#else
				//For other platforms, use normal scaling
				CameraFeed.rectTransform.localScale = new Vector3(1f, 1f, 1f);
			#endif

			//Setting the native size of the camera feed
			CameraFeed.SetNativeSize();

			//Get the parent RectTransform to adjust the layout accordingly
			RectTransform parentRectTransform = CameraFeed.transform.parent.GetComponent<RectTransform>();
			//Get the width and height of the parent RectTransform
			float parentWidth = parentRectTransform.rect.width;
			float parentHeight = parentRectTransform.rect.height;

			//Determine if the video stream is rotated more than 45 degrees but less than 135 degrees
			bool isRotated = Mathf.Abs(rotationZ % 180) > 45 && Mathf.Abs(rotationZ % 180) < 135;

			//If the video is rotated, swap the width and height
			if (isRotated)
			{
				float temp = parentWidth;
				parentWidth = parentHeight;
				parentHeight = temp;
			}

			//Calculate the width and height of the video stream, maintaining the aspect ratio
			float width = parentWidth;
			float height = parentWidth / aspectRatio;
			//Adjust the dimensions if the calculated height is greater than the parent height
			if (height > parentHeight)
			{
				height = parentHeight;
				width = parentHeight * aspectRatio;
			}

			//Set the size and rotation of the CameraFeed's RectTransform
			CameraFeed.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
			CameraFeed.gameObject.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, rotationZ);
			//Center the CameraFeed within the parent RectTransform
			float canvasRect_width = parentRectTransform.rect.width;
			float canvasRect_height = parentRectTransform.rect.height;
			CameraFeed.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(canvasRect_width/2, canvasRect_height/2);

			//We indicate that the video stream display no longer needs an update
			Please_UpdateVideoStreamDisplay = false;
		}

		private void SetUpVideoStream()
		{
			WebCamDevice cameraDevice = new WebCamDevice();
			bool cameraDeviceFound = false;
			//We get all connected webcam devices
			WebCamDevice[] devices = WebCamTexture.devices;

			//If no webcam devices are found
			if (devices.Length == 0)
			{
			    Debug.LogError("No webcam/camera found");
			    return;
			}

			//Loop to find a back/rear facing camera
			for (int i = 0; i < devices.Length; i++)
			{
			    if (!devices[i].isFrontFacing)
			    {
			        cameraDevice = devices[i];
			        cameraDeviceFound = true;
			        break;
			    }
			}

			//If no back/rear facing camera found, we look for a front-facing one
			if(!cameraDeviceFound)
			{
			    for (int i = 0; i < devices.Length; i++)
			    {
			        if (devices[i].isFrontFacing)
			        {
			            cameraDevice = devices[i];
			            cameraDeviceFound = true;
			            break;
			        }
			    }
			}

			//If no camera is found, log an error and return
			if(!cameraDeviceFound)
			{
			    Debug.LogError("No webcam/camera found");
			    return;
			}

			Debug.Log("Current screen resolution : " + Screen.width + "x" + Screen.height + "");

			//Get the available resolutions for the selected camera device
			Resolution[] resolutions = cameraDevice.availableResolutions;
			//Check if there are available resolutions
			if (resolutions != null && resolutions.Length > 0)
			{
			    //We want to select the best match/resolution for the video stream
			    float screenAspectRatio = (float)Screen.width / Screen.height;
			    Resolution bestResolution = resolutions[0];
			    smallestAspectRatioDiffPortrait = float.MaxValue;
			    smallestAspectRatioDiffLandscape = float.MaxValue;
			    float screenPixelNb = Screen.width * Screen.height;
			    float smallestPixelNbDifference = float.MaxValue;

			    switch (deviceCamera)
			    {
			        case DeviceCamera.BestMatchToScreenSizeRatio:
			            foreach (Resolution res in resolutions)
			            {
			                float aspectRatioPortrait = (float)res.height / res.width;;
			                float aspectRatioLandscape = (float)res.width / res.height;
			                float aspectRatioDiffPortrait = Mathf.Abs(screenAspectRatio - aspectRatioPortrait);
			                float aspectRatioDiffLandscape = Mathf.Abs(screenAspectRatio - aspectRatioLandscape);
			                float cameraPixelNb = res.width * res.height;
			                float pixelNbAbsDiff = Mathf.Abs(screenPixelNb - cameraPixelNb);
			                
			                if (aspectRatioDiffPortrait <= smallestAspectRatioDiffPortrait)
			                {
			                    if(pixelNbAbsDiff < smallestPixelNbDifference)
			                    {
			                        smallestPixelNbDifference = pixelNbAbsDiff;
			                        smallestAspectRatioDiffPortrait = aspectRatioDiffPortrait;
			                        bestResolution = res;
			                    }
			                }

			                if (aspectRatioDiffLandscape <= smallestAspectRatioDiffLandscape)
			                {
			                    if(pixelNbAbsDiff < smallestPixelNbDifference)
			                    {
			                        smallestPixelNbDifference = pixelNbAbsDiff;
			                        smallestAspectRatioDiffLandscape = aspectRatioDiffLandscape;
			                        bestResolution = res;
			                    }
			                }
			            }

			            if(smallestAspectRatioDiffPortrait < smallestAspectRatioDiffLandscape)
			            {
			                Debug.Log("Best portrait resolution found: " + bestResolution.height + "x" + bestResolution.width + "");
			            }
			            else
			            {
			                Debug.Log("Best landscape resolution found: " + bestResolution.width + "x" + bestResolution.height + "");
			            }
			            break;

			        case DeviceCamera.BestQuality_WorstPerformances:
			            float saved_cameraPixelNb = 0.0f;
			            foreach (Resolution res in resolutions)
			            {
			                float cameraPixelNb = res.width * res.height;
			                
			                if (saved_cameraPixelNb < cameraPixelNb)
			                {
			                    saved_cameraPixelNb = cameraPixelNb;
			                    bestResolution = res;
			                }
			            }

			            Debug.Log("Best resolution found: " + bestResolution.width + "x" + bestResolution.height + "");
			            break;

			        case DeviceCamera.Default_BestPerformances_LowResolution:
			            foreach (Resolution res in resolutions)
			            {
			                float aspectRatioPortrait = (float)res.height / res.width;
			                float aspectRatioLandscape = (float)res.width / res.height;
			                float aspectRatioDiffPortrait = Mathf.Abs(screenAspectRatio - aspectRatioPortrait);
			                float aspectRatioDiffLandscape = Mathf.Abs(screenAspectRatio - aspectRatioLandscape);
			                float cameraPixelNb = res.width * res.height;

			                if (aspectRatioDiffPortrait <= smallestAspectRatioDiffPortrait)
			                {
			                    if (cameraPixelNb < smallestPixelNbDifference)
			                    {
			                        smallestPixelNbDifference = cameraPixelNb;
			                        smallestAspectRatioDiffPortrait = aspectRatioDiffPortrait;
			                        bestResolution = res;
			                    }
			                }

			                if (aspectRatioDiffLandscape <= smallestAspectRatioDiffLandscape)
			                {
			                    if (cameraPixelNb < smallestPixelNbDifference)
			                    {
			                        smallestPixelNbDifference = cameraPixelNb;
			                        smallestAspectRatioDiffLandscape = aspectRatioDiffLandscape;
			                        bestResolution = res;
			                    }
			                }
			            }

			            if (smallestAspectRatioDiffPortrait < smallestAspectRatioDiffLandscape)
			            {
			                Debug.Log("Lowest portrait resolution found: " + bestResolution.height + "x" + bestResolution.width + "");
			            }
			            else
			            {
			                Debug.Log("Lowest landscape resolution found: " + bestResolution.width + "x" + bestResolution.height + "");
			            }
			            break;
			    }

			    VideoStream = new WebCamTexture(cameraDevice.name, bestResolution.width, bestResolution.height);
			    VideoStream.Play();
			    CameraFeed.texture = VideoStream;
			}
			else //If no resolutions are available, initialize the video stream with default settings
			{
			    Debug.Log("Initializing video stream with the default settings");
			    VideoStream = new WebCamTexture(cameraDevice.name);
			    VideoStream.Play();
			    CameraFeed.texture = VideoStream;
			}
		}
	}
}
