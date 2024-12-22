using System.Collections;
using System.Collections.Generic;
using LPSurvivalEngine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Prompt : MonoBehaviour
{
    public static Prompt instance{get;private set;}
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;
    public PlayerInput playerInput;

    private InputAction dropAction;
    private InputAction actionAction;

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

        if (playerInput != null)
        {
            dropAction = playerInput.actions.FindAction("Drop");
            actionAction = playerInput.actions.FindAction("Action");
        }

        else { Debug.LogError("Player Input is null,"); }

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
                text = string.Format("{0} Selected, press {1} to use", item.name, actionAction.bindings[0].ToString());
                break;

            case ItemType.Wieldable:
                text = string.Format("{0} Equipped!", item.name);
                break;

            default:
                text = string.Format("{0} Can't be used here! (Press {1} to throw away)", item.name, dropAction.bindings[0].ToDisplayString());
                break;
        }

        StartCoroutine(ShowAndHidePrompt(text));
    }

    public void UseItemPrompt(ItemDatabase item)
    {
        string text = string.Format("{0} Used!", item.name);

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
