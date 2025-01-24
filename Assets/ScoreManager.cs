using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LPSurvivalEngine;

public class ScoreManager : MonoBehaviour
{

    // �洢���� CameraController ������
    private List<ConeDetection> cameraControllers = new List<ConeDetection>();

    // ��ǰ�ܷ�
    public float totalScore = 0f;

    void Update()
    {
        // ÿ֡�����ܷ�
        CalculateTotalScore();
    }

    void CalculateTotalScore()
    {
        if (cameraControllers.Count != 0)
        {
            // ��շ���
            totalScore = 0f;
        }

        // ��ȡ�����е����� CameraController ���
        cameraControllers.Clear();
        var allCameras = FindObjectsOfType<ConeDetection>();
        foreach (var camera in allCameras) {
            if (camera.gameObject.CompareTag("LiveCamera")) {
                cameraControllers.Add(camera);
            }
        }

        // �������� CameraController ��ȡ accumulatedScore
        foreach (var cameraController in cameraControllers)
        {
            // ȷ�� CameraController ���� accumulatedScore ����
            if (cameraController != null)
            {
                totalScore += cameraController.accumulatedScore; // ���� accumulatedScore �ǹ�����
            }
        }
    }

}
