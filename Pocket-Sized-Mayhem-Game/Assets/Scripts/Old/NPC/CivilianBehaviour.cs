using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using CharacterMovement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CivilianBehaviour : CharacterMovement3D
{
    [field: SerializeField] public EventReference SpawnSFX { get; set; }
    
    [SerializeField] private GameObject _destination;
    [SerializeField] private Animator _animator;
    
    
    private void Start()
    {
        if (!SpawnSFX.IsNull)
        {
            RuntimeManager.PlayOneShot(SpawnSFX, this.gameObject.transform.position);
        }
        
    }

    protected override void  Update()
    {
        base.Update();
        MoveTo(_destination.transform.position);
        
    }
    
    public void SetDestionation(GameObject newDestination)
    {
        _destination = newDestination;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Shelter")))
        {
            _animator.SetTrigger("DestinationReached");
        }
    }
}
