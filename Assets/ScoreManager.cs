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
    private float totalScore = 0f;
    private float accumulatedTimerScore = 0f;
    public float accumulatedTotalScore = 0f;

            private float timer = 0f;

    void Update()
    {
        CalculateTotalScore();
    }

    void CalculateTotalScore()
    {
        bool hasLiveCamera = false;
        
        // 先检查场景中是否存在符合条件的摄像机
        foreach (var camera in FindObjectsOfType<ConeDetection>())
        {
            if (camera.gameObject.CompareTag("LiveCamera"))
            {
                hasLiveCamera = true;
                break;
            }
        }

        // 只有存在符合条件的摄像机时才进行计时加分
        if (hasLiveCamera)
        {
            timer += Time.deltaTime;
            if (timer >= Random.Range(0.1f, 4f))
            {
                accumulatedTimerScore += 1f;
                timer = 0f;
            }
        }
        else
        {
            timer = 0f; // 如果没有符合条件的摄像机，重置计时器
        }

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
                totalScore += accumulatedTimerScore;
                accumulatedTotalScore = totalScore;
            }
        }
    }
}
