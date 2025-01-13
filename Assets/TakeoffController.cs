using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Fusion;
using System.Globalization;

public class TakeoffController : NetworkBehaviour, IInteractable
{
    public ScreenFade screenFade;
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    public BoolEvent OnBoolChanged;
    public bool IsFlying = false;
    [SerializeField] private Transform ShipTransform;
    [SerializeField] private bool IsStarted = false;
    
    [Header("Rotation")]
    [SerializeField] private float rotationAngle = 30f;
    [SerializeField] private float rotationSpeed = 100f;
    private Quaternion initialRotation;
    private bool isRotating = false;
    
    [Header("Elevator Shake Settings")] 
    [SerializeField] private AnimationCurve shakeCurve;
    [SerializeField] private float shakeDuration = 2.0f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip elevatorOpenSound;
    [SerializeField] private AudioClip elevatorCloseSound;
    [SerializeField] private AudioClip elevatorShakeSound;
    [SerializeField] private AudioSource audioSource;
    
    private const float AUDIO_FADE_DURATION = 0.5f;
    private const float PERLIN_NOISE_SPEED = 50f;
    private const float PERLIN_NOISE_SCALE = 2f;

    private void Awake()
    {
        ShipTransform = transform.parent;
        screenFade = GetComponent<ScreenFade>();
    }

    private void Update()
    {
        // Reserved for future use
    }

    public string GetInteractText() => IsStarted ? "No Way Back" : "Start Game";

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_OnInteract()
    {
        IsFlying = true;
        StartCoroutine(ShakeElevatorPerlinWithSound());
        TriggerScreenFade();
        OnBoolChanged.Invoke(IsFlying);
    }

    public void OnInteract()
    {
        Rpc_OnInteract();
    }

    private IEnumerator ShakeElevatorPerlinWithSound()
    {
        Vector3 originalPosition = ShipTransform.localPosition;
        float elapsed = 0f;

        StartCoroutine(FadeInSound());

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = Mathf.Lerp(shakeMagnitude, 0f, elapsed / shakeDuration);
            float xOffset = (Mathf.PerlinNoise(Time.time * PERLIN_NOISE_SPEED, 0f) * PERLIN_NOISE_SCALE - 1f);
            float yOffset = (Mathf.PerlinNoise(0f, Time.time * PERLIN_NOISE_SPEED) * PERLIN_NOISE_SCALE - 1f);
            
            ShipTransform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0f) * strength;
            yield return null;
        }

        StartCoroutine(FadeOutSound());
        ShipTransform.localPosition = originalPosition;
    }

    private IEnumerator FadeInSound()
    {
        return FadeSound(0f, 1f, true);
    }

    private IEnumerator FadeOutSound()
    {
        return FadeSound(audioSource.volume, 0f, false);
    }

    private IEnumerator FadeSound(float startVolume, float targetVolume, bool playOnStart)
    {
        float elapsed = 0f;

        if (playOnStart)
        {
            audioSource.volume = startVolume;
            audioSource.Play();
        }

        while (elapsed < AUDIO_FADE_DURATION)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / AUDIO_FADE_DURATION);
            yield return null;
        }

        audioSource.volume = targetVolume;
        
        if (!playOnStart)
        {
            audioSource.Stop();
        }
    }

    private void TriggerScreenFade()
    {
        screenFade?.StartCoroutine("FadeScreenAndShowSubtitle");
    }
}
