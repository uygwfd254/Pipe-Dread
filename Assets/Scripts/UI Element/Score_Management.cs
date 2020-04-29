using System;
using UnityEngine;
using TMPro;

public class Score_Management : MonoBehaviour
{
    protected Score_Management() {}
    // game objects
    TextMeshProUGUI ScoreText;

    int level;
    int scores;
    int lives;
    int time;
    int start_time;
    // Start is called before the first frame update
    void Start()
    {
        ScoreText = transform.Find("Score and Info").GetComponent<TextMeshProUGUI>();
        // original settings
        level = PlayerPrefs.GetInt("Level", 1);
        lives = PlayerPrefs.GetInt("Lives", 3);
        time = 0;
        start_time = Math.Max(15 - (level / 5), 5);

        if (lives == 0)
            Level_Manager.Instance.FadeToMainMenu();
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
                             "Time: " + (Math.Abs(time - start_time)).ToString("0");

        ScoreText.text = information;
    }

    void update_time() {
        time = (int)Event_Manager.TriggerEvent("get_time");
    }

    // set up listener for communication
    Func<System.Object, System.Object> RemoveLives;
    Func<System.Object, System.Object> AddScore;
    Func<System.Object, System.Object> SubtractScore;
    Func<System.Object, System.Object> GoToNextLevel;
    Func<System.Object, System.Object> SendLevel;
    Func<System.Object, System.Object> SendScore;

    void Awake() {
        RemoveLives = new Func<System.Object, System.Object>(removeOneLive);
        AddScore = new Func<System.Object, System.Object>(addScore);
        SubtractScore = new Func<System.Object, System.Object>(subtractScore);
        GoToNextLevel = new Func<System.Object, System.Object>(goToNextLevel);
        SendLevel = new Func<System.Object, System.Object>(sendLevel);
        SendScore = new Func<System.Object, System.Object>(sendScore);
    }

    void OnEnable()
    {
        Event_Manager.StartListening("remove_a_live", RemoveLives);
        Event_Manager.StartListening("add_score_to_scoreboard", AddScore);
        Event_Manager.StartListening("subtract_score_to_scoreboard", SubtractScore);
        Event_Manager.StartListening("go_to_next_level", GoToNextLevel);
        Event_Manager.StartListening("send_level", SendLevel);
        Event_Manager.StartListening("send_score", SendScore);
    }

    void OnDisable()
    {
        Event_Manager.StopListening("remove_a_live", RemoveLives);
        Event_Manager.StopListening("add_score_to_scoreboard", AddScore);
        Event_Manager.StopListening("subtract_score_to_scoreboard", SubtractScore);
        Event_Manager.StopListening("go_to_next_level", GoToNextLevel);
        Event_Manager.StopListening("send_level", SendLevel);
        Event_Manager.StopListening("send_score", SendScore);
    }

    // listening functions
    System.Object removeOneLive(System.Object p) {
        lives--;
        PlayerPrefs.SetInt("Lives", lives);
        return null;
    }

    System.Object addScore(System.Object score) {
        scores += (int)score;
        return null;
    }

    System.Object subtractScore(System.Object score) {
        scores -= (int)score;
        return null;
    }

    System.Object goToNextLevel(System.Object p) {
        level++;
        lives++;
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Lives", lives);
        Level_Manager.Instance.ReloadCurrentScene();
        return null;
    }

    System.Object sendLevel(System.Object p) {
        return level;
    }

    System.Object sendScore(System.Object p) {
        return scores;
    }
}