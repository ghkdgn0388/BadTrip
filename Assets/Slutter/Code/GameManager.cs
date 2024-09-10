using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("# Game Control")]
    public bool isLive = true;
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    public float addTime;

    public Player player;
    public Result uiResult;
    public int Floor;
    public int highScore;

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 144;
        AudioManager.instance.PlayBgm(true);

        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameOver();
        }
    }
    public void GameOver()
    {
        if (Floor > highScore)
        {
            highScore = Floor;
            PlayerPrefs.SetInt("HighScore", highScore);  // 최고 기록 저장
        }

        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.1f);
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();
    }
    public void AddTime()
    {
        gameTime -= addTime;
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
        Resume();
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
    public int GetHighScore()
    {
        return highScore;
    }

}
