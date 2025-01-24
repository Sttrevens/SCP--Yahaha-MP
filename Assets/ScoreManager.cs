using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LPSurvivalEngine;

public class ScoreManager : MonoBehaviour
{
    // 洢 CameraController 
    private List<ConeDetection> cameraControllers = new List<ConeDetection>();

    // ǰܷ
    public float totalScore = 0f;
    private float timer = 0f;

    void Update()
    {
        // 每帧增加计时器
        timer += Time.deltaTime;
        
        // 每秒固定增加1分
        if (timer >= 1f)
        {
            totalScore += 1f;
            timer = 0f;
        }

        // 保持原有的分数计算逻辑
        CalculateTotalScore();
    }

    void CalculateTotalScore()
    {
        if (cameraControllers.Count != 0)
        {
            //շ
            totalScore = 0f;
        }

        // ȡе CameraController 
        cameraControllers.Clear();
        var allCameras = FindObjectsOfType<ConeDetection>();
        foreach (var camera in allCameras) {
            if (camera.gameObject.CompareTag("LiveCamera")) {
                cameraControllers.Add(camera);
            }
        }

        //  CameraController ȡ accumulatedScore
        foreach (var cameraController in cameraControllers)
        {
            // ȷ CameraController accumulatedScore
            if (cameraController != null)
            {
                totalScore += cameraController.accumulatedScore; // accumulatedScore ǹ
            }
        }
    }
}
