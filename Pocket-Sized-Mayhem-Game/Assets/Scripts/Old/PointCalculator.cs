using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointCalculator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pointText;
    private void Start()
    {
        PlayerStats.Points = 0;
    }
    void Update()
    {
        _pointText.text = "Point:" + PlayerStats.Points;
    }
}
