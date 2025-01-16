using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    public enum EnemyState { Idle, Attack }
    [SerializeField]
    private GameObject[] players;

    public EnemyState currentState;
    public Transform target;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 10f;
    public float detectionRange = 10f;

    private RootBaseState currentStateBehavior;
    
    [Header("Patrol")]
    public float fieldOfViewAngleHorizontal = 90f;
    public float fieldOfViewAngleVertical = 60f;
    public float sensingRadius = 0.1f;
    
    [HideInInspector] public GameObject targetPlayer;

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            target = players[0].transform;
        }

        SwitchState(new RootIdleState());
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentStateBehavior?.UpdateState(this);
            players = GameObject.FindGameObjectsWithTag("Player");
            //the first player
            if (players.Length > 0)
            {
                target = players[0].transform;
            }
        }
        
    }

    public void SwitchState(RootBaseState newState)
    {
        currentStateBehavior?.ExitState(this);
        currentStateBehavior = newState;
        currentStateBehavior.EnterState(this);
    }

    public bool PlayerInSight()
    {
        foreach (GameObject player in players)
        {
            Vector3 toPlayer = player.transform.position - transform.position;
            Vector3 horizontalToPlayer = Vector3.ProjectOnPlane(toPlayer, Vector3.up);

            float horizontalAngle = Vector3.Angle(transform.forward, horizontalToPlayer);
            float verticalAngle = Vector3.Angle(toPlayer, horizontalToPlayer);

            if (horizontalAngle < fieldOfViewAngleHorizontal / 2 && verticalAngle < fieldOfViewAngleVertical / 2)
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, sensingRadius, toPlayer.normalized, out hit, detectionRange))
                {
                    if (hit.collider.gameObject == player)
                    {
                        targetPlayer = player;
                        return true;
                    }
                }

                float distanceToPlayer = toPlayer.magnitude;
                if (Vector3.Angle(transform.forward, horizontalToPlayer) <= fieldOfViewAngleHorizontal / 2)
                {
                    if (distanceToPlayer <= detectionRange)
                    {
                        if (Physics.CheckSphere(player.transform.position, sensingRadius))
                        {
                            targetPlayer = player;
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    
}
