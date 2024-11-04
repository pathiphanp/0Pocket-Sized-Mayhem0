using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CivilianSpawner : Spawner
{
    [SerializeField] private GameObject _shelter;

    private CivilianManager _civilianManager;
    private NavMeshAgent _navAgent;

}
