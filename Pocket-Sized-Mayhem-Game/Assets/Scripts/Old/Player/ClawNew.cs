using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawNew : Weapon
{
    [SerializeField] private bool _fired = false;
    [SerializeField] private float _originalStartY;

    private Vector3 _mousePos;
    private Vector3 hitpoint;
    private RaycastHit hit;
    public override void Fire()
    {
        _fired = true;
    }
    private void Update()
    {
        _mousePos = Input.mousePosition;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(_mousePos), out hit))
        {
            return;
        }

        this.transform.position = hit.point;
        hitpoint = hit.point;
        hitpoint.y = _originalStartY;
        transform.position = hitpoint;
    }
}
