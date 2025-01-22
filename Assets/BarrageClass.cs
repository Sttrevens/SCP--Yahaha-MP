using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class BarrageClass
{
    public static BarrageClass instance;
    BarrageJson[] barrageJson;//配置
    BarrageItemsJson[] barrageItemsJson;//配置
    private List<BarrageItemJson> curbarrages = new List<BarrageItemJson>();
    static public Dictionary<BarrageType, List<BarrageItemsJson>> TYPE_Barrage = new Dictionary<BarrageType, List<BarrageItemsJson>>{};
    public BarrageClass()
    {
        if(instance != null){
            return;
        }
        instance = this;
        LoadByJson();
        reset();
    }
    public void reset()
    {
        TYPE_Barrage.Add(BarrageType.newbie, getBarrageListByType(BarrageType.newbie));
        TYPE_Barrage.Add(BarrageType.day, getBarrageListByType(BarrageType.day));
        TYPE_Barrage.Add(BarrageType.night, getBarrageListByType(BarrageType.night));
        TYPE_Barrage.Add(BarrageType.success, getBarrageListByType(BarrageType.success));
        TYPE_Barrage.Add(BarrageType.fail, getBarrageListByType(BarrageType.fail));
        Debug.Log("新手弹幕:"+JsonMapper.ToJson(TYPE_Barrage[BarrageType.newbie]));
    }
    //获取弹幕组合;
    public List<BarrageItemsJson> getBarrageListByType(BarrageType type){
        string[] idArr = barrageJson[((int)type)].item_arr;
        List<BarrageItemsJson> barrageArr = new List<BarrageItemsJson>();
        for(int i = 0; i < idArr.Length; i++) {
            int id = int.Parse(idArr[i]);
            barrageArr.Add(barrageItemsJson[id]);
        }
        return barrageArr;
    }
    //Json转换成对应的object
    private void LoadByJson () {
        TextAsset text = Resources.Load<TextAsset>("Jsons/" + "Barrage");
        barrageJson = JsonMapper.ToObject<BarrageJson[]>(text.text);
        Debug.Log("弹幕: 原json"+text);
        text = Resources.Load<TextAsset>("Jsons/" + "BarrageItem");
        barrageItemsJson = JsonMapper.ToObject<BarrageItemsJson[]>(text.text);
        Debug.Log("弹幕:"+JsonMapper.ToJson(barrageJson) + JsonMapper.ToJson(barrageItemsJson));
    }
        //获取弹幕类型;
    public BarrageJson getBarrageByType(BarrageType type){
        return barrageJson[((int)type)];
    }
        //获取弹幕组合;
    public BarrageItemJson[] getBarrageByID(int id){
        return barrageItemsJson[id].item;
    }
}
public enum BarrageType{
    newbie,
    day,
    night,
    success,
    fail
    }
/// <summary>
/// 弹幕类型汇总
/// </summary>
public class BarrageJson
{
    public int id = 0;
    public string type = "";
    public double min;//弹幕滚动速度
    public double max;//弹幕滚动速度
    public string[] item_arr;//弹幕id    
}
/// <summary>
/// 弹幕组合
/// </summary>
public class BarrageItemsJson
{
    public int id;
    public string desc;//类型说明
    public BarrageItemJson[] item;
}
/// <summary>
/// 弹幕单例
/// </summary>
public class BarrageItemJson
{
    public int index;//
    public string desc;//弹幕内容
}
