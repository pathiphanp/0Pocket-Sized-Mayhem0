using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/Waves", order = 1)]
public class Wave : ScriptableObject
{
    [field: SerializeField]
    public string Time { get;  set; }

    [field: SerializeField]
    public List<GameObject> Spawners { get;  set; }

    [field: SerializeField]
    public string NumberToSpawn { get;  set; }
}
