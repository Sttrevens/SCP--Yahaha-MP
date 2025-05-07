using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using LPSurvivalEngine;
using UnityEngine.Serialization;
using VLB;

public class ChasingEnemy : EnemyMovement
{
    public GameObject targetPlayer;
    [HideInInspector] public Vector3 soundTargetPosition;
    public float chasingSpeed = 5f;
    public float hearingRadius = 10f;
    public IEnemyState StateAfterChasing;
    
    [SerializeField] private string stateAfterChasingString; // 在Inspector中设置

    [SerializeField] private AudioClip[] chasingSfXs;
    [SerializeField] private Vector2 chasingSfXTimeRange;
    private Coroutine _soundEffectCoroutine;
    
    private Dictionary<string, IEnemyState> stateDictionary 
        = new Dictionary<string, IEnemyState>
        {
            { "PossessingState", new PossessingState() },
            { "IdleState", new IdleState() }
            // 也可以用反射或手动填更多映射
        };

    private void Start()
    {
        if (stateDictionary.TryGetValue(stateAfterChasingString, out IEnemyState foundState))
        {
            StateAfterChasing = foundState;
        }
        else
        {
            StateAfterChasing = new AttackingState();
        }
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (PlayerInSight() && enemy.CurrentState is PatrollingState)
        {
            Debug.Log("[EnemyAI] Player detected while patrolling, switching to chase state");
            enemy.SwitchState(new ChasingState());
            agent.speed = chasingSpeed;
        }

        if (enemy.CurrentState is ChasingState)
        {
            if (chasingSfXs != null && chasingSfXs.Length > 0 && _soundEffectCoroutine == null)
            {
                _soundEffectCoroutine = StartCoroutine(PlayRandomSoundEffects());
            }
        }
        else if (_soundEffectCoroutine != null)
        {
            StopSoundEffectCoroutine();
        }
    }
    
    public float stepAngle = 5f; // 多射线之间的步进角度

    [Header( "Gizmos" )]
    public bool drawGizmos = false; // Exposed variable to control drawing

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
    Vector3 rayStartPosition = transform.position + Vector3.up * 1f; // Raise the origin point by 1 unit

    if (targetPlayer != null)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayStartPosition, targetPlayer.transform.position);
    }

    Gizmos.color = Color.yellow;

    // Full cone rays (both horizontal and vertical angles)
    for (float horizontal = -fieldOfViewAngleHorizontal / 2; horizontal <= fieldOfViewAngleHorizontal / 2; horizontal += stepAngle)
    {
        for (float vertical = -fieldOfViewAngleVertical / 2; vertical <= fieldOfViewAngleVertical / 2; vertical += stepAngle)
        {
            Vector3 rayDirection = Quaternion.Euler(vertical, horizontal, 0) * transform.forward;
            Gizmos.DrawRay(rayStartPosition, rayDirection * detectionRange);
        }
    }
}

    /// <summary>
    /// 判断是否有玩家在怪物的视野中
    /// </summary>
    /// <returns>玩家是否在视野中</returns>
public bool PlayerInSight()
{
    if (targetPlayer != null)
    {
        if (targetPlayer.tag != "Player" || targetPlayer.GetComponent<HealthSystem>().isDeadNetworked)
        {
            targetPlayer = null;
            return false;
        }
    }

    Debug.Log("[EnemyAI] Checking for players in sight...");

    foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > detectionRange)
            continue;

        float horizontalAngle = Vector3.Angle(transform.forward, toPlayer);
        if (horizontalAngle > fieldOfViewAngleHorizontal / 2f)
            continue;

        float verticalAngle = Vector3.Angle(transform.forward, toPlayer);
        if (verticalAngle > fieldOfViewAngleVertical / 2f)
            continue;

        // Full cone check (combining horizontal and vertical angles)
        for (float horizontal = -fieldOfViewAngleHorizontal / 2; horizontal <= fieldOfViewAngleHorizontal / 2; horizontal += stepAngle)
        {
            for (float vertical = -fieldOfViewAngleVertical / 2; vertical <= fieldOfViewAngleVertical / 2; vertical += stepAngle)
            {
                Vector3 rayDirection = Quaternion.Euler(vertical, horizontal, 0) * transform.forward;

                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 1f, rayDirection, out hit, detectionRange))
                {
                    Debug.DrawLine(transform.position + Vector3.up * 1f, hit.point, Color.red, 0.1f);

                    if (hit.collider.gameObject == player)
                    {
                        Debug.Log("[EnemyAI] Player detected via multiple rays!");
                        targetPlayer = player;
                        return true;
                    }
                }
            }
        }
    }

    Debug.Log("[EnemyAI] No players detected in sight");
    return false;
}

    private IEnumerator PlayRandomSoundEffects()
    {
        while (true)
        {
            float waitTime = Random.Range(chasingSfXTimeRange.x, chasingSfXTimeRange.y);
            yield return new WaitForSeconds(waitTime);
            
            if (chasingSfXs != null && chasingSfXs.Length > 0)
            {
                AudioClip randomClip = chasingSfXs[Random.Range(0, chasingSfXs.Length)];
                AudioManager.instance.PlaySFX(this.gameObject, randomClip);
            }
        }
    }
    
    private void StopSoundEffectCoroutine()
    {
        if (_soundEffectCoroutine != null)
        {
            StopCoroutine(_soundEffectCoroutine);
            _soundEffectCoroutine = null;
        }
    }
    
    public bool PlayerHeard()
    {
        Debug.Log("[EnemyAI] Checking for players by sound...");
        Collider[] colliders = Physics.OverlapSphere(transform.position, hearingRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player") && collider.transform.Find("Model").GetComponent<AudioSource>().isPlaying)
            {
                Debug.Log("[EnemyAI] Player detected by sound");
                soundTargetPosition = collider.transform.position;
                return true;
            }
        }
        Debug.Log("[EnemyAI] No players detected by sound");
        return false;
    }
}
