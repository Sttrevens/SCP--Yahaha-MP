using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BobojianScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    private ScoreManager scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = scoreManager.accumulatedTotalScore.ToString("F0");
    }
}
