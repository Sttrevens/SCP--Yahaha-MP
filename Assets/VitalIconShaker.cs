// Start of Selection
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VitalIconShaker : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image vitalImage;

    [Header("Shake Settings")]
    [Tooltip("Threshold below which the icon starts shaking (0.3 = 30%).")]
    [SerializeField] private float shakeThreshold = 0.3f;
    [Tooltip("Maximum possible shake intensity at 0 fill.")]
    [SerializeField] private float maxShakeIntensity = 5f;

    private Vector3 originalPosition;

    void Start()
    {
        if (vitalImage == null)
        {
            vitalImage = GetComponent<Image>();
        }
        // Store the initial position of the object for shake offset.
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (vitalImage == null) return;

        float fillValue = vitalImage.fillAmount;

        // When fill is below the threshold, start shaking. The lower the fill, the stronger the shake.
        if (fillValue < shakeThreshold)
        {
            float shakeFactor = (shakeThreshold - fillValue) / shakeThreshold; 
            float currentShakeIntensity = shakeFactor * maxShakeIntensity;

            Vector3 randomShakeOffset = Random.insideUnitSphere * currentShakeIntensity;
            // Shake in local position
            transform.localPosition = originalPosition + randomShakeOffset;
        }
        else
        {
            // Reset position if above threshold
            transform.localPosition = originalPosition;
        }
    }
}
