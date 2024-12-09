using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonControl : MonoBehaviour
{
    [SerializeField] GameObject positionSpawnGuard;
    [SerializeField] GameObject guardNpcPrefab;

    [SerializeField] GameObject[] positionWalkAround;
   
    public void SpawnGuard()
    {
        NPCGuard newGuard = Instantiate(guardNpcPrefab, positionSpawnGuard.transform.position,
        positionSpawnGuard.transform.rotation).GetComponent<NPCGuard>();
        int rndP = Random.Range(0, positionWalkAround.Length);
        newGuard.SetUpTarget(positionWalkAround[rndP].transform.position);
        newGuard.ResetStatus();
    }
}
