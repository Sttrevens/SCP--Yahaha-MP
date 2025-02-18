using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    [Header("碰撞音效设置")]
    public AudioClip collisionSound;            // 碰撞音效
    public float minImpactForce = 1f;          // 最小碰撞力阈值（小于此值不播放）
    public float maxImpactForce = 10f;         // 最大碰撞力阈值（最大音量对应的力度值）
    public float minVolume = 0.1f;             // 音效的最小音量
    public float maxVolume = 1f;               // 音效的最大音量

    private void OnCollisionEnter(Collision collision)
    {
        // 获取当前碰撞的力度
        float collisionForce = collision.relativeVelocity.magnitude;

        // 如果碰撞力度小于最小阈值，跳过
        if (collisionForce < minImpactForce) return;

        // 计算音量大小并限制在范围内
        float volume = Mathf.InverseLerp(minImpactForce, maxImpactForce, collisionForce / 2);
        volume = Mathf.Clamp(volume, minVolume, maxVolume);

        AudioManager.instance.PlaySFX(this.gameObject, collisionSound, volume);
    }
}