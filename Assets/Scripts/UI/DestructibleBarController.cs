using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DestructibleBarController : MonoBehaviour
{
    public static DestructibleBarController Instance; // 单例模式

    public RectTransform healthBarFill; // 需要绑定的血条填充组件的 RectTransform
    public CanvasGroup healthBarCanvasGroup; // 血条的 CanvasGroup，用于调整透明度
    public float fadeDuration = 0.5f; // 控制血条消失的时间
    public float displayDuration = 2f; // 血条显示时间（2秒）

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 更新血条：计算比例并调用显示血条的协程
    public void UpdateHealthBar(float currentHP, float totalHP)
    {
        StopAllCoroutines();

        // 计算血条宽度比例
        float widthRatio = currentHP / totalHP;
        StartCoroutine(ShowHealthBar(widthRatio));
    }

    private IEnumerator ShowHealthBar(float targetWidthRatio)
    {
        // 显示血条并逐渐提高Alpha
        healthBarCanvasGroup.alpha = 0;
        healthBarCanvasGroup.gameObject.SetActive(true);

/*        // 逐渐增加Alpha到1
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += 1;*/
            healthBarCanvasGroup.alpha = Mathf.Clamp(1, 0, 1);
/*            yield return null;
        }*/

        // 获取当前血条宽度
        float startWidth = healthBarFill.sizeDelta.x;

/*        // 逐渐改变血条宽度
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newWidth = Mathf.Lerp(startWidth, targetWidthRatio * healthBarFill.parent.GetComponent<RectTransform>().sizeDelta.x, elapsedTime / fadeDuration);
            healthBarFill.sizeDelta = new Vector2(newWidth, healthBarFill.sizeDelta.y);
            yield return null;
        }*/

        // 直接设置最终宽度
        healthBarFill.sizeDelta = new Vector2(targetWidthRatio * healthBarFill.parent.GetComponent<RectTransform>().sizeDelta.x, healthBarFill.sizeDelta.y);

        // 2秒后逐渐让血条消失
        yield return new WaitForSeconds(displayDuration);

        StartCoroutine(FadeOutHealthBar());
    }

    private IEnumerator FadeOutHealthBar()
    {
        // 逐渐降低Alpha
        float alpha = healthBarCanvasGroup.alpha;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime / fadeDuration * 2; // 衰退速度加快
            healthBarCanvasGroup.alpha = Mathf.Clamp(alpha, 0, 1);
            yield return null;
        }

        // 完全消失后禁用血条
        healthBarCanvasGroup.gameObject.SetActive(false);
    }
}
