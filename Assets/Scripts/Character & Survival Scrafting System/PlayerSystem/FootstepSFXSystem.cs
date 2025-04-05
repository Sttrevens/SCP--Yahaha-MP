using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FootstepSFXSystem : MonoBehaviour
{
    // 常量定义
    private const string MudTag = "Mud";
    private const string MetalTag = "Metal";
    private const float WeightThreshold = 0.5f; // 权重阈值

    [Header("Footstep")]
    [SerializeField] private AudioClip[] mudFootstepClips; // 泥土脚步声数组
    [SerializeField] private AudioClip[] metalFootstepClips; // 金属脚步声数组
    [SerializeField] private float raycastDistance = 1.1f; // 射线检测距离
    
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 通过 AnimationEvent 调用该方法。
    /// 可通过传递动画片段名称参数来标识当前动画片段，方便查找对应的clip信息
    /// </summary>
    /// <param name="clipName">调用该事件的动画片段名称</param>
    public void PlayFootstepSound(string clipName = null)
    {
        // 获取当前动画层的所有动画片段信息
        AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        foreach (var clipInfo in clipInfos)
        {
            // 当 AnimationEvent 传递了 clipName 时，只处理匹配那个动画片段的状态
            if (!string.IsNullOrEmpty(clipName) && !clipInfo.clip.name.Equals(clipName))
                continue;

            if (clipInfo.weight >= WeightThreshold)
            {
                // 若权重满足阈值，进行射线检测
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance))
                {
                    HandleFootstepSfxByTag(hit.collider.tag);
                }
                // 找到满足条件的片段后退出循环
                break;
            }
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
            AudioManager.Instance.PlaySFX(this.gameObject, clip);
        }
    }
}