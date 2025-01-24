using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrageItem : MonoBehaviour
{
    public Text userName;
    public Text text;
    private string string1 = "爷傲_奈我何";
    private string string2 = "沧州市精神病院王院长";
    private string string3 = "无法被选中的NPC";
    private string string4 = "菜菜坤114514号";
    private string string5 = "脑袋尖尖";
    private string string6 = "活著就是為了桜烏麻衣";
    private string string7 = "不知道起什么昵称好呢";
    private string string8 = "丛雨ciallo";
    private string string9 = "小璐不是一头鹿";
    private string string10 = "知识的雪豹";

    private string string11 = "东东";
    private string string12 = "陈子元";
    private string string13 = "张张";
    private string string14 = "Jiojio";
    private string string15 = "锐克5代联系我";
    private string string16 = "芝士雪豹";
    private string string17 = "直播间有人，出事就报警";
    private string string18 = "感觉不如原神...画质";
    private string string19 = "害怕的啊啊啊，啊啊啊啊啊啊啊啊";
    private string string20 = "这个画面怎么看的我想吐";
    private string string21 = "邮电部诗人";
    private string string22 = "没影子啊";
    private string string23 = "这特么跟鬼屋里的活体NPC似的 吓人";
    private string string24 = "他的头为什么前倾";
    private string string25 = "你是？";
    

    public void setData(BarrageItemJson data){
        Debug.Log("开始set");
        userName.text = GetRandomStringName() + "：";
        text.text = GetRandomStringText();
    }
    private void Update() {
        GetComponent<RectTransform>().localScale = Vector3.one;
    }
    public string GetRandomStringName()
    {
        // 随机生成一个0到3之间的数字
        int randomIndex = Random.Range(0, 15);  

        // 使用随机索引返回字符串
        switch (randomIndex)
        {
            case 0: return string1;
            case 1: return string2;
            case 2: return string3;
            case 3: return string4;
            case 4: return string5;
            case 5: return string6;
            case 6: return string7;
            case 7: return string8;
            case 8: return string9;
            case 9: return string10;
            case 10: return string11;
            case 11: return string12;
            case 12: return string13;
            case 13: return string14;
            case 14: return string15;
            default: return "Unknown String!";  // 理论上不会到这里
        }
    }
    public string GetRandomStringText()
    {
        // 随机生成一个0到3之间的数字
        int randomIndex = Random.Range(0, 10);  

        // 使用随机索引返回字符串
        switch (randomIndex)
        {
            case 0: return string16;
            case 1: return string17;
            case 2: return string18;
            case 3: return string19;
            case 4: return string20;
            case 5: return string21;
            case 6: return string22;
            case 7: return string23;
            case 8: return string24;
            case 9: return string25;
            default: return "Unknown String!";  // 理论上不会到这里
        }
    }
}