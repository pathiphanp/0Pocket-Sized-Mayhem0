using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class TestManager : MonoBehaviour
{
    [SerializeField] private float _gameTime = 300;
    [SerializeField] private float _startHunger = 50;
    [SerializeField] private float _startPoint = 0;
    [SerializeField] private TextMeshProUGUI _timerText;

    private float _startTime;
    private float _elapsedTime;
    void Awake()
    {
        PlayerStats.GameTime = _gameTime;
        PlayerStats.Hunger = _startHunger;
        PlayerStats.Points = _startPoint;
    }

    void Update()
    {
        _elapsedTime = Time.time - _startTime;

        if (_gameTime - _elapsedTime > 0)
        {
            int minutes = (int)((_gameTime - _elapsedTime) / 60) % 60;
            int seconds = (int)((_gameTime - _elapsedTime) % 60);
            _timerText.text = "Remaining Time: " + string.Format("{0:0}:{1:00}", minutes, seconds);
        }
        else
        {
            _timerText.text = "Remaining Time: " + 0;
            SceneManager.LoadScene("WinScreen");
        }
    }
    public void OnIncreaseTime()
    {
        Debug.Log("xs");
        _elapsedTime += 10;
    }
    public void OnDecreaseTime()
    {
        _elapsedTime -= 10;
    }
    public void OnIncreasePoint()
    {
        PlayerStats.Points += 10;
    }
    public void OnDecreasePoint()
    {
        Debug.Log("hehe");
        PlayerStats.Points -= 10;
    }

    public void OnRestartScene()
    {
        SceneManager.LoadScene("GameScene - M3");
    }


    public static void AddPoint(float point)
    {
        PlayerStats.Points += point;
    }
    public static void LosePoint()
    {
        PlayerStats.Points -= 10;
    }
}
