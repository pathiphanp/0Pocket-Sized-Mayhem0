using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] GameObject portal;
    [Header("Enemy Prefab")]
    [SerializeField] GameObject civilian;

    [Header("Weve")]
    [SerializeField] int totalHumans;
    [SerializeField] int waveNum;
    [SerializeField] float radiusSpawn;
    [SerializeField] float waveCooldown;

    bool canSpawn = true;
    // Start is called before the first frame update
    void Start()
    {
        GameManager._instance.AddTotalHumans(totalHumans);    
    }

    // Update is called once per frame
    void Update()
    {
        ControlSpawnEnemy();
    }

    void ControlSpawnEnemy()
    {
        if (canSpawn && totalHumans > 0)
        {
            totalHumans -= waveNum;
            canSpawn = false;

            for (int i = 0; i < waveNum; i++)
            {
                Vector3 randomPosition = Random.insideUnitCircle * radiusSpawn;
                randomPosition = new Vector3(randomPosition.x, this.gameObject.transform.position.y, randomPosition.y) + this.gameObject.transform.position;
                NPCnormal nPCnormal = Instantiate(civilian, randomPosition, civilian.transform.rotation).GetComponent<NPCnormal>();
                nPCnormal.targetOut = portal;
            }
            StartCoroutine(CooldownSpawn());
        }

    }
    IEnumerator CooldownSpawn()
    {
        yield return new WaitForSeconds(waveCooldown);
        canSpawn = true;
    }
}
