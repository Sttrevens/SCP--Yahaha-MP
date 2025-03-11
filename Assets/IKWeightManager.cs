using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class IKWeightManager : MonoBehaviour
{
    public Rig rig;
    private Coroutine weightCoroutine;
    public RigBuilder rigBuilder;

    private void Awake()
    {
        if(rig == null) rig = GetComponentInChildren<Rig>();
    }

    private void Start()
    {
        rig.weight = 0f;
    }

    // 开始切换权重
    public void ChangeWeight(float targetWeight, float speed)
    {
        if (weightCoroutine != null)
            StopCoroutine(weightCoroutine);

        weightCoroutine = StartCoroutine(ChangeWeightRoutine(targetWeight, speed));
    }

    // 协程实现平滑权重切换
    private IEnumerator ChangeWeightRoutine(float targetWeight, float speed)
    {
        float initialWeight = rig.weight;
        float elapsed = 0f;
        float duration = Mathf.Abs(targetWeight - initialWeight) / speed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rig.weight = Mathf.Lerp(initialWeight, targetWeight, elapsed / duration);
            yield return null;
        }

        rig.weight = targetWeight;
    }
}