using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _mallet;

    [SerializeField] private GameObject _target;

    [SerializeField] private Material _clawTargetMaterial;
    [SerializeField] private Material _malletTargetMaterial;

    private Weapon _activeWeapon;

    private LayerMask _layerMask;

    void Start()
    {
        _activeWeapon = _mallet.GetComponent<Weapon>();
    }

    public void OnFire()
    {
        _activeWeapon.Fire();
    }
    
}
