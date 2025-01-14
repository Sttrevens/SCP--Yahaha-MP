using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    public GameObject fadeScreen;
    public Image blackScreenImage; // ���ں����� Image ���
    public TextMeshProUGUI subtitleText; // ������ʾ��Ļ�� Text ���

    public string subtitle = "May this journey lead to the stars."; // ��ʾ����Ļ�ı�
    public float fadeDuration = 2.0f; // �������ʱ��
    public float holdSubtitleDuration = 2.0f; // ��Ļ����ʱ��

    public ScoreManager scoreManager;

    private void Awake()
    {
        // ȷ����������Ļ����ѱ���ֵ
        if (blackScreenImage == null || subtitleText == null)
        {
            Debug.LogError("blackScreenImage �� subtitleText δ��ȷ��ֵ��");
            return;
        }

        // ���ú�������ĻΪȫ͸�������ɼ�
        blackScreenImage.gameObject.SetActive(true); // ȷ�����󼤻����ֱ���޸� color ����
        blackScreenImage.color = new Color(0, 0, 0, 0); // ����Ϊȫ͸��

        subtitleText.gameObject.SetActive(true); // ȷ�����󼤻�
        subtitleText.text = ""; // ȷ��û���ı�����
        subtitleText.color = new Color(1, 1, 1, 0); // ����Ϊȫ͸��
    }
    // private void FadeScreen()
    // {
    //     // ��ʼʱ��������Ļ���ɼ�
    //     blackScreenImage.gameObject.SetActive(true);
    //     subtitleText.gameObject.SetActive(true);
    //
    //     blackScreenImage.color = new Color(0, 0, 0, 0); // ��ʼ͸����Ϊ0
    //     subtitleText.text = ""; // ��ʼû����Ļ
    //     subtitleText.color = new Color(1, 1, 1, 0); // ��ʼ͸����Ϊ0
    //
    //     // �����������
    //     StartCoroutine(FadeScreenAndShowSubtitle());
    // }

    // �����������ʾ��Ļ��Э��
    private IEnumerator FadeScreenAndShowSubtitle(bool isBack)
    {
        
        // �𽥱��
        yield return StartCoroutine(FadeToBlack());

        // ��ʾ��Ļ
        if (!isBack)
        {
            subtitleText.text = subtitle;
        }
        else
        {
            subtitleText.text = "Your total viewers: " + scoreManager.totalScore.ToString("F0");
        }
        yield return StartCoroutine(ShowSubtitle());

        // �𽥻ָ�����
        yield return StartCoroutine(FadeFromBlack());

        // �ڻָ�����������Ļ
        subtitleText.text = "";
        scoreManager.totalScore = 0;
        fadeScreen.SetActive(false);
    }
    
    public void TriggerScreenFade(bool showSubtitle)
    {
        fadeScreen.SetActive(true);
        StartCoroutine(FadeScreenAndShowSubtitle(showSubtitle));
    }

    // ���䵽����
    private IEnumerator FadeToBlack()
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration); // ��͸������͸��
            blackScreenImage.color = new Color(0, 0, 0, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        blackScreenImage.color = new Color(0, 0, 0, 1); // ȷ����ȫ����
    }

    // ��ʾ��Ļ
    private IEnumerator ShowSubtitle()
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration); // ��Ļ����
            subtitleText.color = new Color(1, 1, 1, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        subtitleText.color = new Color(1, 1, 1, 1); // ȷ����Ļ��ȫ�ɼ�

        // �ȴ���Ļ����ʱ��
        yield return new WaitForSeconds(holdSubtitleDuration);
    }

    // ����ָ�����
    private IEnumerator FadeFromBlack()
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration); // �Ӳ�͸����͸��
            blackScreenImage.color = new Color(0, 0, 0, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        blackScreenImage.color = new Color(0, 0, 0, 0); // ȷ����ȫ͸��
    }
}
