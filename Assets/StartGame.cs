using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public CanvasGroup blackScreenCanvas;

    public GameObject levelScene;
    public GameObject lobbyScene;
    // Start is called before the first frame update
    void Start()
    {
        levelScene.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchtoScene()
    {
        StartCoroutine(FadetoBlack());
    }

    private IEnumerator FadetoBlack()
    {
        blackScreenCanvas.gameObject.SetActive(true);

        float duration = 3f; // Fade duration in seconds
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            blackScreenCanvas.alpha = 0 + Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        blackScreenCanvas.alpha = 1;

        lobbyScene.SetActive(false);
        levelScene.SetActive(true);

        StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeFromBlack()
    {
        float duration = 3f; // Fade duration in seconds
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
