using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    private static bool waiting;
    private float timeElapsed;
    private bool timerOn;
    public float timeAllowed;
    public float scoreRate;

    private Pause pauseScript;
    public GameObject lossMenuUI;

    public Text timerDisplay;
    public Text scoreDisplay;

    public Text scoreLostDisplay;
    public Text highScoreLostDisplay;

    void Start()
    {
        pauseScript = FindObjectOfType<Pause>();

        timeElapsed = 0;
        timerOn = false;
        if (instance == null) {
            instance = this;
        }
        if (waiting) {
            StartTimer();
            waiting = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float timeRemaining = timeAllowed - timeElapsed;
        if (timerOn) {
            timeElapsed += Time.deltaTime;
            timeRemaining = timeAllowed - timeElapsed;
            if (timeElapsed >= timeAllowed) {
                timerOn = false;
                if (PlayerPrefs.GetInt("score") > PlayerPrefs.GetInt("highscore")) {
                    PlayerPrefs.SetInt("highscore", PlayerPrefs.GetInt("score"));
                }
                Time.timeScale = 0.0f;
                Lose();
            }
        }
        timerDisplay.text = "Time Remaining - " + (int)(timeRemaining/60) + ":" + ((int)(timeRemaining%60)).ToString("00");
        scoreDisplay.text = "Score: " + PlayerPrefs.GetInt("score").ToString();
    }

    void Lose() {
        pauseScript.PlayLostSound();
        pauseScript.GameIsLost = true;
        scoreLostDisplay.text = PlayerPrefs.GetInt("score").ToString();
        highScoreLostDisplay.text = PlayerPrefs.GetInt("highscore").ToString();
        lossMenuUI.SetActive(true);
    }

    public static void AddWin() {
        int score = PlayerPrefs.GetInt("score") + (int)((instance.timeAllowed - instance.timeElapsed) * instance.scoreRate + 0.5f);
        PlayerPrefs.SetInt("score", score);
    }

    public static void StartTimer() {
        if (instance != null) {
            instance.timerOn = true;
            instance.timeElapsed = 0;
        }
        else {
            waiting = true;
        }
    }
}
