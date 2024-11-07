using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("MainManu")]
    [SerializeField] public GameObject mainManuObj;
    [SerializeField] public Button btnStartGame;
    [SerializeField] public Button btnExitGame;

    [Header("GamePlay")]
    [SerializeField] public GameObject missionUi;
    [Header("Total Humans")]
    [SerializeField] TMP_Text totalHumansText;
    [Header("Humans Kill")]
    [SerializeField] TMP_Text humansKillText;
    [Header("Enemy escaped")]
    [SerializeField] TMP_Text humansEscapedText;

    [Header("Player Win")]
    [SerializeField] public GameObject playerWinUi;
    [SerializeField] TMP_Text scorePlayerKillText;
    [SerializeField] TMP_Text scoreHumansEscapedText;

    [Header("CutScenesStartGame")]
    [SerializeField] DescriptionCutScenes[] descriptionCutScenes;

    [Header("Camera")]
    [SerializeField] public GameObject mainManuCamera;
    [SerializeField] public GameObject gameplayCamera;
    [SerializeField] public GameObject zoomPotalCamera;

    private void Start()
    {
        btnStartGame.onClick.AddListener(GameManager._instance.StartGame);
    }
    private void OnDisable()
    {
        btnStartGame.onClick.RemoveListener(GameManager._instance.StartGame);
    }

    public void UpdateUiTotalHumans(string _num)
    {
        totalHumansText.text = _num;
    }
    public void UpdateUiHumansKill(string _num)
    {
        humansKillText.text = _num;
    }
    public void UpdateUiHumansEscaped(string _num)
    {
        humansEscapedText.text = _num;
    }
    public void ResetAllCamera()
    {
        mainManuCamera.SetActive(false);
        gameplayCamera.SetActive(false);
        zoomPotalCamera.SetActive(false);
    }
    public void ChangeCamera(GameObject _camaraTarget)
    {
        ResetAllCamera();
        _camaraTarget.SetActive(true);
    }

    public DescriptionCutScenes FindDescriptionCutScenes(string nameDescription)
    {
        DescriptionCutScenes d = Array.Find(descriptionCutScenes, x => x.nameDescription == nameDescription);
        return d;
    }
    public void UpdateUiPlayerKill(int _scorePlayerKill)
    {
        scorePlayerKillText.text = _scorePlayerKill.ToString();
    }
    public void UpdateUiHumansEscaped(int _scoreHumansEscaped)
    {
        scoreHumansEscapedText.text = _scoreHumansEscaped.ToString();
    }
}
