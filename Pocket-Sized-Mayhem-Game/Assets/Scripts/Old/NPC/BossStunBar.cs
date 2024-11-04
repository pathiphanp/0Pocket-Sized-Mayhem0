using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossStunBar : MonoBehaviour
{
    [SerializeField] private float _maxStunDuration;
    [SerializeField] public float _stunDuration;
    [SerializeField] private float _stunIncrease;
    [SerializeField] private Image _fillBar;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;

    private float _percentage;
    private float _elapsedTime;
    private float _startTime;
    private float _minStunDuration;
    private void Start()
    {
        _minStunDuration = 0;
        _maxStunDuration = 10;
        _stunDuration = _minStunDuration;
    }

    private void Update()
    {
        _stunDuration = Mathf.Clamp(_stunDuration, _minStunDuration, _maxStunDuration);
        _percentage = _stunDuration/ _maxStunDuration;
        _fillBar.fillAmount = _percentage;
        if(_stunDuration > 0 && _agent.enabled )
        {
            _agent.isStopped = true;
        }
        else if(_agent.enabled)
        {
            _agent.isStopped = false;
        }
        _stunDuration -= Time.deltaTime;

        transform.rotation = Camera.main.transform.rotation;
    }

    public void IncreaseStun()
    {
        _stunDuration += _stunIncrease;
    }
    public float GetStunDuration()
    {
        return _stunDuration;
    }
}
