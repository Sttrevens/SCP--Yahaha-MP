using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ElevatorLobbyController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup blackScreenCanvas;

    [Header("Scene References")]
    [SerializeField] private GameObject createRoomScene;
    [SerializeField] private GameObject joinRoomScene;
    [SerializeField] private GameObject producerListScene;

    [Header("Elevator Elements")]
    [SerializeField] private Animation DoorsAnim;

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
        // Fade to black
        yield return StartCoroutine(FadeToBlack());

        DoorsAnim [DoorsAnim.clip.name].normalizedTime = 0;
        DoorsAnim [DoorsAnim.clip.name].speed = 1;
        DoorsAnim.Play();

        // Wait for door animation to finish (assuming 1 second for demo)
        yield return new WaitForSeconds(1f);

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

        // Fade back in
        yield return StartCoroutine(FadeFromBlack());
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
