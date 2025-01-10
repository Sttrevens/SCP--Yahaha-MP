using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // TMP Text 用于显示总分
    [SerializeField] private TMP_Text scoreText;

    // 存储所有 CameraController 的引用
    private List<ConeDetection> cameraControllers = new List<ConeDetection>();

    // 当前总分
    private float totalScore = 0f;

    void Start()
    {
        // 确保 Score Text 已经分配
        if (scoreText == null)
        {
            Debug.LogError("Score Text not assigned in ScoreManager!");
            return;
        }

        // 初始化分数显示
        UpdateScoreDisplay();
    }

    void Update()
    {
        // 每帧更新总分
        CalculateTotalScore();
        UpdateScoreDisplay();
    }

    void CalculateTotalScore()
    {
        // 清空分数
        totalScore = 0f;

        // 获取场景中的所有 CameraController 组件
        cameraControllers.Clear();
        cameraControllers.AddRange(FindObjectsOfType<ConeDetection>());

        // 遍历所有 CameraController 获取 accumulatedScore
        foreach (var cameraController in cameraControllers)
        {
            // 确保 CameraController 中有 accumulatedScore 属性
            if (cameraController != null)
            {
                totalScore += cameraController.accumulatedScore; // 假设 accumulatedScore 是公开的
            }
        }
    }

    // 更新分数显示
    void UpdateScoreDisplay()
    {
        // 将大分数更新到 TMP Text 上
        scoreText.text = "Total Viewers: " + totalScore.ToString("F0");
    }
}
