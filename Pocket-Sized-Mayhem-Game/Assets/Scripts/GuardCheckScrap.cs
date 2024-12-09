using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardCheckScrap : MonoBehaviour
{
    NPCGuard npcGuard;
    private void Start()
    {
        npcGuard = GetComponent<NPCGuard>();
        npcGuard.checkScrap = this;
    }
    List<BuildingScrap> buildingScraps;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BuildingScrap>() != null)
        {
            npcGuard.CallAttackScarp(other.gameObject);
            this.gameObject.SetActive(false);
        }
    }
}
