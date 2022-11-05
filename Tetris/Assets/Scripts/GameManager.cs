using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    bool isPaused = false;
    public bool gameStarted = false;
    public GameObject pauseMenu;

    public int score = 0;
    public int lines = 0;
    public int pieces = 0;
    public int level = 1;
    public int highScore = 0;
    public bool isTime;
    public bool isLevel;

    [Header("Gameplay Texts")]
    public Text scoreText;
    public Text levelText;
    public Text pieceText;
    public Text lineText;
    public Text timerText;
    public Text highScoreText;
    public Transform lineClearTextContainer;
    public GameObject lineClearTextPrefab;

    [Header("Game Over")]
    public bool gameOver;
    public GameObject gameOverUI;
    public Text goScoreText;
    public Text goLevelText;
    public Text goPieceText;
    public Text goLineText;
    public Text goTimerText;

    [Header("Game Start")]
    public GameObject startTextPrefab;

    //Timer
    private TimeSpan timePlaying;
    private bool timerGoing;
    private float elapsedTime;
    
    private void Start()
    {
        Time.timeScale = 0f;
        gameOver = false;
        StartCoroutine(StartGame());
    }

    private void Update()
    {

        if (gameOver)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Toggle();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            score += 10000;
        }
        
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (!isPaused && !gameOver)
            {
                Toggle();
            }
        }
    }
    public void GameOver()
    {
        gameOver = true;

        
        AudioManager.instance.FadeOut();
        Time.timeScale = 0;
        gameOverUI.SetActive(true);
        StartCoroutine(SetGameOverTexts());
    }

    public void Retry()
    {
        SceneFader.instance.FadeTo(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneFader.instance.FadeTo(0);
    }

    public void CreateLineClearText(string linesCleared)
    {
        GameObject lineTextGo = Instantiate(lineClearTextPrefab, lineClearTextContainer);

        Text lineText = lineTextGo.GetComponent<Text>();
        lineText.text = linesCleared;

        Destroy(lineTextGo, 1f);
    }

    public void Resume()
    {
        Toggle();
    }

    public void Toggle()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            AudioManager.instance.FadeThin();
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            AudioManager.instance.FadeIn();

        }
        isPaused = !isPaused;

    }

    public void Exit()
    {
        Application.Quit();
    }

    public void UpdateText()
    {
      
        scoreText.text = "Score : " + score.ToString();
        levelText.text = "Level " + level.ToString();
        pieceText.text = "Pieces : " + pieces.ToString();
        lineText.text = "Lines : " + lines.ToString();

    }

    public void SetDefaults()
    {
        timerText.text = "00:00.00";
        timerGoing = false;
        score = 0;
        lines = 0;
        pieces = 0;
        level = 1;
    }

    public void LineClearScore(int lines)
    {

        switch(lines)
        {
            case 1:
                score += 40 * level;
                AudioManager.instance.PlayAudio(AudioManager.instance.lineClear1);
                CreateLineClearText("SINGLE");

                break;
            case 2:
                score += 100 * level;
                AudioManager.instance.PlayAudio(AudioManager.instance.lineClear2);
                CreateLineClearText("DOUBLE");

                break;
            case 3:
                score += 300 * level;
                AudioManager.instance.PlayAudio(AudioManager.instance.lineClear3);
                CreateLineClearText("TRIPLE");

                break;
            case 4:
                score += 1200 * level;
                AudioManager.instance.PlayAudio(AudioManager.instance.lineClear4);
                AudioManager.instance.PlayAudio(AudioManager.instance.lineClear4Add);
                CreateLineClearText("TETRIS!");

                break;
            default:
                score += 0;
                break;
        }

        this.lines += lines;
        UpdateText();
    }



    public void BeginTimer()
    {
        timerGoing = true;
        elapsedTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    private IEnumerator SetGameOverTexts()
    {
        int scoreCounter = 0;
        goLevelText.text = level.ToString();
        goTimerText.text = timerText.text;

        if (isTime)
        {
            highScore = PlayerPrefs.GetInt("TimeHighScore", 0);
            highScoreText.text = "High Score: " + highScore.ToString();
        }
        else if (isLevel)
        {
            highScore = PlayerPrefs.GetInt("LevelHighScore", 0);
            highScoreText.text = "High Score: " + highScore.ToString();
        }
        


        while (scoreCounter <= score/3)
        {
            goScoreText.text = scoreCounter.ToString();
            scoreCounter += score/40;
            yield return null;
        }
        while(scoreCounter <= score/1.5)
        {
            goScoreText.text = scoreCounter.ToString();
            scoreCounter += score/30;
            yield return null;
        }
        while (scoreCounter <= score - 50)
        {
            goScoreText.text = scoreCounter.ToString();
            scoreCounter += 10;
            yield return null;
        }
        while (scoreCounter <= score)
        {
            goScoreText.text = scoreCounter.ToString();
            scoreCounter += 1;
            yield return null;
        }

        int lineCounter = 0;
        while (lineCounter <= lines)
        {
            goLineText.text = lineCounter.ToString();
            lineCounter++;
            yield return null;
        }

        int pieceCounter = 0;
        while (pieceCounter <= pieces)
        {
            goPieceText.text = pieceCounter.ToString();
            pieceCounter++;
            yield return null;
        }


        
        if (isTime)
        {
            if (score > highScore)
            {
                PlayerPrefs.SetInt("TimeHighScore", score);
                highScore = score;
            }
            highScoreText.text = "High Score: " + highScore.ToString();
        }
        else if (isLevel)
        {
            if (score > highScore)
            {
                PlayerPrefs.SetInt("LevelHighScore", score);
                highScore = score;
            }
            highScoreText.text = "High Score: " + highScore.ToString();
        }
    }

    public GameObject CreateStartText(string startText)
    {
        GameObject lineTextGo = Instantiate(startTextPrefab, lineClearTextContainer);

        Text lineText = lineTextGo.GetComponent<Text>();
        lineText.text = startText;
        if(startText == "3" || startText == "2" || startText == "1")
        {
            lineText.fontSize = 200;
        }
        else
        {
            lineText.fontSize = 72;
        }

        return lineTextGo;
    }
    private IEnumerator StartGame()
    {
        yield return new WaitForSecondsRealtime(1.9f);
        GameObject text1 = CreateStartText("3");
        AudioManager.instance.PlayAudio(AudioManager.instance.timerClick);
        yield return new WaitForSecondsRealtime(0.9f);
        Destroy(text1);
        GameObject text2 = CreateStartText("2");
        AudioManager.instance.PlayAudio(AudioManager.instance.timerClick);
        yield return new WaitForSecondsRealtime(0.9f);
        Destroy(text2);
        AudioManager.instance.PlayAudio(AudioManager.instance.timerClick);
        GameObject text3 = CreateStartText("1");
        yield return new WaitForSecondsRealtime(0.9f);
        Destroy(text3);
        GameObject text4 = CreateStartText("START!");
        AudioManager.instance.PlayAudio(AudioManager.instance.timerStart);
        gameStarted = true;
        UpdateText();
        SetDefaults();
        BeginTimer();
        Time.timeScale = 1f;
        Destroy(text4, 1f);

        AudioManager.instance.StartMusic();
    }

    #region Timer
    public void EndTimer()
    {
        timerGoing = false;
    }

    private IEnumerator UpdateTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timePlayingStr = timePlaying.ToString("mm':'ss'.'ff");
            timerText.text = timePlayingStr;

            yield return null;
        }
    }
    #endregion

}
