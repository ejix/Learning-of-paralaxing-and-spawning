using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;
    
    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
   // public GameObject countdownPage;
    public Text scoreText;
   
    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown
    }
    int score = 0;
    public bool gameOver = false;
    public bool GameOver { get { return gameOver; } }

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        Tapcontrol.OnPlayerDied += OnPlayerDied;
        Tapcontrol.OnPlayerScored += OnPlayerScored;
    }
    private void OnDisable()
    {
        Tapcontrol.OnPlayerDied -= OnPlayerDied;
        Tapcontrol.OnPlayerScored -= OnPlayerScored;
    }
  
    void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            
        }
        SetPageState(PageState.GameOver);
    }

    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }

    void SetPageState(PageState state)
    {
        switch(state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
              //  countdownPage.SetActive(false);

                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
              //  countdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
              //  countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                //countdownPage.SetActive(true);
                break;
        }
    }
    public void ConfirmGameOver()
    {
         OnGameOverConfirmed();
        scoreText.text = "0";
        OnGameOverConfirmed();
        SetPageState(PageState.Start);
    }
    public void StartGame()
    {
        OnGameStarted();
        SetPageState(PageState.None);
    }
}
