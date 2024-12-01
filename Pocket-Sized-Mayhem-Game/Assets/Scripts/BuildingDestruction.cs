using FMODUnity;
using Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDestruction : MonoBehaviour, TakeDamage
{
    [SerializeField] private int _force;
    [SerializeField] private List<GameObject> _pieces;
    private bool _isDestoyed;
    TargetType type = TargetType.Building;
    [field: SerializeField] public EventReference DeathSFX { get; set; }
    void Start()
    {
        _isDestoyed = false;
        _pieces = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject piece = transform.GetChild(i).gameObject;
            _pieces.Add(piece);
        }
    }
    public TargetType TakeDamage()
    {
        RuntimeManager.PlayOneShot(DeathSFX, this.gameObject.transform.position);
        this.GetComponent<Rigidbody>().isKinematic = false;
        foreach (GameObject piece in _pieces)
        {
            if (piece.GetComponent<Rigidbody>() == null)
            {
                BuildingScrap _Bs = piece.AddComponent<BuildingScrap>();
                _Bs.rb = piece.AddComponent<Rigidbody>();
                _Bs.rb.useGravity = true;
                _Bs.Explode(_force);
            }
        }
        Destroy(this);
        return type;
    }
}
