using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShake : MonoBehaviour
{
   public float shakeMagnitude = 0.5f;
    public float distortionSpeed = 20f;
    public float shakeIncreaseRate = 0.1f;
    public float maxShakeMagnitude = 5f;
    public float shakeDuration = 5f;
    private bool isShaking = false;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private float shakeTimer = 0f;

    void Start()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (isShaking)
        {
            shakeTimer += Time.deltaTime;
            shakeMagnitude = Mathf.Min(shakeMagnitude + shakeIncreaseRate * Time.deltaTime, maxShakeMagnitude);

            transform.position = originalPosition + new Vector3(
                Mathf.PerlinNoise(Time.time * distortionSpeed, 0) * shakeMagnitude * 5f,
                Mathf.PerlinNoise(0, Time.time * distortionSpeed) * shakeMagnitude * 5f,
                Mathf.PerlinNoise(Time.time * distortionSpeed, Time.time * distortionSpeed) * shakeMagnitude * 5f
            );

            transform.rotation = originalRotation * Quaternion.Euler(
                Mathf.Sin(Time.time * distortionSpeed) * shakeMagnitude * 50f,
                Mathf.Cos(Time.time * distortionSpeed) * shakeMagnitude * 50f,
                Mathf.Sin(Time.time * distortionSpeed) * shakeMagnitude * 50f
            );

            transform.localScale = originalScale + new Vector3(
                Mathf.PerlinNoise(Time.time * distortionSpeed, 0) * shakeMagnitude * 5f,
                Mathf.PerlinNoise(0, Time.time * distortionSpeed) * shakeMagnitude * 5f,
                Mathf.PerlinNoise(Time.time * distortionSpeed, Time.time * distortionSpeed) * shakeMagnitude * 5f
            );
        }
    }

    public void StartShake()
    {
        isShaking = true;
        shakeTimer = 0f;
        shakeMagnitude = 0.5f;
    }

    public void StopShake()
    {
        isShaking = false;
        shakeTimer = 0f;
        shakeMagnitude = 0.5f;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
    }
}
