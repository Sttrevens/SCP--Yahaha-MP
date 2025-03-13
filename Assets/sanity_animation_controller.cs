using UnityEngine;
using UnityEngine.UI;

public class SanityUIController : MonoBehaviour
{
    [Header("用于监测 fillAmount 的 Image（如理智值条）")]
    public Image sanityImage;

    [Header("理智值阈值(用于区分状态1和状态3)")]
    [Range(0f, 1f)]
    public float threshold = 0.5f;

    [Header("状态1(大于等于阈值时)播放的图片集")]
    public Sprite[] state1Sprites;

    [Header("状态2(正在减少时)播放的图片集")]
    public Sprite[] state2Sprites;

    [Header("状态3(小于阈值时)播放的图片集")]
    public Sprite[] state3Sprites;

    [Header("用于显示动画帧的目标UI Image")]
    public Image displayImage;

    [Header("每秒播放几帧(越大播放越快)")]
    public float frameRate = 5f;

    // 记录当前理智值
    private float _currentSanity;
    // 记录上一次理智值
    private float _lastSanity;

    // 记录动画播放用的时间和帧索引
    private float _frameTimer;
    private int _currentFrame;

    // 三种状态的枚举
    private enum SanityState { State1, State2, State3 }
    private SanityState _currentState = SanityState.State1;

    private void Start()
    {
        if (sanityImage != null)
        {
            _currentSanity = sanityImage.fillAmount;
            _lastSanity = _currentSanity;
        }
    }

    private void Update()
    {
        if (sanityImage == null || displayImage == null) return;

        // 1. 获取当前和上一次 fillAmount
        _currentSanity = sanityImage.fillAmount;

        // 2. 判定当前状态
        bool isDecreasing = _currentSanity < _lastSanity;

        if (isDecreasing)
        {
            _currentState = SanityState.State2;  // 正在减少
        }
        else
        {
            // 不在减少时，判断与阈值的关系
            if (_currentSanity >= threshold)
                _currentState = SanityState.State1;
            else
                _currentState = SanityState.State3;
        }

        // 如果状态变化了，重置帧索引和计时，让动画从头开始
        // （也可根据需求决定是否从头开始播放）
        // 这里举例：状态变化则重置
        if (_currentState != GetStateBySanity(_lastSanity))
        {
            _currentFrame = 0;
            _frameTimer = 0f;
        }

        // 3. 播放对应状态下的动画帧
        PlayStateAnimation(_currentState);

        // 4. 记录本帧结束时的 fillAmount，以及当前状态
        _lastSanity = _currentSanity;
    }

    /// <summary>
    /// 根据给定的 sanity 值判断当时可能的状态(用于比较上一次状态).
    /// </summary>
    private SanityState GetStateBySanity(float sanityValue)
    {
        // 这只是为了和当前状态做对比，推断上一次的状态
        // 不考虑减少/增加，因为上一帧已经固定了
        return sanityValue >= threshold ? SanityState.State1 : SanityState.State3;
    }

    /// <summary>
    /// 播放当前状态对应的动画帧
    /// </summary>
    private void PlayStateAnimation(SanityState state)
    {
        // 根据状态选择 Sprite 数组
        Sprite[] currentSprites = null;
        switch (state)
        {
            case SanityState.State1:
                currentSprites = state1Sprites;
                break;
            case SanityState.State2:
                currentSprites = state2Sprites;
                break;
            case SanityState.State3:
                currentSprites = state3Sprites;
                break;
        }

        // 如果没有设置对应的 Sprite 数组，直接返回
        if (currentSprites == null || currentSprites.Length == 0) return;

        // 更新计时器并判断是否切换帧
        _frameTimer += Time.deltaTime;
        float frameDuration = 1f / frameRate;
        if (_frameTimer >= frameDuration)
        {
            _frameTimer = 0f;
            _currentFrame++;

            // 如果超过数组长度则循环回到第0帧
            if (_currentFrame >= currentSprites.Length)
                _currentFrame = 0;
        }

        // 根据当前帧索引设置 displayImage 的 Sprite
        displayImage.sprite = currentSprites[_currentFrame];
    }
}