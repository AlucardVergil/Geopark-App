using UnityEngine;

public class PulsateEffect : MonoBehaviour
{
    public float pulsateSpeed = 2f; // Speed of pulsation
    public float scaleAmount = 0.2f; // Amount to scale up and down
    private Vector3 originalScale;

    void Start()
    {
        // Save the original scale of the object
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Calculate a pulsating scale factor using a sine wave
        float scaleFactor = 1 + Mathf.Sin(Time.time * pulsateSpeed) * scaleAmount;

        // Apply the scale factor to the object
        transform.localScale = originalScale * scaleFactor;
    }
}
