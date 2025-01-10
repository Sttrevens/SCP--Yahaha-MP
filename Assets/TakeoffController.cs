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
    [SerializeField] private bool IsStarted = false; // 默认状态
    [Header("Rotation")]
    [SerializeField] private float rotationAngle = 30f; // 旋转角度
    [SerializeField] private float rotationSpeed = 100f; // 旋转速度
    private Quaternion initialRotation; // 初始旋转
    private bool isRotating = false;
    [Header("Elevator Shake Settings")]
    [SerializeField] private AnimationCurve shakeCurve; // 控制震动幅度
    public float shakeDuration = 2.0f; // 震动持续时间
    [SerializeField] private float shakeMagnitude = 0.1f; // 震动幅度
    [Header("Audio Clips")]
    [SerializeField] private AudioClip elevatorOpenSound; // 电梯开门音效
    [SerializeField] private AudioClip elevatorCloseSound; // 电梯关门音效
    [SerializeField] private AudioClip elevatorShakeSound; // 电梯震动音效
    [SerializeField] private AudioSource audioSource; // 用于播放音效的音频源组件

    void Awake()
    {
        ShipTransform = transform.parent;
        screenFade = GetComponent<ScreenFade>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GetInteractText()
    {
        return string.Format("{0}", IsStarted ? "No Way Back" : "Start Game");
    }

    // RPC: 当任何客户端触发互动时，执行以下逻辑
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_OnInteract()
    {
        // 触发飞行状态
        IsFlying = true;

        // 开始震动并播放音效
        StartCoroutine(ShakeElevatorPerlinWithSound());

        // 触发屏幕淡出
        TriggerScreenFade();

        // 调用 OnBoolChanged 事件（可以用于其他逻辑，比如更新UI）
        OnBoolChanged.Invoke(IsFlying);
    }

    // 客户端调用 OnInteract 时，会触发 RPC 同步方法
    public void OnInteract()
    {

            Rpc_OnInteract(); // 通过 RPC 调用同步方法
    }

    private IEnumerator ShakeElevatorPerlinWithSound()
    {
        Vector3 originalPosition = ShipTransform.localPosition;
        float elapsed = 0f;

        // 淡入音效
        StartCoroutine(FadeInSound());

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = (1f - (elapsed / shakeDuration)) * shakeMagnitude; // 随时间衰减
            float xOffset = Mathf.PerlinNoise(Time.time * 50f, 0f) * 2f - 1f; // 横向随机
            float yOffset = Mathf.PerlinNoise(0f, Time.time * 50f) * 2f - 1f; // 纵向随机
            ShipTransform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0f) * strength;
            yield return null;
        }

        // 淡出音效
        StartCoroutine(FadeOutSound());

        ShipTransform.localPosition = originalPosition; // 复位
    }

    private IEnumerator FadeInSound()
    {
        float duration = 0.5f; // 淡入时长，可根据实际调整
        float elapsed = 0f;
        float startVolume = 0f;
        float targetVolume = 1f;

        audioSource.volume = startVolume;
        audioSource.Play();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOutSound()
    {
        float duration = 0.5f; // 淡出时长，可根据实际调整
        float elapsed = 0f;
        float startVolume = audioSource.volume;
        float targetVolume = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
        audioSource.Stop();
    }

    private void TriggerScreenFade()
    {
        if (screenFade != null)
        {
            screenFade.StartCoroutine("FadeScreenAndShowSubtitle");
        }
    }
}
