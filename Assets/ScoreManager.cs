using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LPSurvivalEngine;

public class ScoreManager : MonoBehaviour
{
    // TMP Text ������ʾ�ܷ�
    [SerializeField] private TMP_Text scoreText;

    // �洢���� CameraController ������
    private List<ConeDetection> cameraControllers = new List<ConeDetection>();

    // ��ǰ�ܷ�
    public float totalScore = 0f;

    void Start()
    {
        // ȷ�� Score Text �Ѿ�����
        if (scoreText == null)
        {
            Debug.LogError("Score Text not assigned in ScoreManager!");
            return;
        }

        // ��ʼ��������ʾ
        UpdateScoreDisplay();
    }

    void Update()
    {
        // ÿ֡�����ܷ�
        CalculateTotalScore();
        UpdateScoreDisplay();
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

    // ���·�����ʾ
    void UpdateScoreDisplay()
    {
        // ����������µ� TMP Text ��
        scoreText.text = "Total Viewers: " + totalScore.ToString("F0");
    }
}
