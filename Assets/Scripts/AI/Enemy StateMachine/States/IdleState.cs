using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class IdleState : EnemyBaseState
{
    private LieQController _lieQController;
    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);
        _lieQController = enemy.GetComponent<LieQController>();
    }

    public override void UpdateState(Enemy enemy)
    {
        if (_lieQController.isCooldown || _lieQController.isRestraining)
        {
            // 冷却或正在牵制状态下，无其他行为
            return;
        }

        if (IsPlayerInRange(enemy.transform.position, _lieQController.detectionRange))
{
    Debug.Log("Player is in range, checking if restrainCoroutine is null.");
    if (_lieQController.restrainCoroutine == null)
    {
        Debug.Log("Starting restrain player coroutine.");
        _lieQController.restrainCoroutine =
            _lieQController.StartCoroutine(_lieQController.RestrainPlayer());
    }
    else
    {
        Debug.Log("Restrain coroutine is already running.");
    }
}
    }

    public override void ExitState(Enemy enemy)
    {
       
    }
    
    private bool IsPlayerInRange(Vector3 position, float radius)
{
    int playerLayerMask = LayerMask.GetMask("Player");
    Collider[] hits = Physics.OverlapSphere(position, radius, playerLayerMask);
    foreach (var hit in hits)
    {
        Debug.Log("hit something: " + hit.name);
        if (hit.CompareTag("Player"))
        {
            Debug.Log($"Player detected within range: {radius} at position: {position}");
            _lieQController.targetPlayer = hit.gameObject;
            return true;
        }
    }
    Debug.Log($"No player detected within range: {radius} at position: {position}");
    return false;
}
}
