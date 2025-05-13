using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class IdleState : EnemyBaseState
{
    private LieQController _lieQController;
    private List<GameObject> _allPlayers;
    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);
        _lieQController = enemy.GetComponent<LieQController>();
        _allPlayers = new List<GameObject>();
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            _allPlayers.Add(player);
    }

    public override void UpdateState(Enemy enemy)
    {
        if (_lieQController.isCooldown || _lieQController.isRestraining)
        {
            // 冷却或正在牵制状态下，无其他行为
            return;
        }

        if (IsPlayerInRange(enemy.transform, _lieQController.detectionRange))
{
    Debug.Log("Player is in range, checking if restrainCoroutine is null.");
    if (_lieQController.restrainCoroutine == null)
    {
        Debug.Log("Starting restrain player coroutine.");
        _lieQController.restrainCoroutine =
            _lieQController.StartCoroutine(AttemptRestrainPlayer());
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
    
    private bool IsPlayerInRange(Transform enemyTransform, float radius)
{
    foreach (GameObject player in _allPlayers)
    {
        Vector3 toPlayer = player.transform.position - enemyTransform.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > radius)
            continue;

        float horizontalAngle = Vector3.Angle(enemyTransform.forward, toPlayer);
        if (horizontalAngle > 360)
            continue;

        float verticalAngle = Vector3.Angle(enemyTransform.forward, toPlayer);
        if (verticalAngle > 360)
            continue;

        // Full cone check (combining horizontal and vertical angles)
        for (float horizontal = - 360; horizontal <= 360; horizontal += 10)
        {
            for (float vertical = - 360; vertical <= 360; vertical += 10)
            {
                Vector3 rayDirection = Quaternion.Euler(vertical, horizontal, 0) * enemyTransform.forward;

                RaycastHit hit;
                if (Physics.Raycast(enemyTransform.position + Vector3.up * 1f, rayDirection, out hit, radius))
                {
                    Debug.DrawLine(enemyTransform.position + Vector3.up * 1f, hit.point, Color.red, 0.1f);

                    if (hit.collider.gameObject == player)
                    {
                        //Debug.Log("[EnemyAI] Player detected via multiple rays!");
                        _lieQController.targetPlayer = player;
                        return true;
                    }
                }
            }
        }
    }
    //Debug.Log($"No player detected within range: {radius} at position: {enemyTransform.position}");
    return false;
}

    private IEnumerator AttemptRestrainPlayer()
    {
        int checkCount = 0;
        const int requiredChecks = 5;
    
        while (checkCount < requiredChecks)
        {
            if (!IsPlayerInRange(_lieQController.transform, _lieQController.detectionRange))
            {
                _lieQController.restrainCoroutine = null;
                yield break;
            }
    
            checkCount++;
            yield return new WaitForSeconds(1f);
        }
    
        _lieQController.RestrainPlayer();
        _lieQController.restrainCoroutine = null;
    }
}
