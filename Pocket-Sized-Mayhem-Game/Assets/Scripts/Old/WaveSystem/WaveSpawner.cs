using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Wave[] waves;

    private Wave currentWave;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private Transform[] spawnpoints;

    private float timeBtwnSpawns;
    private int i = 0;

    private bool stopSpawning = false;

    private void Awake()
    {

        currentWave = waves[i];
        //timeBtwnSpawns = currentWave.TimeBeforeThisWave;
    }

    private void Update()
    {
        if (stopSpawning)
        {
            return;
        }

        if (Time.time >= timeBtwnSpawns)
        {
           // SpawnWave();
            //IncWave();

            //timeBtwnSpawns = Time.time + currentWave.TimeBeforeThisWave;
        }
    }

    private void SpawnWave()
    {
        //for (int i = 0; i < currentWave.Time; i++)
        //{
        //    Instantiate(_enemyPrefab, _enemyPrefab.transform.position, _enemyPrefab.transform.rotation);
        //}
    }

    private void IncWave()
    {
        if (i + 1 < waves.Length)
        {
            i++;
            currentWave = waves[i];
        }
        else
        {
            stopSpawning = true;
        }
    }
}