using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrageUI : MonoBehaviour
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
    void Start(){
        // MessageCenter.Instance.RigisterListener(MessageName.INSERT_BARRAGE, InsertBarrage);
        Debug.Log("弹幕UI初始化");
        barrageClass = new BarrageClass();
        userNameClass = new UserNameClass();
        // 实例化的同时就拿到弹幕
        barrageClass = BarrageClass.instance;
        Debug.Log("弹幕类:" + barrageClass);
        Debug.Log("是否成功获取到单例: " + (barrageClass != null));
        //设置弹幕类型为新手
        
    }
    void Update(){
        SetBarrageList(BarrageType.day);
    }



    public void SetBarrageList(BarrageType type) {
        //拿到类名
        barrageType = type;
        //拿到最小和最大弹幕出现速度
        min = barrageClass.getBarrageByType(type).min;
        max = barrageClass.getBarrageByType(type).max;
        //拿到弹幕组合列表
        barrageItemsJson = BarrageClass.TYPE_Barrage[barrageType];
        //初始化弹幕组合索引
        index = 0;
        arrIndex = 0;
        //如果弹幕滚动停止了，则播放下一组弹幕
        if(isStop){
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
        StartCoroutine(createItem());
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
        //创建弹幕UI
    IEnumerator createItem(){
    GameObject _obj = Instantiate(item, scroll_rect.content.transform, false);
    _obj.SetActive(true);
    
    RectTransform rt = _obj.GetComponent<RectTransform>();
    rt.anchoredPosition3D = Vector3.zero; // 重置锚点位置
    rt.localRotation = Quaternion.identity;
    rt.localScale = Vector3.one;

    _obj.GetComponent<BarrageItem>().setData(curBarrage);
    
    // 强制刷新布局
    LayoutRebuilder.ForceRebuildLayoutImmediate(scroll_rect.content);
    
    yield return new WaitForEndOfFrame();
    scrollViewNevigation.Nevigate(rt, Mathf.Min(0.8f, ((float)min)/2));
}

}
