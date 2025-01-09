using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartEffect : MonoBehaviour
{
    [SerializeField] private CanvasGroup blackScreenCanvas;

    // Start is called before the first frame update
    void Start()
    {
        blackScreenCanvas.alpha = 1;
        blackScreenCanvas.gameObject.SetActive(true);

        //StartCoroutine(FadeFromBlack());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public IEnumerator FadeFromBlack()
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
