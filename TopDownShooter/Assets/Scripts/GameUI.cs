using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityScript.Steps;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public TextMeshProUGUI newWaveTitle;
    public TextMeshProUGUI enemyCount;
    public TextMeshProUGUI scoreUI;
    public RectTransform healthBar;

    private Player player;
    private Spawner spawner;

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    private void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        print(ScoreKeeper.score);
        if (player != null)
        {
            float healthPercent = player.health / player.startingHealth;
            healthBar.localScale = new Vector3(healthPercent, 1,1);
        }
        
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    void OnNewWave(int waveNumber)
    {
        string[] waveNumbers = {"One", "Two", "Three", "Four", "Five"};
        newWaveTitle.text = $"-Wave {waveNumbers[waveNumber-1]}-";
        if (spawner.currentWave.infinite)
        {
            enemyCount.text = "Enemies: infinite";
        }
        else
        {
            enemyCount.text = $"Enemies: {spawner.currentWave.enemyCount}";
        }
        
        StartCoroutine(AnimateNewWaveBanner());
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1.5f;
        float animationPercent = 0;
        float bannerAnimationSpeed = 3f;
        int dir = 1;

        float endDelayTime = Time.time + 1 / bannerAnimationSpeed + delayTime;

        while (animationPercent >= 0)
        {
            animationPercent += Time.deltaTime * bannerAnimationSpeed*dir;

            if (animationPercent >= 1)
            {
                animationPercent = 1;
                if (Time.time > endDelayTime)
                {
                    dir = -1;
                }
               
            }
            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-210,45,animationPercent);
            yield return null;
        }

    }

    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, Color.black, 1f));
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent<1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }

    }

    //UI input
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }
}
