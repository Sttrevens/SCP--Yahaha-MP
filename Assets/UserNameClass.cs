using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public struct UserName {
    public int id;
    public string nickName;
}

[System.Serializable]
public class UserNameWrapper
{
    public UserName[] userNames;
}

public class UserNameClass
{
    static public UserName[] userName; //配置
    static private UserName defaultName = new UserName { id = 0, nickName = "默认用户" };

    public UserNameClass(){
        LoadByJson();
    }

    private void LoadByJson() {
        try 
        {
            string userNamePath = Path.Combine(Application.streamingAssetsPath, "Jsons", "UserName.json");
            
            if (!File.Exists(userNamePath))
            {
                Debug.LogError("用户名配置加载失败: 未找到文件 " + userNamePath);
                InitializeDefaultUserNames();
                return;
            }
            
            string userNameText = File.ReadAllText(userNamePath);
            Debug.Log("用户名配置文件内容：\n" + userNameText);
            
            try 
            {
                // 检查是否是数组格式，如果是，则包装它
                if (userNameText.TrimStart().StartsWith("["))
                {
                    userNameText = "{\"userNames\":" + userNameText + "}";
                    Debug.Log("转换后的JSON格式：\n" + userNameText);
                }

                var wrapper = JsonUtility.FromJson<UserNameWrapper>(userNameText);
                if (wrapper == null || wrapper.userNames == null || wrapper.userNames.Length == 0)
                {
                    Debug.LogError("用户名配置为空或无效");
                    InitializeDefaultUserNames();
                    return;
                }
                userName = wrapper.userNames;
                Debug.Log("用户名配置解析结果：" + userName.Length + "条配置");
                Debug.Log("解析后的配置内容：\n" + JsonUtility.ToJson(wrapper, true));
            }
            catch (System.Exception e)
            {
                Debug.LogError("用户名配置JSON解析失败：" + e.Message);
                InitializeDefaultUserNames();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadByJson整体执行失败：" + e.Message);
            InitializeDefaultUserNames();
        }
    }

    private void InitializeDefaultUserNames()
    {
        Debug.Log("初始化默认用户名列表");
        userName = new UserName[]
        {
            new UserName { id = 0, nickName = "用户1" },
            new UserName { id = 1, nickName = "用户2" },
            new UserName { id = 2, nickName = "用户3" }
        };
    }

    static public UserName GetRandomName(){
        if (userName == null || userName.Length == 0)
        {
            Debug.LogWarning("用户名列表为空，返回默认用户名");
            return defaultName;
        }
        return userName[Random.Range(0, userName.Length)];
    }
}