
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

    public void setData(BarrageItemJson data){
        if(!Object.HasStateAuthority) return;
        Debug.Log("开始set");
        userNameText = UserNameClass.GetRandomName().nickName + ": ";
        textText = data.desc;
    }

    public override void Spawned()
    {
        userName.text = userNameText;
        text.text = textText;
        GetComponent<RectTransform>().localScale = Vector3.one;
    }

    public override void FixedUpdateNetwork()
    {
        userName.text = userNameText;
        text.text = textText;
        GetComponent<RectTransform>().localScale = Vector3.one;
    }
    
}

