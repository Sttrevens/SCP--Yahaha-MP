using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson; 
public struct UserName{
    int id;
    public string nickName;
}
public class UserNameClass
{
    static public UserName[] userName;//配置

    public UserNameClass(){
        LoadByJson();
    }
    private void LoadByJson () {
        TextAsset text = Resources.Load<TextAsset>("Jsons/" + "UserName");
        userName = JsonMapper.ToObject<UserName[]>(text.text);
    }
    static public UserName GetRandomName(){
        return userName[Random.Range(0, userName.Length)];
    }
}
