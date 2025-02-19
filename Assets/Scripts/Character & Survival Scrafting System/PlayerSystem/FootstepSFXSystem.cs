using UnityEngine;
using Fusion;

public class FootstepSFXSystem : NetworkBehaviour
{
    // 常量定义
    private const string MudTag = "Mud";
    private const string MetalTag = "Metal";

    [Header("Footstep")]
    [SerializeField] private AudioClip[] mudFootstepClips; // 泥土脚步声数组
    [SerializeField] private AudioClip[] metalFootstepClips; // 金属脚步声数组
    [SerializeField] private float raycastDistance = 1.1f; // 射线检测距离

    private AudioSource audioSource;

    private void Start()
    {
        InitializeAudioSource();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_PlayFootstep()
    {
        PlayFootstepSound();
    }

    // 动画事件调用
    public void PlayFootstepSound()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            HandleFootstepSfxByTag(hit.collider.tag);
        }
    }

    private void HandleFootstepSfxByTag(string tag)
    {
        if (tag == MudTag)
        {
            PlaySfx(mudFootstepClips);
        }
        else if (tag == MetalTag)
        {
            PlaySfx(metalFootstepClips);
        }
    }

    private void PlaySfx(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(clip);
        }
    }

    private void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
}