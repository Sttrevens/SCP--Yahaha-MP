using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public enum BarrageType{
    newbie,
    good,
    bad,
    success,
    fail,
    goodenemy,
    badenemy,
    spam,
    goodEyedSlimeBlue,
    badEyedSlimeBlue,
    goodPlayer,
    badPlayer,
    goodDaGuai,
    goodLieQ,
    goodPurpleFlower,
    badPurpleFlower,
    goodCactusMaster,
    goodGhostGreen,
    goodGhostTrans,
    goodBlueScyver,
    goodGlodenScyver,
    goodRedScyver,
    goodJudaEyedSlimeRed,
    badGhostTrans
}

[Serializable]
public class BarrageJson
{
    public int id = 0;
    public string type = "";
    public double min; //弹幕滚动速度
    public double max; //弹幕滚动速度
    public string[] item_arr; //弹幕id    
}

[Serializable]
public class BarrageItemsJson
{
    public int id;
    public string desc; //类型说明
    public BarrageItemJson[] item;
}

[Serializable]
public class BarrageItemJson
{
    public int index;
    public string desc; //弹幕内容
}

[Serializable]
public class BarrageJsonWrapper
{
    public BarrageJson[] barrages;
}

[Serializable]
public class BarrageItemsJsonWrapper
{
    public BarrageItemsJson[] items;
}

public class BarrageClass
{
    public static BarrageClass instance;
    public BarrageJson[] barrageJson;//配置
    public BarrageItemsJson[] barrageItemsJson;//配置
    private List<BarrageItemJson> curbarrages = new List<BarrageItemJson>();
    static public Dictionary<BarrageType, List<BarrageItemsJson>> TYPE_Barrage = new Dictionary<BarrageType, List<BarrageItemsJson>>{};
    public BarrageClass()
    {
        if(instance != null){
            return;
        }
        instance = this;
        Debug.Log("弹幕类初始化");
        LoadByJson();
        Debug.Log("弹幕类加载完成mmmmmmmmmmmmmmmmmm");
        reset();
    }
    private void LoadByJson()
    {
        try 
        {
            string barragePath = Path.Combine(Application.streamingAssetsPath, "Jsons", "Barrage.json");
            string barrageItemPath = Path.Combine(Application.streamingAssetsPath, "Jsons", "BarrageItem.json");

            // 读取第一个配置文件
            if (!File.Exists(barragePath))
            {
                Debug.LogError("弹幕配置加载失败: 未找到文件 " + barragePath);
                return;
            }
            
            string barrageText = File.ReadAllText(barragePath);
            Debug.Log("弹幕配置文件内容：\n" + barrageText);
            
            try 
            {
                // 使用JsonUtility解析
                var wrapper = JsonUtility.FromJson<BarrageJsonWrapper>("{\"barrages\":" + barrageText + "}");
                barrageJson = wrapper.barrages;
                
                // 添加详细的日志输出
                Debug.Log("弹幕配置解析结果：" + (barrageJson != null ? barrageJson.Length + "条配置" : "解析失败"));
                
                // 输出每条弹幕配置的详细信息
                if (barrageJson != null)
                {
                    for (int i = 0; i < barrageJson.Length; i++)
                    {
                        string itemArrString = barrageJson[i].item_arr != null 
                            ? string.Join(", ", barrageJson[i].item_arr)
                            : "null";
                            
                        Debug.Log($"第{i}条弹幕配置：\n" +
                            $"ID: {barrageJson[i].id}\n" +
                            $"类型: {barrageJson[i].type}\n" +
                            $"最小速度: {barrageJson[i].min}\n" +
                            $"最大速度: {barrageJson[i].max}\n" +
                            $"弹幕ID数组: [{itemArrString}]");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("弹幕配置JSON解析失败：" + e.Message);
                return;
            }

            // 读取第二个配置文件
            if (!File.Exists(barrageItemPath))
            {
                Debug.LogError("弹幕配置加载失败: 未找到文件 " + barrageItemPath);
                return;
            }
            
            string barrageItemText = File.ReadAllText(barrageItemPath);
            
            try
            {
                // 使用JsonUtility解析
                var wrapper = JsonUtility.FromJson<BarrageItemsJsonWrapper>("{\"items\":" + barrageItemText + "}");
                barrageItemsJson = wrapper.items;
                Debug.Log("弹幕项目解析结果：" + (barrageItemsJson != null ? barrageItemsJson.Length + "条配置" : "解析失败"));
            }
            catch (System.Exception e)
            {
                Debug.LogError("弹幕项目JSON解析失败：" + e.Message);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadByJson整体执行失败：" + e.Message);
        }
    }
    public void reset()
    {
        // ... existing code ...
        foreach (BarrageType type in System.Enum.GetValues(typeof(BarrageType)))
        {
            TYPE_Barrage.Add(type, getBarrageListByType(type));
        }
        
        // 创建临时包装类来序列化列表
        var wrapper = new BarrageItemsJsonWrapper { items = TYPE_Barrage[BarrageType.newbie].ToArray() };
        string json = JsonUtility.ToJson(wrapper, true);
        Debug.Log("新手弹幕:" + json);
    }
    //获取弹幕组合;
    public List<BarrageItemsJson> getBarrageListByType(BarrageType type)
    {
        Debug.Log($"获取类型 {type} 的弹幕列表");
        if (barrageJson == null || barrageJson.Length <= (int)type)
        {
            Debug.LogError($"barrageJson 无效或索引 {(int)type} 超出范围");
            return new List<BarrageItemsJson>();
        }
        
        string[] idArr = barrageJson[((int)type)].item_arr;
        // 输出当前类型的配置信息
        Debug.Log($"类型 {type} 的配置: " + JsonUtility.ToJson(barrageJson[(int)type], true));
        Debug.Log($"类型 {type} 的 ID 数组长度: {idArr.Length}");
        
        List<BarrageItemsJson> barrageArr = new List<BarrageItemsJson>();
        for(int i = 0; i < idArr.Length; i++) 
        {
            int id = int.Parse(idArr[i]);
            Debug.Log($"处理 ID: {id}");
            if (barrageItemsJson != null && id < barrageItemsJson.Length)
            {
                barrageArr.Add(barrageItemsJson[id]);
                // 输出每个添加的弹幕项目信息
                Debug.Log($"添加弹幕项目 {id}: " + JsonUtility.ToJson(barrageItemsJson[id], true));
            }
            else
            {
                Debug.LogError($"无效的弹幕项目 ID: {id}");
            }
        }
        
        // 输出最终结果
        var wrapper = new BarrageItemsJsonWrapper { items = barrageArr.ToArray() };
        Debug.Log($"类型 {type} 的最终弹幕列表: " + JsonUtility.ToJson(wrapper, true));
        
        return barrageArr;
    }
    public BarrageJson getBarrageByType(BarrageType type){
        Debug.Log("获取弹幕类型" + type);
        return barrageJson[((int)type)];
    }
        //获取弹幕组合;
    public BarrageItemJson[] getBarrageByID(int id){
        return barrageItemsJson[id].item;
    }
}