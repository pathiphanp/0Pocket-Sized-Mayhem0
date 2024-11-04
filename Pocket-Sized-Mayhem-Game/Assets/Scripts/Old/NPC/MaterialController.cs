using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField] private GameObject _vacuumSuckPoint;
    [SerializeField] private Material _material;
    
    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        _material.SetVector("_Target", _vacuumSuckPoint.transform.position);
    }
}
