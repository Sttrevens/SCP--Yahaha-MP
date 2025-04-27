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

    public List<ConeDetection> padCameras;
    private List<FilmTarget> _enemies;
    private int _currentSumAestheticLevel;
    
    public static BarrageUI instance;

    public int goodViewersAmount = 100;
    public float spamProbability;

    void Awake()
    {
        instance = this;
        padCameras = new List<ConeDetection>();
        _enemies = new List<FilmTarget>();
    }
    
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
    // 更新敌人数据
    UpdateEnemyData();

    // 计算内容相关弹幕概率
    float contentBarrageChance = CalculateContentBarrageChance();

    // 计算观众趋势
    float viewersTrend = CalculateViewersTrend(5f);
    
    // 获取当前观众数量
    int currentViewers = ScoreManager.Instance.CurrentViewers;
    
    // 判断观众数量是否太少
    bool forceBadBarrage = currentViewers < goodViewersAmount;

    // 处理两种弹幕类型
    if (Random.value <= contentBarrageChance)
    {
        // Content Barrage路径
        HandleContentBarrage(viewersTrend, forceBadBarrage);
    }
    else
    {
        // 非Content Barrage路径
        HandleRegularBarrage(viewersTrend, forceBadBarrage);
    }
}

// 更新敌人数据
private void UpdateEnemyData()
{
    if (_enemies != null)
        _enemies.Clear();
    
    _currentSumAestheticLevel = 0;
    
    foreach (var padCamera in padCameras)
    {
        if (padCamera.cachedTargets.Count >= 0)
        {
            foreach (var enemy in padCamera.targetsInView)
            {
                _enemies.Add(enemy.GetComponent<FilmTarget>());
            }
        }
    }

    foreach (var _enemy in _enemies)
    {
        _currentSumAestheticLevel += _enemy.aestheticLevel;
    }
}

// 计算内容相关弹幕概率
private float CalculateContentBarrageChance()
{
    if (_currentSumAestheticLevel >= 20)
    {
        return 0.7f;
    }
    else
    {
        return _currentSumAestheticLevel * 0.7f / 20f;
    }
}

// 处理内容相关弹幕（Content Barrage）
private void HandleContentBarrage(float viewersTrend, bool forceBadBarrage)
{
    // 尝试选择基于特定敌人标签的弹幕
    foreach (var _enemy in _enemies)
    {
        if (_enemy.targetTag != "")
        {
            if (Random.value <= _enemy.aestheticLevel / _currentSumAestheticLevel)
            {
                // 如果观众少于阈值，强制使用坏弹幕
                // 否则基于趋势判断
                bool useGoodBarrage = !forceBadBarrage && 
                                     (viewersTrend >= -0.1f);

                string barrageTypeName = useGoodBarrage ? 
                                       "good" + _enemy.targetTag : 
                                       "bad" + _enemy.targetTag;
                
                // 尝试将字符串转换为枚举值
                if (System.Enum.TryParse(barrageTypeName, out BarrageType barrageType))
                {
                    SetBarrageList(barrageType);
                    return;
                }
                else
                {
                    Debug.LogWarning($"无法找到匹配的BarrageType: {barrageTypeName}");
                    // 如果转换失败，回退到通用enemy弹幕
                }
            }
        }
    }
    
    // 如果没有成功使用特定敌人标签，使用通用enemy弹幕
    // 强制坏弹幕或基于趋势判断
    if (forceBadBarrage || viewersTrend < -0.1f)
    {
        SetBarrageList(BarrageType.badenemy);
    }
    else
    {
        SetBarrageList(BarrageType.goodenemy);
    }
}

// 处理普通弹幕（非Content Barrage）
private void HandleRegularBarrage(float viewersTrend, bool forceBadBarrage)
{
    var currentViewers = ScoreManager.Instance.CurrentViewers;
    spamProbability = 0f;
    
    // 确定垃圾弹幕的概率
    if (currentViewers < goodViewersAmount / 5)
    {
        // 观众太少，跳过垃圾弹幕判断，直接设置好/坏弹幕
        SetRegularGoodBadBarrage(viewersTrend, forceBadBarrage);
        return;
    }
    else if (currentViewers < goodViewersAmount)
    {
        spamProbability = 0.01f; // 观众少，10%概率显示垃圾弹幕
    }
    else if (currentViewers < goodViewersAmount * 2)
    {
        spamProbability = 0.02f; // 观众适中，20%概率显示垃圾弹幕
    }
    else // currentViewers >= goodViewersAmount * 2
    {
        spamProbability = 0.03f; // 观众很多，30%概率显示垃圾弹幕
    }
    
    // 决定是否显示垃圾弹幕
    if (Random.value <= spamProbability)
    {
        SetBarrageList(BarrageType.spam);
    }
    else
    {
        SetRegularGoodBadBarrage(viewersTrend, forceBadBarrage);
    }
}

