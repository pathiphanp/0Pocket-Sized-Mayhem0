using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomEscapePointGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] _escapeEndPoints;
    [SerializeField] private List<GameObject> _civilians;

    [SerializeField] private float _escapeRange;
    public void AddCivilian(GameObject civilian)
    {
        _civilians.Add(civilian);
    }
}
