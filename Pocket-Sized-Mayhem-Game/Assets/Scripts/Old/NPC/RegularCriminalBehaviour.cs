using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using CharacterMovement;
using UnityEngine;
using UnityEngine.AI;
using PrimeTween;

public class RegularCriminalBehaviour : CharacterMovement3D
{
    [field: SerializeField] public EventReference AttackSFX { get; set; }
    
    [SerializeField] protected GameObject _shelter;
    [SerializeField] protected GameObject _targetCivilian;
    [SerializeField] protected GameObject _primaryTarget;

    [SerializeField] protected float _detectionRadius;
   
    [SerializeField] protected NavMeshAgent _navMeshAgent;
    [SerializeField] protected Animator _enemyAnimator;

    private bool _isSpinning= false;
    
    private void Start()
    {
        _shelter = _primaryTarget;
        if (!AttackSFX.IsNull)
        {
            RuntimeManager.PlayOneShot(AttackSFX, this.gameObject.transform.position);
        }
       // GetComponent<NavMeshAgent>().agentTypeID = Random.Range(0, 2);
    }

    protected override void Update()
    {
        base.Update();
        if (Vector3.Distance(transform.position, _primaryTarget.transform.position) < 1f)
        {
            _enemyAnimator.SetTrigger("Attack");
            Stop();
            SetLookPosition(_primaryTarget.transform.position);
        }
        else
        {
            MoveTo(_primaryTarget.transform.position); 
        }

        if (_isSpinning)
        {
            transform.Rotate(180 * Time.deltaTime, 180 * Time.deltaTime, 180 * Time.deltaTime);

        }
        
    }

    public void SetTarget(GameObject target)
    {
        if(target != null)
        {
            _primaryTarget = target;
        }
    }

    public virtual bool HasTarget()
    {
        if(_primaryTarget != _shelter)
        {
            return true;
        }
        return false;
    }

    public float GetDetectionRadius()
    {
        return _detectionRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _primaryTarget)
        {
            _primaryTarget = _shelter;
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("Vacuum"))
        {
            _isSpinning = true;
            _enemyAnimator.SetTrigger("Vacuum");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(15))
        {
            // GameManager.LosePoint();
        }
    }
}
