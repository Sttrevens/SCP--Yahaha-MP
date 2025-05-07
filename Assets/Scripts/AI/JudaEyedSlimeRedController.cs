using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.Serialization;

public class JudaEyedSlimeRedController : NetworkBehaviour
{
    private ChasingEnemy _chasingEnemy;
    [HideInInspector] public float originalDetectionRange;
    private Enemy _enemy;
    public GameObject target;
    
    public float detectionDistance = 25f;

    // Start is called before the first frame update
    void Start()
    {
        _chasingEnemy = GetComponent<ChasingEnemy>();
        _enemy = GetComponent<Enemy>();
        
        originalDetectionRange = _chasingEnemy.detectionRange;
    }

    public override void Spawned()
    {
        
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (_enemy.CurrentState is PatrollingState)
        {
            _chasingEnemy.detectionRange = 0;
            FindBrightestLightInDetectionRange();
        }
        else
        {
            _chasingEnemy.detectionRange = originalDetectionRange;
        }
    }

    private void FindBrightestLightInDetectionRange()
    {
        // 找到所有光源
        Light[] allLights = FindObjectsOfType<Light>();
        Transform brightestLight = null;
        float maxWeightedBrightness = float.MinValue;

        foreach (var light in allLights)
        {
            // 排除自身和子物体可在这里处理
            if (light.transform == transform || light.transform.IsChildOf(transform)) 
                continue;

            // 判断距离
            float distance = Vector3.Distance(transform.position, light.transform.position);
            if (distance <= detectionDistance)
            {
                // 计算亮度权重
                float weightedBrightness = light.intensity / distance;
                if (weightedBrightness > maxWeightedBrightness)
                {
                    maxWeightedBrightness = weightedBrightness;
                    brightestLight = light.transform;
                }
            }
        }


if (brightestLight != null)
{
    Debug.Log("Brightest Light: " + brightestLight.name); // Print the brightest light
    _chasingEnemy.agent.SetDestination(brightestLight.position);
}
    }

    public void Ahhh(GameObject target)
    {
        if (!(_enemy.CurrentState is PatrollingState))
        {
            return;
        }
        
        Rpc_Ahhh();
        this.target = target;
        transform.rotation = Quaternion.LookRotation(this.target.transform.position - transform.position);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_Ahhh()
    {
        PlayGetHitSfX(_enemy);
        _enemy.animator.SetTrigger("GetHit");
    }
    
    void PlayGetHitSfX(Enemy enemy)
    {
        if (enemy.sfxClips != null)
        {
            foreach (var clip in enemy.sfxClips)
            {
                if (clip.label == "GetHit")
                {
                    AudioManager.instance.PlaySFX(enemy.gameObject, clip.clip);
                    break;
                }
            }
        }
    }
}
