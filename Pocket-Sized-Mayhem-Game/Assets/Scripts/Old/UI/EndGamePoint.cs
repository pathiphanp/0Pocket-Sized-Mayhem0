using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Hierarchy;
using UnityEngine;

public class EndGamePoint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    private float point = PlayerStats.Points;
    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = point.ToString();
    }
}
