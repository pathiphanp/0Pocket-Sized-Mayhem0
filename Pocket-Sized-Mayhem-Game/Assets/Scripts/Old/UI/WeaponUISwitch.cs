using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUISwitch : MonoBehaviour
{
    [SerializeField] private GameObject _primary;
    [SerializeField] private GameObject _secondary;

    [SerializeField] private Sprite _mallet;
    [SerializeField] private Sprite _claw;

    public void OnSwitchWeapon()
    {
        if(_primary.GetComponent<Image>().sprite == _mallet)
        {
            _primary.GetComponent<Image>().sprite = _claw;
            _secondary.GetComponent<Image>().sprite = _mallet;
        }
        else
        {
            _primary.GetComponent<Image>().sprite = _mallet;
            _secondary.GetComponent<Image>().sprite = _claw;
        }
    }
}