// 设置普通good/bad弹幕（非enemy类型）
private void SetRegularGoodBadBarrage(float viewersTrend, bool forceBadBarrage)
{
    // 如果观众少于阈值，强制设置为bad
    // 否则根据趋势判断
    if (forceBadBarrage)
    {
        SetBarrageList(BarrageType.bad);
    }
    else if (viewersTrend < 0)
    {
        if (Random.value <= -viewersTrend)
            SetBarrageList(BarrageType.bad);
        else
        {
            SetBarrageList(BarrageType.good);
        }
    }
    else
    {
            SetBarrageList(BarrageType.good);
    }
}

// 计算观众趋势
private float CalculateViewersTrend(float timeWindow)
{
    // 获取当前和历史观众数据
    var scoreManager = ScoreManager.Instance;
    int currentViewers = scoreManager.CurrentViewers;
    int lastFrameViewers = scoreManager.LastFrameViewers;
    float timeElapsed = Time.time - scoreManager.LastViewersDecreaseTime;
    
    // 如果已经记录了足够长的下降时间
    if (timeElapsed > 0 && timeElapsed <= timeWindow)
    {
        // 计算下降持续时间占比
        float decreaseRatio = timeElapsed / timeWindow;
        
        // 根据下降持续时间评估趋势
        if (decreaseRatio > 0.7f) // 如果下降超过70%的时间窗口
        {
            return -0.5f; // 明显的下降趋势
        }
        else if (decreaseRatio > 0.3f) // 如果下降超过30%的时间窗口
        {
            return -0.2f; // 轻微的下降趋势
        }
    }
    
    // 如果当前观众数量正在增加
    if (currentViewers > lastFrameViewers)
    {
        return 0.3f; // 返回正值表示上升趋势
    }
    
    // 默认返回轻微的正值（偏向积极判断）
    return 0.1f;
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
        
        int viewers = ScoreManager.Instance.CurrentViewers;
if (viewers >= 500)
{
    min = 1;
    max = 2;
}
else
{
    min = Mathf.Lerp(5, 2, viewers / 500f);
    max = Mathf.Lerp(10, (float)min, viewers / 500f);
}
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

        /*if(barrageType == BarrageType.newbie){
                // 
                arrIndex = 1;//播放下一段新手弹幕
                //拿到当前弹幕组合
                curBarrageArr = barrageItemsJson[arrIndex];
                //拿到当前弹幕组合的弹幕
                baragesArr = curBarrageArr.item;
                //开始播放弹幕
                StartCoroutine(nextBarrage());
        }else if((barrageType == BarrageType.good || barrageType == BarrageType.bad) || barrageType == BarrageType.spam){*/
            //随机抽取一段弹幕
            arrIndex = Random.Range(0, barrageItemsJson.Count);
            //拿到当前弹幕组合
            curBarrageArr = barrageItemsJson[arrIndex];
            //拿到当前弹幕组合的弹幕
            baragesArr = curBarrageArr.item;
            //初始化弹幕索引
            index = Random.Range(0, baragesArr.Length);
            //开始播放弹幕
            StartCoroutine(nextBarrage());
        /*}else if(barrageType == BarrageType.success || barrageType == BarrageType.fail){//特定场景下弹幕
            if(arrIndex < barrageItemsJson.Count){//依次播放成功失败段落的弹幕
                curBarrageArr = barrageItemsJson[0];
                baragesArr = curBarrageArr.item;
                // baragesArr = AddBarage();
                RandomList(baragesArr, curBarrageArr.item.Length, out baragesArr);//打乱各条弹幕顺序
                StartCoroutine(nextBarrage());
                arrIndex ++;
            }
        }*/
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
        yield return new WaitForSeconds(Random.Range(((float)min), ((float)max)));
        curBarrage = baragesArr[index];
        RPC_CreateItem();
        index ++;
        /*if(barrageType == BarrageType.newbie){//新手弹幕播放完后不会出新的内容
            if(index < baragesArr.Length){
                StartCoroutine(nextBarrage());
            }else{
                isStop = true;
            }
        }else if((barrageType == BarrageType.good || barrageType == BarrageType.bad) || barrageType == BarrageType.spam){*/
            NextBarrageArr();
        /*}else if(barrageType == BarrageType.success || barrageType == BarrageType.fail){//成功失败
            if(curBarrageArr.desc != "特定场景弹幕"){
                NextBarrageArr();
            }else{
                if(index < baragesArr.Length){
                    StartCoroutine(nextBarrage());
                }else{
                    isStop = true;
                }
            }
        }*/
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