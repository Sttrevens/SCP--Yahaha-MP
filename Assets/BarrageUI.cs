using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class BarrageUI : NetworkBehaviour
{
    public ScrollRect scroll_rect;//滚动区域
    public ScrollViewNevigation scrollViewNevigation;//用于滚动缓动动画的插件
    public GameObject item;//弹幕object
    private List<BarrageItemsJson> barrageItemsJson = new List<BarrageItemsJson>();//弹幕组合列表
    private BarrageItemsJson curBarrageArr;//当前弹幕组合
    private BarrageType barrageType;//当前弹幕组合类型
    private BarrageItemJson curBarrage;//当前弹幕
    private int arrIndex = 0;//第几组弹幕
    private int index = 0;//弹幕组合里的第几条弹幕
    private BarrageClass barrageClass;//当前弹幕信息
    private UserNameClass userNameClass;//当前用户名信息
    BarrageItemJson[] baragesArr;
    double min;//最小弹幕出现速度
    double max;//最大弹幕出现速度
    public bool isStop = true;//是否弹幕滚动停止了
    public override void Spawned()
    {
        Debug.Log("弹幕UI初始化");
        // 只需要创建一次实例
        
        userNameClass = new UserNameClass();
        if (BarrageClass.instance == null)
        {
            BarrageClass.instance = new BarrageClass();  // 确保初始化
        }
        // 确保实例化成功
        // if (barrageClass == null)
        // {
        //     Debug.LogError("弹幕类初始化失败");
        //     return;
        // }
        
        Debug.Log("弹幕类初始化成功");
    }

    public override void FixedUpdateNetwork()
    {
        SetBarrageList(BarrageType.day);
    }

    // {
    //     // 添加空检查
    //     // if (barrageClass == null)
    //     // {
    //     //     Debug.LogError("弹幕类为空，重新初始化");
    //     //     barrageClass = new BarrageClass();
    //     //     return;
    //     // }
    //     
    //     
    // }



    public void SetBarrageList(BarrageType type) 
    {
        Debug.Log($"设置弹幕列表，类型: {type}");
        barrageType = type;
        
        if (BarrageClass.instance == null)
        {
            Debug.LogError("barrageClass 为空");
            return;
        }
        
        min = BarrageClass.instance.getBarrageByType(type).min;
        max = BarrageClass.instance.getBarrageByType(type).max;
        Debug.Log($"弹幕速度范围: {min} - {max}");
        
        if (!BarrageClass.TYPE_Barrage.ContainsKey(barrageType))
        {
            Debug.LogError($"找不到类型 {barrageType} 的弹幕列表");
            return;
        }
        
        barrageItemsJson = BarrageClass.TYPE_Barrage[barrageType];
        Debug.Log($"获取到 {barrageItemsJson.Count} 条弹幕");
        
        index = 0;
        arrIndex = 0;
        
        if(isStop)
        {
            NextBarrageArr();
        }
    }


    // 切换到下一组弹幕
    public void NextBarrageArr(){
        //设置弹幕滚动停止为false
        isStop = false;

        if(barrageType == BarrageType.newbie){
                // 
                arrIndex = 1;//播放下一段新手弹幕
                //拿到当前弹幕组合
                curBarrageArr = barrageItemsJson[arrIndex];
                //拿到当前弹幕组合的弹幕
                baragesArr = curBarrageArr.item;
                //开始播放弹幕
                StartCoroutine(nextBarrage());
        }else if(barrageType == BarrageType.day || barrageType == BarrageType.night){//随机抽取一段弹幕
            //随机抽取一段弹幕
            arrIndex = Random.Range(0, barrageItemsJson.Count);
            //拿到当前弹幕组合
            curBarrageArr = barrageItemsJson[arrIndex];
            //拿到当前弹幕组合的弹幕
            baragesArr = curBarrageArr.item;
            //初始化弹幕索引
            index = 0;
            //开始播放弹幕
            StartCoroutine(nextBarrage());
        }else if(barrageType == BarrageType.success || barrageType == BarrageType.fail){//特定场景下弹幕
            if(arrIndex < barrageItemsJson.Count){//依次播放成功失败段落的弹幕
                curBarrageArr = barrageItemsJson[0];
                baragesArr = curBarrageArr.item;
                // baragesArr = AddBarage();
                RandomList(baragesArr, curBarrageArr.item.Length, out baragesArr);//打乱各条弹幕顺序
                StartCoroutine(nextBarrage());
                arrIndex ++;
            }
        }
    }
    void RandomList(BarrageItemJson[] barrageItemsArr, int count, out BarrageItemJson[] rangeArr){
        List<BarrageItemJson> barrageList = new List<BarrageItemJson>();
        List<int> indexList = new List<int>();//一个和animalList数量相同的序列List
        for(int i = 0; i < barrageItemsArr.Length; i++) {
            indexList.Add(i);
        }

        int countNum = barrageItemsArr.Length;
        while (barrageList.Count < countNum)
        {
            int rangeNum = Random.Range(0,indexList.Count-1);//随机一个数
            int index = indexList[rangeNum];//在List取出该随机数的index

            barrageList.Add(barrageItemsArr[index]);
            indexList.Remove(index);
            if(barrageList.Count == count) break;
        }
        rangeArr = barrageList.ToArray();
    }    
    
    //下一条弹幕
    IEnumerator nextBarrage(){
        curBarrage = baragesArr[index];
        RPC_CreateItem();
        index ++;
        yield return new WaitForSeconds(Random.Range(((float)min), ((float)max)));
        if(barrageType == BarrageType.newbie){//新手弹幕播放完后不会出新的内容
            if(index < baragesArr.Length){
                StartCoroutine(nextBarrage());
            }else{
                isStop = true;
            }
        }else if(barrageType == BarrageType.day || barrageType == BarrageType.night){//白天和黑夜都为单独一句
            NextBarrageArr();
        }else if(barrageType == BarrageType.success || barrageType == BarrageType.fail){//成功失败
            if(curBarrageArr.desc != "特定场景弹幕"){
                NextBarrageArr();
            }else{
                if(index < baragesArr.Length){
                    StartCoroutine(nextBarrage());
                }else{
                    isStop = true;
                }
            }
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_CreateItem()
    {
        if(!Object.HasStateAuthority) return;
        StartCoroutine(createItem());
    }

        //创建弹幕UI
    IEnumerator createItem(){
    NetworkObject _obj = Runner.Spawn(item, scroll_rect.content.transform.position, Quaternion.identity);
    //_obj.SetActive(true);
    yield return null;
    _obj.gameObject.GetComponent<BarrageItem>().setData(curBarrage);
    RPC_OnItemCreated(_obj);
}

[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
   public void RPC_OnItemCreated(NetworkObject _obj) {
       // 这里是在所有客户端执行
       if (_obj == null) return;
       
       RectTransform rt = _obj.gameObject.GetComponent<RectTransform>();
       if(rt == null) return;

       rt.SetParent(scroll_rect.content.transform);
       rt.anchoredPosition3D = Vector3.zero; // 重置锚点位置
       rt.localRotation = Quaternion.identity;
       rt.localScale = Vector3.one;
    Debug.Log("Barrage Fucked1.");
    // 强制刷新布局
    LayoutRebuilder.ForceRebuildLayoutImmediate(scroll_rect.content);
    
    scrollViewNevigation.Nevigate(rt, Mathf.Min(0.8f, ((float)min)/2));
    Debug.Log("Barrage Fucked2.");
   }

}
