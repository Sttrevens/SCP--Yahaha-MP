using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Antlr3.Runtime;

public class ElevatorLobbyController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup blackScreenCanvas;

    [Header("Scene References")]
    [SerializeField] private GameObject createRoomScene;
    [SerializeField] private GameObject joinRoomScene;
    [SerializeField] private GameObject producerListScene;

    [Header("Elevator Elements")]
    [SerializeField] private Transform elevatorTransform; // 电梯 Transform
    [SerializeField] private Animation DoorsAnim;
    [HideInInspector][SerializeField] private bool isOpen = false;

    [Header("Elevator Shake Settings")]
    [SerializeField] private AnimationCurve shakeCurve; // 控制震动幅度
    [SerializeField] private float shakeDuration = 0.5f; // 震动持续时间
    [SerializeField] private float shakeMagnitude = 0.1f; // 震动幅度

    private GameObject currentScene;

    void Start()
    {
        // Ensure all external scenes are disabled initially
        createRoomScene.SetActive(false);
        joinRoomScene.SetActive(false);
        producerListScene.SetActive(false);
        blackScreenCanvas.alpha = 0;
        blackScreenCanvas.gameObject.SetActive(false);
    }

    public void OnButtonSelect(string action)
    {
        StartCoroutine(HandleSceneChange(action));
    }

    private IEnumerator HandleSceneChange(string action)
    {
        // Trigger elevator shaking effect
        yield return StartCoroutine(ShakeElevatorPerlin());


        // Disable current scene
        if (currentScene != null)
        {
            currentScene.SetActive(false);
        }

        // Switch to the selected scene
        switch (action)
        {
            case "CreateRoom":
                currentScene = createRoomScene;
                break;

            case "JoinRoom":
                currentScene = joinRoomScene;
                break;

            case "ProducerList":
                currentScene = producerListScene;
                break;

            case "ExitGame":
                Application.Quit();
                yield break;
        }

        if (currentScene != null)
        {
            currentScene.SetActive(true);
        }

        OpenElevator();
        
    }

    public void OpenElevator()
    {
        StartCoroutine(OpenElevatorDoors());
    }

    public IEnumerator OpenElevatorDoors()
    {
        if (isOpen != true)
        {
            // Wait for door animation to play
            yield return new WaitForSeconds(1f);
            DoorsAnim[DoorsAnim.clip.name].normalizedTime = 0;
            DoorsAnim[DoorsAnim.clip.name].speed = 1;
            DoorsAnim.Play();
            isOpen = true;

            FindFirstObjectByType<TitleScreenUI>().HandleButtonClick("Initializing...");

            if (DoorsAnim[DoorsAnim.clip.name].speed > 0)
            {
                Invoke("DoorsClosing", 5f);
            }
        }
    }

    void DoorsClosing()
    {
        if (isOpen)
        {
            DoorsAnim[DoorsAnim.clip.name].normalizedTime = 1;
            DoorsAnim[DoorsAnim.clip.name].speed = -1;
            DoorsAnim.Play();

            isOpen = false;

            FindFirstObjectByType<TitleScreenUI>().ResetUI();
        }
        }

        private IEnumerator ShakeElevatorCurve()
    {
        Vector3 originalPosition = elevatorTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = shakeCurve.Evaluate(elapsed / shakeDuration) * shakeMagnitude;
            elevatorTransform.localPosition = originalPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        // Restore original position
        elevatorTransform.localPosition = originalPosition;
    }

    private IEnumerator ShakeElevatorPerlin()
    {
        Vector3 originalPosition = elevatorTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = (1f - (elapsed / shakeDuration)) * shakeMagnitude; // 随时间衰减
            float xOffset = Mathf.PerlinNoise(Time.time * 50f, 0f) * 2f - 1f; // 横向随机
            float yOffset = Mathf.PerlinNoise(0f, Time.time * 50f) * 2f - 1f; // 纵向随机
            elevatorTransform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0f) * strength;
            yield return null;
        }

        elevatorTransform.localPosition = originalPosition; // 复位
    }


    private IEnumerator FadeToBlack()
    {
        float duration = 1f; // Fade duration in seconds
        float elapsed = 0f;

        blackScreenCanvas.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            blackScreenCanvas.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        blackScreenCanvas.alpha = 1;
    }

    private IEnumerator FadeFromBlack()
    {
        float duration = 1f; // Fade duration in seconds
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            blackScreenCanvas.alpha = 1 - Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        blackScreenCanvas.alpha = 0;
        blackScreenCanvas.gameObject.SetActive(false);
    }
}
