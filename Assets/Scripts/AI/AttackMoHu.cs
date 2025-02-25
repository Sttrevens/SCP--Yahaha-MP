using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMoHu : MonoBehaviour
{
    public Enemy enemy;

    private ChasingEnemy _chasingEnemy;

    private EnemyAttack _enemyAttack;
    // Start is called before the first frame update
    void Start()
    {
        _chasingEnemy = enemy.GetComponent<ChasingEnemy>();
        _enemyAttack = enemy.GetComponent<EnemyAttack>();
    }

    private void MoHu()
    {
        Debug.Log("MoHumiaosdadsadsadsaughdfsyaugfytasufgauysf");
        PlayMoHuForSecond(5.0f);
    }

    private void PlayMoHuForSecond(float seconds)
    {
        StartCoroutine(PlayPostProcessing(seconds));
    }
    IEnumerator PlayPostProcessing(float seconds)
    {
        GlobalPostProcessing.instance.ChangeMohuState(true);
        yield return new WaitForSeconds(seconds);
        GlobalPostProcessing.instance.ChangeMohuState(false);
    }
    
}
