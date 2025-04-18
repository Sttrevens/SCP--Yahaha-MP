using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilmTarget : MonoBehaviour
{
    public float maxAestheticFatigueValue; //最大审美疲劳值
    public float currentAestheticFatigueValue; //当前审美疲劳值
    public int aestheticLevel; //拍摄对象的美学等级，越高拍摄得分越高
    public string targetTag; //拍摄对象的标签，方便检索对应的弹幕池

    void Start()
    {
        currentAestheticFatigueValue = maxAestheticFatigueValue;
    }
}
