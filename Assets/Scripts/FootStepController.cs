using UnityEngine;

public class FootstepController : MonoBehaviour
{
    public AudioClip mudFootstepClip; // 泥土脚步声
    public AudioClip metalFootstepClip; // 金属脚步声
    public float raycastDistance = 1.1f; // 射线检测距离

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // 动画事件调用的函数
    public void PlayFootstep()
    {
        Debug.Log("开始播放脚步声喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵喵");
        // 发射射线检测地面类型
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            // 根据地面 Tag 播放不同的脚步声
            if (hit.collider.CompareTag("Mud"))
            {
                PlaySound(mudFootstepClip);
            }
            else if (hit.collider.CompareTag("Metal"))
            {
                PlaySound(metalFootstepClip);
            }
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}