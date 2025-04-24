using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TaskUI : MonoBehaviour
{
    [Header("UI 引用")]
    public Text donorNameText;
    public Text taskNameText;
    public Text taskDescriptionText;
    public Text rewardText;
    public TMP_Text timeRemainingText;
    public Image timerFillImage;
    
    [Header("动画设置")]
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;
    public float completionFlashDuration = 0.3f;
    public Color completionColor = Color.green;
    public Color failureColor = Color.red;
    
    private BountyTaskManager.ActiveTask _task;
    private CanvasGroup _canvasGroup;
    private Coroutine _timerCoroutine;
    private Color _originalTimerColor;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        _originalTimerColor = timerFillImage.color;
        _canvasGroup.alpha = 0f;
        
        StartCoroutine(FadeIn());
    }
    
    public void SetupTask(BountyTaskManager.ActiveTask task)
    {
        _task = task;
        
        // 设置UI文本
        donorNameText.text = task.donorName;
        taskNameText.text = task.taskName;
        taskDescriptionText.text = task.taskDescription;
        rewardText.text = $"{task.rewardAmount:F0} 金币";
        
        // 启动计时器
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }
        _timerCoroutine = StartCoroutine(UpdateTimer());
        
        // 订阅任务事件
        BountyTaskManager.Instance.OnTaskCompleted += OnTaskCompleted;
        BountyTaskManager.Instance.OnTaskFailed += OnTaskFailed;
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件
        if (BountyTaskManager.Instance != null)
        {
            BountyTaskManager.Instance.OnTaskCompleted -= OnTaskCompleted;
            BountyTaskManager.Instance.OnTaskFailed -= OnTaskFailed;
        }
    }
    
    private IEnumerator UpdateTimer()
    {
        while (_task != null && _task.remainingTime > 0)
        {
            // 更新时间文本
            timeRemainingText.text = $"{Mathf.CeilToInt(_task.remainingTime)}s";
            
            // 更新计时器填充
            timerFillImage.fillAmount = _task.remainingTime / _task.timeLimit;
            
            // 时间少于30%时闪烁
            if (_task.remainingTime / _task.timeLimit < 0.3f)
            {
                timerFillImage.color = Color.Lerp(_originalTimerColor, Color.red, 
                    Mathf.PingPong(Time.time * 2f, 1f));
            }
            
            yield return null;
        }
    }
    
    private void OnTaskCompleted(BountyTaskManager.ActiveTask task)
    {
        if (_task.id == task.id)
        {
            StopTimer();
            StartCoroutine(ShowCompletionFlash(true));
        }
    }
    
    private void OnTaskFailed(BountyTaskManager.ActiveTask task)
    {
        if (_task.id == task.id)
        {
            StopTimer();
            StartCoroutine(ShowCompletionFlash(false));
        }
    }
    
    private void StopTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }
    
    private IEnumerator ShowCompletionFlash(bool success)
    {
        Color flashColor = success ? completionColor : failureColor;
        
        // 闪烁效果
        for (float t = 0; t < completionFlashDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / completionFlashDuration;
            float intensity = Mathf.PingPong(normalizedTime * 2f, 1f);
            
            timerFillImage.color = Color.Lerp(_originalTimerColor, flashColor, intensity);
            
            yield return null;
        }
        
        // 显示最终状态
        timeRemainingText.text = success ? "DONE" : "FAILED";
        timerFillImage.color = flashColor;
        
        // 淡出
        yield return new WaitForSeconds(1f);
        StartCoroutine(FadeOut());
    }
    
    private IEnumerator FadeIn()
    {
        for (float t = 0; t < fadeInDuration; t += Time.deltaTime)
        {
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }
        _canvasGroup.alpha = 1f;
    }
    
    private IEnumerator FadeOut()
    {
        for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
        {
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }
        Destroy(gameObject);
    }
}