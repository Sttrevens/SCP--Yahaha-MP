using UnityEngine;
using Fusion;

public class FootstepController : NetworkBehaviour
{
    public AudioClip[] mudFootstepClips; // 泥土脚步声数组
    public AudioClip[] metalFootstepClips; // 金属脚步声数组
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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_PlayFootstep()
    {
        PlayFootstep();
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
                PlaySound(mudFootstepClips);
            }
            else if (hit.collider.CompareTag("Metal"))
            {
                PlaySound(metalFootstepClips);
            }
        }
    }

    void PlaySound(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
}