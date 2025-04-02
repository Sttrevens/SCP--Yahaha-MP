using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections.Generic;
using System.Collections;

public class RigController : MonoBehaviour
{
    public RigBuilder rigBuilder;
    

    public Dictionary<string, Rig> rigs = new Dictionary<string, Rig>();
    private Dictionary<Rig, Coroutine> rigCoroutines = new Dictionary<Rig, Coroutine>();

    private void Awake()
    {
        //初始化字典
        foreach (var rigLayer in rigBuilder.layers)
        {
            print("[RigManager] 现在的riglayer "+rigLayer.name);
            //在加载时候就初始化权重为0
            rigLayer.rig.weight = .0f;
            rigs[rigLayer.rig.name] = rigLayer.rig;
        }
    }

    /// <summary>
    /// 通过名字拿到rig的实例
    /// </summary>
    /// <param name="rigName"></param>
    /// <param name="targetWeight"></param>
    /// <param name="speed"></param>
    public void SetRigWeight(string rigName, float targetWeight, float speed)
    {
        if (!rigs.ContainsKey(rigName))
        {
            Debug.LogError($"不存在名为{rigName}的Rig!");
            return;
        }

        Rig rig = rigs[rigName];

        if (rigCoroutines.ContainsKey(rig) && rigCoroutines[rig] != null)
            StopCoroutine(rigCoroutines[rig]);

        rigCoroutines[rig] = StartCoroutine(ChangeWeightRoutine(rig, targetWeight, speed));
    }

    private IEnumerator ChangeWeightRoutine(Rig rig, float target, float speed)
    {
        float initial = rig.weight;
        float elapsed = 0f;
        float duration = Mathf.Abs(target - initial) / speed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rig.weight = Mathf.Lerp(initial, target, elapsed / duration);
            yield return null;
        }

        rig.weight = target;
    }

    // 方便的快捷调用方法
    public void SwitchToAim(float speed)
    {
        SetRigWeight("AimRig", 1f, speed);
        SetRigWeight("IdleRig", 0f, speed);
        SetRigWeight("HippieRig", 0f, speed);
    }

    public void SwitchToHippie(float speed)
    {
        SetRigWeight("HippieRig", 1f, speed);
        SetRigWeight("AimRig", 0f, speed);
    }

    public void SwitchToHipFire(float speed)
    {
        SetRigWeight("AimRig", 0f, speed);
        SetRigWeight("IdleRig", 0f, speed);
        SetRigWeight("AnyWayRig", 1f, speed);
        SetRigWeight("HippieRig", 0f, speed);
    }

    public void SwitchToIdle(float speed)
    {
        SetRigWeight("AimRig", 0f, speed);
        SetRigWeight("IdleRig", 0f, speed);
        SetRigWeight("AnyWayRig", 1f, speed);
        SetRigWeight("HippieRig", 0f, speed);
    }
}