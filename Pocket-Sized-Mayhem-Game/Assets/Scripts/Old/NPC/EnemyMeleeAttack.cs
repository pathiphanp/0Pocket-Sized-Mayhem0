using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMeleeAttack : MonoBehaviour
{
    [SerializeField] private BoxCollider _knifeCollider;
    private void Start()
    {
        _knifeCollider.enabled = false;
    }

    private void Attack()
    {
        _knifeCollider.enabled = true;
    }
    private void AttackEnd()
    {
        _knifeCollider.enabled = false;
    }
}
