using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms.Impl;
using Unity.Collections.LowLevel.Unsafe;
using System;
using Unity.Services.Leaderboards.Models;
using Unity.VisualScripting;
public class LeaderboardAuthenticator : MonoBehaviour
{
    // Create a leaderboard with this ID in the Unity Cloud Dashboard
    const string LeaderboardId = "High_Score";

    [SerializeField] private string _playerName;
    [SerializeField] private double _playerScore;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.ClearSessionToken();
            await SignInAnonymously();
            DontDestroyOnLoad(this.transform.parent);
        }
    }
    async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + _playerName);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            Debug.Log(s);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await AuthenticationService.Instance.UpdatePlayerNameAsync(_playerName);
    }

    public async void AddScore()
    {
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, _playerScore);
    }
}
