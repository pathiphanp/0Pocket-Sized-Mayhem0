using System.Collections;
using UnityEngine;

public enum TargetType
{
    NPC, Building
}
public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public PlayerControl playerControl;
    [HideInInspector] public UiManager uiManager;
    [SerializeField] public TextEffect textEffectControl;
    float playerWinPoint;
    [SerializeField] public float humansPercentKill;
    [SerializeField] public int totalHumans;
    [SerializeField] public int playerKill = 0;
    [SerializeField] public int humansEscaped = 0;

    public bool canPlayGame = false;
    public override void Awake()
    {
        base.Awake();
        uiManager = FindAnyObjectByType<UiManager>();
        playerControl = FindAnyObjectByType<PlayerControl>();
    }
    private void Start()
    {
        playerControl.gameObject.SetActive(false);
    }
    public void AddPointPlayerKill()
    {
        string report = playerKill++.ToString() + " / " + playerWinPoint;
        uiManager.UpdateUiHumansKill(report);
        RemoveTotalHumans();
    }
    public void AddPointHumansEscaped()
    {
        uiManager.UpdateUiHumansEscaped(humansEscaped++.ToString());
        RemoveTotalHumans();
    }
    public void RemoveTotalHumans()
    {
        totalHumans--;
        uiManager.UpdateUiTotalHumans(totalHumans.ToString());
        if (totalHumans == 0)
        {
            StopGame();
            uiManager.playerWinUi.SetActive(true);
            StartCoroutine(StartShowPlayerKill());
            StartCoroutine(StartShowHumansEscaped());
        }
    }
    IEnumerator StartShowPlayerKill()
    {
        int _scorPlayerKill = 0;
        while (_scorPlayerKill != playerKill)
        {
            _scorPlayerKill++;
            uiManager.UpdateUiPlayerKill(_scorPlayerKill);
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator StartShowHumansEscaped()
    {
        int _scorHumansEscaped = 0;
        while (_scorHumansEscaped != humansEscaped)
        {
            _scorHumansEscaped++;
            uiManager.UpdateUiHumansEscaped(_scorHumansEscaped);
            yield return new WaitForSeconds(0.015f);
        }
    }
    public void AddTotalHumans(int _taltalHumans)
    {
        GameManager._instance.totalHumans += _taltalHumans;
        uiManager.UpdateUiTotalHumans(totalHumans.ToString());
        playerWinPoint = totalHumans * humansPercentKill;
        uiManager.UpdateUiHumansKill(0 + " / " + playerWinPoint.ToString());
    }

    public void StartGame()
    {
        StartCoroutine(StartEventStartGame());
    }
    IEnumerator StartEventStartGame()
    {
        uiManager.mainManuObj.SetActive(false);
        uiManager.ChangeCamera(uiManager.zoomPotalCamera);
        yield return new WaitForSeconds(2f);//Wait Zoom form MainManu to GamePlay
        DescriptionCutScenes descriptionCutScenesStartGame = uiManager.FindDescriptionCutScenes("StartGame");
        descriptionCutScenesStartGame.cutScenesObject.SetActive(true);
        textEffectControl.StartReadText(descriptionCutScenesStartGame.description, descriptionCutScenesStartGame.textCutScens);
        yield return new WaitForSeconds(descriptionCutScenesStartGame.durationCutScenes);//Wait CutScenes
        descriptionCutScenesStartGame.cutScenesObject.SetActive(false);
        uiManager.ChangeCamera(uiManager.gameplayCamera);
        yield return new WaitForSeconds(2f);//Wait CutScenes
        canPlayGame = true;
        uiManager.missionUi.SetActive(true);
        playerControl.gameObject.SetActive(true);
    }

    void StopGame()
    {
        canPlayGame = false;
        uiManager.missionUi.SetActive(false);
    }
}