using System.Collections;
using FMOD;
using UnityEngine;

public enum SpawnSide
{
    X_Side, Z_Side
}
public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] GameObject portal;
    [Header("Enemy Prefab")]
    [SerializeField] GameObject civilian;

    [Header("Spawn Control")]
    [SerializeField] SpawnPosition[] spawnPositions;
    [Header("Weve")]
    [SerializeField] int bigwaveNum;

    [SerializeField] int totalHumans;
    [SerializeField] int waveNum;
    [SerializeField] float radiusSpawn;
    [SerializeField] float waveCooldown;

    // top<topLeft +  topRight> | left<topLeft +  BottomLeft> | right<topRight +  BottomRight> | bottom<BottomLeft +  BottomRight>
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
            for (int numWave = 0; numWave < bigwaveNum; numWave++)
            {
                #region Random Position Spawn
                int rndPositionSpawn = Random.Range(0, spawnPositions.Length);
                Vector3 centerSpawn = spawnPositions[rndPositionSpawn].positionSpawn[0].transform.position;
                if (spawnPositions[rndPositionSpawn].side == SpawnSide.X_Side)
                {
                    float X_st = spawnPositions[rndPositionSpawn].positionSpawn[0].transform.position.x;
                    float X_end = spawnPositions[rndPositionSpawn].positionSpawn[1].transform.position.x;
                    float rndX = Random.Range(X_st, X_end);
                    centerSpawn.x = rndX;
                }
                else
                {
                    float Z_st = spawnPositions[rndPositionSpawn].positionSpawn[0].transform.position.z;
                    float Z_end = spawnPositions[rndPositionSpawn].positionSpawn[1].transform.position.z;
                    float rndZ = Random.Range(Z_st, Z_end);
                    centerSpawn.z = rndZ;
                }
                #endregion

                totalHumans -= waveNum;
                canSpawn = false;
                for (int i = 0; i < waveNum; i++)
                {
                    Vector3 randomPosition = Random.insideUnitCircle * radiusSpawn;
                    randomPosition = new Vector3(randomPosition.x, centerSpawn.y, randomPosition.y) + centerSpawn;
                    NpcCivilian nPCnormal = Instantiate(civilian, randomPosition, civilian.transform.rotation).GetComponent<NpcCivilian>();
                    nPCnormal.targetOut = portal;
                }
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
