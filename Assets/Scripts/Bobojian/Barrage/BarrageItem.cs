
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class BarrageItem : NetworkBehaviour
{
    [Networked] public string userNameText { get; set; }
    public Text userName;
    [Networked] public string textText { get; set; }
    public Text text;

    private string localUserNameText;
    private string localTextText;
    public void setData(BarrageItemJson data, string username){
        if(!Object.HasStateAuthority) return;
        Debug.Log("开始set");
        localUserNameText = username + ": ";
        localTextText = data.desc;
    }

    public override void Spawned()
    {
        userName.text = userNameText;
        text.text = textText;
        GetComponent<RectTransform>().localScale = Vector3.one;
    }

    private void FixedUpdate()
    {
        if (Object.HasStateAuthority)
        {
            userNameText = localUserNameText;
            textText = localTextText;
        }
    }

    public override void FixedUpdateNetwork()
    {
        userName.text = userNameText;
        text.text = textText;
        GetComponent<RectTransform>().localScale = Vector3.one;
    }
}

