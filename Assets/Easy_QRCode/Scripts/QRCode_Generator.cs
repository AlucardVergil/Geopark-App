using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System;

namespace Easy_QRCode.Generator
{
public class QRCode_Generator : MonoBehaviour
{
	//Text to be encoded in the QR Code
	public string QRCodeText = "Hello World !";
	//UI element to display the QR code
	[SerializeField] private RawImage rawQrCode;

	void Start()
	{
		//Generate QR code texture and assign it to the UI element
		rawQrCode.texture = GenerateQRCode(QRCodeText, 256, 256);
		//Adjust the size of the QR code image to fit its parent UI element
		FitWithParent(rawQrCode, 0f);
	}

	//Method to generate a QR code as a Texture2D
	private Texture2D GenerateQRCode(string textForEncoding, int width, int height)
	{
		var writer = new BarcodeWriter
		{
			//Set the format of the QR code
			Format = BarcodeFormat.QR_CODE,
			Options = new ZXing.Common.EncodingOptions
			{
				Width = width,
				Height = height,
				Margin = 0 //No margin for the QR code
			}
		};

		//Generate the QR code
		writer.Options.Hints.Add(ZXing.EncodeHintType.CHARACTER_SET, "UTF-8"); //2024-08-28 Support added for Cyrillic characters
		var color32 = writer.Write(textForEncoding);
		var encoded = new Texture2D(width, height);
		encoded.SetPixels32(color32);
		encoded.Apply();

		return encoded;
	}

	//Method to resize the QR code image to fit within its parent UI element
	private Vector2 FitWithParent(RawImage image, float padding = 0f)
	{
		float w = 0f, h = 0f;
		var parent = image.GetComponentInParent<RectTransform>();
		var imageTransform = image.GetComponent<RectTransform>();
 
		//Check if the image has a texture to display
		if(image.texture != null)
		{
			if(!parent)
			{
				//If there's no parent, return current size
				return imageTransform.sizeDelta;
			}
			padding = 1f - padding;
			//Calculate the aspect ratio of the image
			float ratio = image.texture.width / (float)image.texture.height;
			var bounds = new Rect(0f, 0f, parent.rect.width, parent.rect.height);
			//Check for image rotation and adjust bounds accordingly
			if(Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
			{
				bounds.size = new Vector2(bounds.height, bounds.width);
			}
			//Resize image based on height first
			h = bounds.height * padding;
			w = h * ratio;
			//If the width exceeds the parent's bounds, resize based on width instead
			if(w > bounds.width * padding)
			{
				w = bounds.width * padding;
				h = w / ratio;
			}
		}
		//Apply the calculated size to the image
		imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
		imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
		return imageTransform.sizeDelta;
	}
}
}
