using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("Total Humans")]
    [SerializeField] TMP_Text totalHumansText;
    [Header("Humans Kill")]
    [SerializeField] TMP_Text humansKillText;
    [Header("Enemy escaped")]
    [SerializeField] TMP_Text humansEscapedText;

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
}
