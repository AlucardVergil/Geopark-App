using UnityEngine;

namespace Easy_QRCode.AutoSwitchLayout
{
	// This script is attached to a GameObject to automatically switch UI layout based on aspect ratio
	[ExecuteInEditMode]
	public class AutoSwitchLayout : MonoBehaviour
	{
		// Reference to the UI layout for portrait mode
		public Transform portraitModeLayoutTransform;
		
		// Reference to the UI layout for landscape mode
		public Transform landscapeModeLayoutTransform;

		// Stores the aspect ratio from the previous frame for comparison
		float m_PreviousAspectRatio;

		// The Update method is called every frame
		private void Update()
		{
			// Calculate the current aspect ratio of the screen
			var aspectRatio = 1f * Screen.width / Screen.height;

			// Check if the aspect ratio has changed since the last frame and if both layouts are assigned
			if (!Mathf.Approximately(aspectRatio, m_PreviousAspectRatio) 
				&& portraitModeLayoutTransform
				&& landscapeModeLayoutTransform)
			{
				// Update the stored aspect ratio to the current one
				m_PreviousAspectRatio = aspectRatio;

				// If aspect ratio is greater than 1, it's landscape mode
				if (aspectRatio > 1f)
				{
					// Enable landscape layout and disable portrait layout
					landscapeModeLayoutTransform.gameObject.SetActive(true);
					portraitModeLayoutTransform.gameObject.SetActive(false);
				}
				else // If aspect ratio is less than or equal to 1, it's portrait mode
				{
					// Enable portrait layout and disable landscape layout
					portraitModeLayoutTransform.gameObject.SetActive(true);
					landscapeModeLayoutTransform.gameObject.SetActive(false);
				}
			}
		}
	}
}
