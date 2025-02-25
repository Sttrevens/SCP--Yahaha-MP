using UnityEngine;

public class RootAttackState : EnemyBaseState
{
    private float throwCooldown;

    public override void EnterState(Enemy enemy)
    {
        base.EnterState(enemy);
        
        throwCooldown = 0f;

        // Start the shaking effect 
        EnemyShake shakeComponent = enemy.GetComponent<EnemyShake>();
        if (shakeComponent != null)
        {
            shakeComponent.StartShake();
        }
    }

    public override void UpdateState(Enemy enemy)
    {
        /*if (!enemy.PlayerInSight())
        {
            enemy.SwitchState(new RootIdleState());
            return;
        }

        throwCooldown -= Time.deltaTime;
        if (throwCooldown <= 0f)
        {
            ThrowProjectile(enemy);
            throwCooldown = 1f; // Cooldown between throws
        }*/
    }

    private void ThrowProjectile(Enemy enemy)
    {
        /*GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        projectile.transform.position = enemy.projectileSpawnPoint.position;
        projectile.transform.localScale = Vector3.one * 0.2f;

        Rigidbody rb = projectile.AddComponent<Rigidbody>();
        if (rb != null && enemy.targetPlayer != null)
        {
            Vector3 direction = (enemy.targetPlayer.transform.position - enemy.projectileSpawnPoint.position).normalized;
            rb.velocity = direction * enemy.projectileSpeed;
        }

        Object.Destroy(projectile, 5f);*/
    }

    public override void ExitState(Enemy enemy)
    {
        // Stop the shaking effect
        EnemyShake shakeComponent = enemy.GetComponent<EnemyShake>();
        if (shakeComponent != null)
        {
            shakeComponent.StopShake();
        }
    }
}