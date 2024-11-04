using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelterHealthBar : MonoBehaviour
{
    [SerializeField] private ShelterHealth _health;
    [SerializeField] private Image _fillBar;

    private void OnValidate()
    {
        if(_health == null)
        {
            GetComponentInParent<ShelterHealth>();
        }
    }

    private void Update()
    {
        if( _health == null)
        {
            return;
        }

        _fillBar.fillAmount = _health.Percentage;
    }
}
