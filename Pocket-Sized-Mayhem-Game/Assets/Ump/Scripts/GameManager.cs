using UnityEngine;
public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public UiManager uiManager;

    float playerWinPoint;
    [SerializeField] public float humansPercentKill;
    [SerializeField] public int totalHumans;
    [SerializeField] public int humansPlayerKill = 0;
    [SerializeField] public int humansEscaped = 0;

    public override void Awake()
    {
        base.Awake();
        uiManager = FindAnyObjectByType<UiManager>();
    }
    public void AddPointPlayerKill()
    {
        string report = humansPlayerKill++.ToString() + " / " + playerWinPoint;
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
    }
    public void AddTotalHumans(int _taltalHumans)
    {
        GameManager._instance.totalHumans += _taltalHumans;
        uiManager.UpdateUiTotalHumans(totalHumans.ToString());
        playerWinPoint = totalHumans * humansPercentKill;
        uiManager.UpdateUiHumansKill(0 + " / " + playerWinPoint.ToString());
    }
}