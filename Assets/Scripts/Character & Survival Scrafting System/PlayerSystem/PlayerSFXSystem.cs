using UnityEngine;
using Fusion;
using LPSurvivalEngine;

public class PlayerSFXSystem : NetworkBehaviour
{
    // 常量定义
    private const string MudTag = "Mud";
    private const string MetalTag = "Metal";

    [Header("Footstep")]
    [SerializeField] private AudioClip[] mudFootstepClips; // 泥土脚步声数组
    [SerializeField] private AudioClip[] metalFootstepClips; // 金属脚步声数组
    [SerializeField] private float raycastDistance = 1.1f; // 射线检测距离

    [Header("Health-related")]
    [SerializeField] private AudioClip[] dyingClips;
    [SerializeField] private AudioClip[] scaredClips;
    [SerializeField] private AudioClip[] tiredClips;

    public HealthSystem healthSystem;
    private AudioSource audioSource;

    private void Start()
    {
        InitializeAudioSource();
    }

    private void Update()
    {
        if (!HasStateAuthority) return;

        HandleHealthStateAudio(healthSystem.isDying, PlayDyingAudio, true);
        HandleHealthStateAudio(healthSystem.isScared, Rpc_PlayScaredAudio, false);
        HandleHealthStateAudio(healthSystem.isTired, Rpc_PlayTiredAudio, false);
    }

    private void HandleHealthStateAudio(bool condition, System.Action PlayMethod, bool stopIfFalse)
    {
        if (condition)
        {
            PlayMethod();
        }
        else if (stopIfFalse)
        {
            Rpc_StopAudio();
        }
    }
    
    public void PlayDyingAudio()
    {
        PlayAudioClip(dyingClips);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_PlayScaredAudio()
    {
        PlayAudioClip(scaredClips);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_PlayTiredAudio()
    {
        PlayAudioClip(tiredClips);
    }

    private void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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

    private void PlayAudioClip(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            if (audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void Rpc_StopAudio()
    {
        audioSource.loop = false;
if (audioSource.isPlaying)
{
    audioSource.SetScheduledEndTime(AudioSettings.dspTime + audioSource.clip.length - audioSource.time);
}
else
{
    audioSource.Stop();
    audioSource.clip = null;
}
    }
}