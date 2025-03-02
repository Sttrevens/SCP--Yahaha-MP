using UnityEngine;

namespace LPSurvivalEngine
{
    public class DamageableObject : MonoBehaviour, IDamagable
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 100;   // ���Ѫ��
        private int currentHealth;                       // ��ǰѪ��

        [Header("Death Settings")]
        [SerializeField] private GameObject deathEffect; // ����ʱ����Ч

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        // �����߼�
        public void Rpc_TakePhysicDamage(int damage)
        {
            currentHealth -= damage;

            // �����������˺���߼�
            Debug.Log($"{gameObject.name} took {damage} damage!");

            // �ж��Ƿ�����
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // �����߼�
        private void Die()
        {
            // ����������Ч
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }

            // �������壨���Ը�����Ҫ�޸ģ�
            Destroy(gameObject);

            Debug.Log($"{gameObject.name} has died!");
        }

        // ��ȡ��ǰѪ��
        public int GetCurrentHealth()
        {
            return currentHealth;
        }
    }
}
