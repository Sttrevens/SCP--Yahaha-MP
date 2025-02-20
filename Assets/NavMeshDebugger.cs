using UnityEngine;
using UnityEngine.AI;
using Fusion;

public class NavMeshDebugger : NetworkBehaviour
{
    private NavMeshPath path;     // 用于存储路径
    private NavMeshAgent agent;   // NavMesh代理组件

    public Transform targetPosition;
    
    void Start()
    {
        // 初始化路径
        path = new NavMeshPath();

        // 获取 NavMeshAgent（若没有直接挂载该组件需要手动添加）
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("No NavMeshAgent attached to the GameObject.");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetComponent<ChasingEnemy>().targetPlayer != null)
        {
            targetPosition = GetComponent<ChasingEnemy>().targetPlayer.transform;
        }
        
        if (agent != null && targetPosition != null)
        {
            // 每帧检查从当前位置到目标的路径状态
            NavMesh.CalculatePath(transform.position, targetPosition.position, NavMesh.AllAreas, path);

            // 检查路径状态
            switch (path.status)
            {
                case NavMeshPathStatus.PathComplete:
                    Debug.Log("Path is complete, target is reachable.");
                    break;

                case NavMeshPathStatus.PathPartial:
                    Debug.Log("Path is partial, target is partially reachable.");
                    break;

                case NavMeshPathStatus.PathInvalid:
                    Debug.Log("Path is invalid, target is not reachable.");
                    break;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            Object.RequestStateAuthority();
        }
    }
}