
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrageItem : MonoBehaviour
{
    public Text userName;
    public Text text;

    public void setData(BarrageItemJson data){
        Debug.Log("开始set");
        userName.text = UserNameClass.GetRandomName().nickName + ": ";
        text.text = data.desc;
    }
    private void Update() {
        GetComponent<RectTransform>().localScale = Vector3.one;
    }
}

