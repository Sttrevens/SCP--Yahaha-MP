using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MinigameController : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("小游戏持续时间，单位：秒")]
    public float gameDuration = 90f;   
    [Tooltip("掉落物生成间隔")]
    public float dropInterval = 1f;
    [Tooltip("玩家左右移动速度")]
    public float playerMoveSpeed = 500f;

    [Header("Item Prefabs")]
    [Tooltip("好物品预制体")]
    public GameObject goodItemPrefab;
    [Tooltip("坏物品预制体")]
    public GameObject badItemPrefab;

    [Header("UI References")]
    [Tooltip("用于生成物品的区域，RectTransform")]
    public RectTransform dropArea;
    [Tooltip("玩家的RectTransform")]
    public RectTransform playerRect;
    [Tooltip("用于显示分数的文本")]
    public TextMeshProUGUI scoreText;
    [Tooltip("用于显示剩余时间的文本")]
    public TextMeshProUGUI timeText;
    [Tooltip("结束结果面板")]
    public GameObject resultPanel;
    [Tooltip("在结束面板里显示结果文本")]
    public TextMeshProUGUI resultText;
    public System.Action<int> onMinigameEnd;
    public GameObject minigamepanel;
    private float timer;         
    private float dropTimer;     
    private int score;           
    private bool gameActive;     

    
    void OnEnable()
    {
        
        StartMinigame();
    }


    public void StartMinigame()
    {
        timer = 0f;
        dropTimer = 0f;
        score = 0;
        gameActive = true;

      
        scoreText.text = "Score: 0";
        timeText.text = "Time: 1:30";
        if (resultPanel != null)
            resultPanel.SetActive(false);

        
        if (playerRect != null)
        {
            Vector2 startPos = new Vector2(0f, -dropArea.rect.height / 2 + 100f);
            playerRect.anchoredPosition = startPos;
        }
    }

    void Update()
    {
        if (!gameActive) return;

        
        timer += Time.deltaTime;
        dropTimer += Time.deltaTime;

       
        float remain = gameDuration - timer;
        if (remain < 0f) remain = 0f;
        UpdateTimeText(remain);

       
        if (timer >= gameDuration)
        {
            EndMinigame();
            return;
        }

        if (dropTimer >= dropInterval)
        {
            SpawnItem();
            dropTimer = 0f;
        }

        
        MovePlayer();

       
        CheckItemCollisions();
    }

    private void UpdateTimeText(float remain)
    {
        int intRemain = Mathf.CeilToInt(remain);
        int minutes = intRemain / 60;
        int seconds = intRemain % 60;
        if (timeText != null)
            timeText.text = $"Time: {minutes}:{seconds:00}";
    }

    private void SpawnItem()
    {
        if (dropArea == null) return;

        
        float rand = Random.value; // 0~1
        GameObject prefab = (rand > 0.5f) ? goodItemPrefab : badItemPrefab;
        if (prefab == null) return;

        
        float halfWidth = dropArea.rect.width / 2f;
        float spawnX = Random.Range(-halfWidth, halfWidth);

       
        float spawnY = dropArea.rect.height / 2f;

       
        GameObject itemObj = Instantiate(prefab, dropArea);
        RectTransform itemRect = itemObj.GetComponent<RectTransform>();

      
        itemRect.anchoredPosition = new Vector2(spawnX, spawnY);
    }

    private void MovePlayer()
    {
        if (playerRect == null) return;

        
        float h = Input.GetAxis("Horizontal");
        Vector2 moveDelta = new Vector2(h * playerMoveSpeed * Time.deltaTime, 0);
        playerRect.anchoredPosition += moveDelta;

        
        float halfWidth = dropArea.rect.width / 2f - playerRect.rect.width / 2f;
        float clampedX = Mathf.Clamp(playerRect.anchoredPosition.x, -halfWidth, halfWidth);
        playerRect.anchoredPosition = new Vector2(clampedX, playerRect.anchoredPosition.y);
    }

   
    private void CheckItemCollisions()
    {
       
        for (int i = 0; i < dropArea.childCount; i++)
        {
            Transform child = dropArea.GetChild(i);
            if (child == null) continue;

            RectTransform itemRect = child.GetComponent<RectTransform>();
            if (itemRect == null) continue;

            
            if (itemRect.anchoredPosition.y < -dropArea.rect.height / 2f)
            {
               
                Destroy(child.gameObject);
                continue;
            }

           
            if (IsOverlap(playerRect, itemRect))
            {
                
                if (child.CompareTag("GoodItem"))
                    AddScore(10);
                else if (child.CompareTag("BadItem"))
                    AddScore(-5);
                else
                    AddScore(1); 

                
                Destroy(child.gameObject);
            }
            else
            {
                
                itemRect.anchoredPosition -= new Vector2(0, 200f * Time.deltaTime);
            }
        }
    }

    private bool IsOverlap(RectTransform rectA, RectTransform rectB)
    {
       
        Rect a = new Rect(rectA.anchoredPosition.x, rectA.anchoredPosition.y,
            rectA.rect.width, rectA.rect.height);
        Rect b = new Rect(rectB.anchoredPosition.x, rectB.anchoredPosition.y,
            rectB.rect.width, rectB.rect.height);
        return a.Overlaps(b);
    }

  
    public void AddScore(int amount)
    {
        score += amount;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

   
    public void EndMinigame()
    {
        gameActive = false;
        minigamepanel.SetActive(false);
        Debug.Log("EndMinigame Called");
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            if (resultText != null)
            {
                resultText.text = $"Your Score: {score}";
            }
        }

       
    }
    public void EndMinigame(int score)
    {
        if (onMinigameEnd != null)
            onMinigameEnd.Invoke(score);
    }
}
