using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardCheckScrap : MonoBehaviour
{
    [SerializeField] NPCGuard npcGuard;
    private void Start()
    {
        npcGuard.checkScrap = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BuildingScrap>() != null)
        {
            npcGuard.CallAttackScarp(other.gameObject);
            this.gameObject.SetActive(false);
        }
    }
}
