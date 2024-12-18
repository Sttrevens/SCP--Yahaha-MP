using System.Collections;
using System.Collections.Generic;
using LPSurvivalEngine;
using TMPro;
using UnityEngine;

public class Prompt : MonoBehaviour
{
    public static Prompt instance{get;private set;}
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;

    void Awake()
    {
     if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    else
        {
            Destroy(gameObject); 
        }

         promptPanel.SetActive(false);

    }

    public void CustomPrompt(string text)
    {
        StartCoroutine(ShowAndHidePrompt(text));
    }

    public void SlotItemPrompt(ItemDatabase item)
    {
        string text;

        switch(item.type)
        {
            case ItemType.Consumable:
                text = string.Format("{0} Used!", item.name);
                break;

            case ItemType.Wieldable:
                text = string.Format("{0} Equipped!", item.name);
                break;

            default:
                text = string.Format("{0} Can't be used here! (Press C to throw away)", item.name);
                break;
        }

        StartCoroutine(ShowAndHidePrompt(text));
    }


    private IEnumerator ShowAndHidePrompt(string displayText, float duration=3.5f)
    {
        // 显示面板
        promptPanel.SetActive(true);

        // 设置提示文本
        promptText.text = displayText;

        // 等待指定的时间（默认 5 秒）
        yield return new WaitForSeconds(duration);

        // 隐藏面板
        promptPanel.SetActive(false);
    }



}
