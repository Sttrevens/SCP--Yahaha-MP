using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchController : MonoBehaviour,IInteractable
{
    public bool IsPressed;
    public GameOverScreen GameOverScreen;
    public Image blackScreenImage; // 用于黑屏的 Image 组件
    public TextMeshProUGUI subtitleText;
    // Start is called before the first frame update
    void Start()
    {
        GameOverScreen = GetComponent<GameOverScreen>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string GetInteractText()
    {
        return string.Format("{0}", IsPressed ? "Already Uploaded" : "UpLoad");
    }
    public void OnInteract()
    {
       IsPressed = true;
        GameOverScreen.ShowScore();
    }
}
