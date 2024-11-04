using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnforcerBehaviour : RegularCriminalBehaviour
{
    [SerializeField] private Patrol _patrolPath;
    private int _currentWaypoint = 0;
    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _primaryTarget = _patrolPath._waypoints[_currentWaypoint];
    }

    private void Update()
    {
        if (_primaryTarget.GetComponent<CivilianDeath>() == null && Vector3.Distance(_targetCivilian.gameObject.transform.position,this.transform.position) > _detectionRadius)
        {
            if (Vector3.Distance(_primaryTarget.transform.position, _navMeshAgent.transform.position) <= _navMeshAgent.stoppingDistance)
            {
                _primaryTarget = NextWaypoint();
            }
        }
        else
        {
            _primaryTarget = _targetCivilian;
        }
        _navMeshAgent.destination = _primaryTarget.transform.position;
    }

    public override bool HasTarget()
    {
        if (_primaryTarget.GetComponentInParent <Patrol>() != null)
        {
            return false;
        }
        return true;
    }

    private GameObject NextWaypoint()
    {
        if(_currentWaypoint < _patrolPath._waypoints.Count - 1)
        {
            _currentWaypoint++;
            return _patrolPath._waypoints[_currentWaypoint];
        }
        else
        {
            _currentWaypoint = 0;
            return _patrolPath._waypoints[0];
        }
    }
}
