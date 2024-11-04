using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hunger : MonoBehaviour
{
    [SerializeField] Image _fillBar;
    // Update is called once per frame
    private void Start()
    {
        PlayerStats.Hunger = 50;
    }
    void Update()
    {
        _fillBar.fillAmount = PlayerStats.Hunger/100;
    }
}
