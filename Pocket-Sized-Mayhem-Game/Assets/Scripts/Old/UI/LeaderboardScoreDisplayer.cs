using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LeaderboardScoreDisplayer : MonoBehaviour
{
    const string LeaderboardId = "High_Score";

    [SerializeField] private string[] _leaderboardScores;
    [SerializeField] private List<Score> _scoreList;
    [SerializeField] private GameObject _textExample;

    private List<GameObject> _textList;

    private void Start()
    {
        _textList = new List<GameObject>();
        _scoreList = new List<Score>();
        GetScores();
    }
    public async void GetScores()
    {
        _scoreList.Clear();
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
        string scoresText = JsonConvert.SerializeObject(scoresResponse);
        string[] separatingStrings = { "playerName\":\"", "score\":", "#", "},{" };
        _leaderboardScores = scoresText.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
        Console.Clear();
        if (_leaderboardScores.Length > 1)
        {
            for (int i = 1; i < _leaderboardScores.Length -3; i += 4)
            {
                Score score = new Score(_leaderboardScores[i], Convert.ToDouble(_leaderboardScores[i + 2]));
                _scoreList.Add(score);
            }
            _leaderboardScores[_leaderboardScores.Length - 1] = _leaderboardScores[_leaderboardScores.Length - 1].Substring(0, _leaderboardScores[_leaderboardScores.Length - 1].Length - 3);
            Score scoreLast = new Score(_leaderboardScores[_leaderboardScores.Length - 3], Convert.ToDouble(_leaderboardScores[_leaderboardScores.Length - 1]));
            _scoreList.Add(scoreLast);
        }
        _scoreList.OrderBy(Score => Score.name);
        makeUITextList();
    }
    public void makeUITextList()
    {
        int i = 1;
        foreach (Score score in _scoreList)
        {
            GameObject scoreText = Instantiate(_textExample);
            _textList.Add(scoreText);
            scoreText.GetComponent<TextMeshProUGUI>().text = i + " - " + score.name + " | " + score.score;
            scoreText.transform.SetParent(this.transform);
            i++;
        }
    }

    public void addScoreToLeaderboard()
    {

    }
}


