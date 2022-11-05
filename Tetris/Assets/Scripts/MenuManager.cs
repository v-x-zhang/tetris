using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private WindowScript windowScript;
    [SerializeField]
    private Animator animator;
    public Text timedSinglePlayerHighScoreText;
    public Text levelSinglePlayerHighScoreText;

    private void Start()
    {
        Time.timeScale = 1;
        //Set Texts
        timedSinglePlayerHighScoreText.text = "Highscore : " + PlayerPrefs.GetInt("TimeHighScore", 0).ToString();
        levelSinglePlayerHighScoreText.text = "Highscore : " + PlayerPrefs.GetInt("LevelHighScore", 0).ToString();
    }


    public void Back()
    {
        animator.SetTrigger("Singleplayer-Main");
    }

    public void SingleplayerClicked()
    {
        animator.SetTrigger("Main-Singleplayer");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void TimeSingleClick() //The on click for when the player clicks on the timed singleplayer
    {
        SceneFader.instance.FadeTo(1);
    }

    public void LevelSingleClick() //The on click for when the player clicks on the level singleplayer
    {
        SceneFader.instance.FadeTo(2);
    }
}
