using PrimeTween;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private CivilianManager _civilianManager;

    [SerializeField] public List<RegularCriminalBehaviour> _regularEnemiesList;

    [SerializeField] private Canvas canvas;

    [SerializeField] private GameObject _pointPopUp;
    [SerializeField] private GameObject _pointEnd;

    private void Start()
    {
        _regularEnemiesList = new List<RegularCriminalBehaviour>();
    }

    private void Update()
    {
        foreach (RegularCriminalBehaviour criminal in _regularEnemiesList)
        {
            if(!criminal.HasTarget())
            {
                criminal.SetTarget(ClosestCivilian(criminal));
            }
        }
    }

    public GameObject ClosestCivilian(RegularCriminalBehaviour enemy)
    {
        float distance = enemy.GetDetectionRadius();
        GameObject closest = null;
        foreach(GameObject civilian in _civilianManager._civilians)
        {
            float compareDist;
            compareDist = Vector3.Distance(enemy.gameObject.transform.position, civilian.gameObject.transform.position);
            if(compareDist < distance)
            {
                distance = compareDist;
                closest = civilian.gameObject;
            }
        }
        return closest;
    }
    public void AddToEnemyList(GameObject enemy)
    {
        enemy.GetComponent<EnemyDeath>().OnKilled += RemoveEnemy;
        _regularEnemiesList.Add(enemy.GetComponent<RegularCriminalBehaviour>());
    }

    public void RemoveEnemy(object sender, GameObject enemy)
    {
        if (!_regularEnemiesList.Contains(enemy.GetComponent<RegularCriminalBehaviour>()))
        {
            return;
        }
        _regularEnemiesList.Remove(enemy.GetComponent<RegularCriminalBehaviour>());
        enemy.GetComponent<EnemyDeath>().OnKilled -= RemoveEnemy;
        Vector3 randomVector = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(5f, 10f), UnityEngine.Random.Range(-5f, 5f));
        GameObject point = Instantiate(_pointPopUp, Camera.main.WorldToScreenPoint(enemy.transform.position+ randomVector), _pointPopUp.transform.rotation, canvas.transform);
        Tween.Scale(point.transform, Vector3.zero, duration: 1, ease: Ease.InOutSine);
        Tween.Position(point.transform, _pointEnd.transform.position ,  duration: 1, ease: Ease.OutSine);
    }
}
