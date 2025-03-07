using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPSurvivalEngine;
using Fusion;

public class PossessEffect : NetworkBehaviour
{
    public Enemy enemy;

    private ChasingEnemy _chasingEnemy;
    
    // Start is called before the first frame update
    void Start()
    {
        _chasingEnemy = enemy.GetComponent<ChasingEnemy>();
    }

    public void Possess(int possessTime)
    {
        PlayAttackSFX(enemy);
        RpcPossess(possessTime);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcPossess(int possessTime)
    {
        
        if (_chasingEnemy.targetPlayer != null)
        {
            
            StartCoroutine(PossessDuration(possessTime));
        }
    }
    

    private IEnumerator PossessDuration(int possessTime)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < possessTime)
        {
            if (_chasingEnemy.targetPlayer != null)
            {
                _chasingEnemy.targetPlayer.GetComponent<PlayerMovement>().BePossessed(transform);
                _chasingEnemy.targetPlayer.transform.localPosition = Vector3.zero;
                _chasingEnemy.targetPlayer.transform.localRotation = Quaternion.identity;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _chasingEnemy.targetPlayer.GetComponent<PlayerMovement>().BePossessed(null);
        EnterCD();
    }

    void PlayAttackSFX(Enemy enemy)
    {
        if (enemy.sfxClips != null)
        {
            foreach (var clip in enemy.sfxClips)
            {
                if (clip.label == "Bite")
                {
                    AudioManager.instance.PlaySFX(enemy.gameObject, clip.clip);
                    break;
                }
            }
        }
    } 

    public void EnterCD()
    {
        StartCoroutine(Wait(3f));
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        enemy.SwitchState(new PatrollingState());
    }

    public void EnterPossessing()
    {
        RpcEnterPossess();
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcEnterPossess()
    {
        
        StartCoroutine(Possessing());
    }
    
    IEnumerator Possessing()
    {
        _chasingEnemy.agent.speed = _chasingEnemy.patrollingSpeed;
        if (_chasingEnemy.patrolMode == EnemyMovement.PatrolMode.FixedPoints)
        {
            // 如果目标点与目前位置的距离没有达到阈值就继续往这个目标巡航 如果距离足够近（进入到半径范围），就更新下一个巡航点
            if (Vector3.Distance(_chasingEnemy.transform.position, _chasingEnemy.patrolPoints[_chasingEnemy.currentPatrolIndex].position) <
                _chasingEnemy.agent.stoppingDistance)
            {
                Debug.Log(
                    $"[EnemyAI] Reached patrol point {_chasingEnemy.currentPatrolIndex}, waiting {_chasingEnemy.waitTimeAtPatrolPoint} seconds");
                _chasingEnemy.enemy._animatorManager.isPatrolling = false;
                yield return new WaitForSeconds(_chasingEnemy.waitTimeAtPatrolPoint);
                _chasingEnemy.enemy._animatorManager.isPatrolling = true;
                _chasingEnemy.currentPatrolIndex = (_chasingEnemy.currentPatrolIndex + 1) % _chasingEnemy.patrolPoints.Length;
                Debug.Log($"[EnemyAI] Moving to next patrol point {_chasingEnemy.currentPatrolIndex}");
            }

            _chasingEnemy.agent.SetDestination(_chasingEnemy.patrolPoints[_chasingEnemy.currentPatrolIndex].position);
        }
        //如果是随机巡航
        else if (_chasingEnemy.patrolMode == EnemyMovement.PatrolMode.RandomCircle || _chasingEnemy.patrolMode == EnemyMovement.PatrolMode.RandomRectangle)
        {
            if (Vector3.Distance(_chasingEnemy.transform.position, _chasingEnemy.agent.destination) < _chasingEnemy.agent.stoppingDistance)
            {
                Debug.Log("[EnemyAI] Reached random patrol point, waiting before next point");
                _chasingEnemy.enemy._animatorManager.isPatrolling = false;
                yield return new WaitForSeconds(_chasingEnemy.waitTimeAtPatrolPoint);
                _chasingEnemy.enemy._animatorManager.isPatrolling = true;
                Vector3 randomPatrolPoint = _chasingEnemy.GetRandomPatrolPoint();
                _chasingEnemy.agent.SetDestination(randomPatrolPoint);
                Debug.Log($"[EnemyAI] Moving to new random patrol point at {randomPatrolPoint}");
            }
        }
        
        yield return null;
    }
}
