using LPSurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif

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

        float duration = 2f; // Fade duration in seconds
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

        for (int i = 0; i < Inventory.instance.slots.Length; i++)
        {
            Inventory.instance.slots[i].item = null;
        }

        Inventory.instance.UpdateUI();

        yield return new WaitForSeconds(2f);

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
