using UnityEngine;
using UnityEngine.UI;

public class PulsatingGlowEffect : MonoBehaviour
{
    public Image glowImage; // Reference to the glow image
    public float pulseSpeed = 2f; // Speed of the pulsation
    public float minAlpha = 0.4f; // Minimum glow intensity
    public float maxAlpha = 0.8f; // Maximum glow intensity

    public float scaleAmount = 0.2f; // Amount to scale up and down
    private Vector3 originalScale;

    private Color originalColor;

    private void Start()
    {
        if (glowImage == null)
        {
            Debug.LogError("Glow Image not assigned!");
            return;
        }

        // Store the original glow color
        originalColor = glowImage.color;

        // Save the original scale of the object
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (glowImage != null)
        {
            // Calculate the pulsation using a sine wave
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2);

            // Update the glow image's color with the new alpha
            glowImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            // Calculate a pulsating scale factor using a sine wave
            float scaleFactor = 1 + Mathf.Sin(Time.time * pulseSpeed) * scaleAmount;

            // Apply the scale factor to the object
            transform.localScale = originalScale * scaleFactor;
        }
    }
}
