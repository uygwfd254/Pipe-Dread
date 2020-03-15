using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score_Management : MonoBehaviour
{
    protected Score_Management() {}
    // game objects
    TextMeshProUGUI ScoreText;

    private int level = 0;
    private int scores = 0;
    private int lives = 0;
    private float time = 0f;
    // Start is called before the first frame update
    void Start()
    {
        ScoreText = transform.Find("Score and Info").GetComponent<TextMeshProUGUI>();
        // original settings
        level = 1;
        lives = 3;
        time = -10f;
    }

    // Update is called once per frame
    void Update()
    {
        update_score_board();
    }

    void update_score_board()
    {
        string information = "Level: " + level.ToString() + "   " +
                             "Scores: " + scores.ToString() + "   " +
                             "Lives: " + lives.ToString() + "   " +
                             "Time: " + (-time).ToString("0");

        ScoreText.text = information;
    }
}