using UnityEngine;
using UnityEngine.UI;

public class SanityUIController : MonoBehaviour
{
    public Image sanityImage;
    public float threshold = 0.5f;
    public Sprite[] state1Sprites;
    public Sprite[] state2Sprites;
    public Sprite[] state3Sprites;
    public Image displayImage;
    public float frameRate = 5f;

    // 用于动画数据
    private float _frameTimer;
    private int _currentFrame;

    private enum SanityState { State1, State2, State3 }
    private SanityState _currentState = SanityState.State1;
    private SanityState _targetState = SanityState.State1;
    private float _stateChangeTimer;
    // 状态延时，例如切换状态需要持续 0.5 秒
    public float stateDelay = 0.5f;

    private float _lastSanity;

    private void Start()
    {
        if (sanityImage != null)
        {
            _lastSanity = sanityImage.fillAmount;
        }
    }

    private void Update()
    {
        if (sanityImage == null || displayImage == null)
            return;

        float currentSanity = sanityImage.fillAmount;
        bool isDecreasing = currentSanity < _lastSanity;

        // 判断目标状态
        if (isDecreasing)
        {
            _targetState = SanityState.State2;
        }
        else
        {
            _targetState = currentSanity >= threshold ? SanityState.State1 : SanityState.State3;
        }

        // 如果目标状态和当前状态不一致，则启动延时计时
        if (_targetState != _currentState)
        {
            _stateChangeTimer += Time.deltaTime;
            if (_stateChangeTimer >= stateDelay)
            {
                // 延时结束后切换状态
                _currentState = _targetState;
                ResetAnimation();
                _stateChangeTimer = 0f;
            }
        }
        else
        {
            // 如果处于同一状态，重置延时计时
            _stateChangeTimer = 0f;
        }

        // 播放动画，当前 _currentState 的动画帧
        PlayStateAnimation(_currentState);

        _lastSanity = currentSanity;
    }

    private void ResetAnimation()
    {
        _currentFrame = 0;
        _frameTimer = 0f;
    }

    private void PlayStateAnimation(SanityState state)
    {
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

        if (currentSprites == null || currentSprites.Length == 0)
            return;

        _frameTimer += Time.deltaTime;
        float frameDuration = 1f / frameRate;
        if (_frameTimer >= frameDuration)
        {
            _frameTimer = 0f;
            _currentFrame = (_currentFrame + 1) % currentSprites.Length;
        }

        displayImage.sprite = currentSprites[_currentFrame];
    }
}