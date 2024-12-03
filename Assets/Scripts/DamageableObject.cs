using UnityEngine;

namespace LPSurvivalEngine
{
    public class DamageableObject : MonoBehaviour, IDamagable
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 100;   // 最大血量
        private int currentHealth;                       // 当前血量

        [Header("Death Settings")]
        [SerializeField] private GameObject deathEffect; // 死亡时的特效

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        // 受伤逻辑
        public void TakePhysicDamage(int damage)
        {
            currentHealth -= damage;

            // 处理物体受伤后的逻辑
            Debug.Log($"{gameObject.name} took {damage} damage!");

            // 判断是否死亡
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // 死亡逻辑
        private void Die()
        {
            // 播放死亡特效
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }

            // 销毁物体（可以根据需要修改）
            Destroy(gameObject);

            Debug.Log($"{gameObject.name} has died!");
        }

        // 获取当前血量
        public int GetCurrentHealth()
        {
            return currentHealth;
        }
    }
}
