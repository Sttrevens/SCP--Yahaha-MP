using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BobojianScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    private ScoreManager scoreManager;
    [SerializeField] private TMP_Text totalScoreText;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreText != null)
            scoreText.text = scoreManager.CurrentViewers.ToString();
        if (totalScoreText != null)
            totalScoreText.text = scoreManager.networkedTotalScore.ToString();
    }
}
