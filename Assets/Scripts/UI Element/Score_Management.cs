using System;
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
    private int time = 0;
    // Start is called before the first frame update
    void Start()
    {
        ScoreText = transform.Find("Score and Info").GetComponent<TextMeshProUGUI>();
        // original settings
        level = 1;
        lives = 3;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        update_score_board();
        update_time();
    }

    void update_score_board()
    {
        string information = "Level: " + level.ToString() + "   " +
                             "Scores: " + scores.ToString() + "   " +
                             "Lives: " + lives.ToString() + "   " +
                             "Time: " + (Math.Abs(time - 10)).ToString("0");

        ScoreText.text = information;
    }

    void update_time() {
        time = (int)Event_Manager.TriggerEvent("get_time");
    }
}