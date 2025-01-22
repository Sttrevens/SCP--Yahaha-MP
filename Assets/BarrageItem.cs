
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarrageItem : MonoBehaviour
{
    public TMP_Text userName;
    public TMP_Text text;

    public void setData(BarrageItemJson data){
        userName.text = UserNameClass.GetRandomName().nickName + ": ";
        text.text = data.desc;
    }
    private void Update() {
        GetComponent<RectTransform>().localScale = Vector3.one;
    }
}
