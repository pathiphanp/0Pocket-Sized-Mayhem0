using CharacterMovement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationTest : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private CharacterMovement3D _movement;

    private void Update()
    {
        _movement.MoveTo(_target.position);
    }
}
