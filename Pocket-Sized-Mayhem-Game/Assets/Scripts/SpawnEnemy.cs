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
    [SerializeField] NPC_ObjectPool npc_Pool;
    [SerializeField] GameObject npc_Driver;

    [Header("Spawn Control")]
    [SerializeField] SpawnPosition[] spawnPositions;
    [Header("Weve")]
    [SerializeField] int bigwaveNum;

    [SerializeField] int totalHumans;
    [SerializeField] int waveNum;
    [SerializeField] float radiusSpawn;
    [SerializeField] float radiusCenterFollow;
    [SerializeField] float waveCooldown;
    [Header("Center Target")]
    int countTargetCenter;
    bool isCenter;
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
        if (canSpawn)
        {
            canSpawn = false;
            if (totalHumans > 0)
            {
                totalHumans -= (waveNum * bigwaveNum);
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
                    NpcCivilian new_Npc = null;
                    for (int i = 0; i < waveNum; i++)
                    {

                        bool rndNpc = RandomChance._instance.GetRandomChance(90);
                        if (rndNpc)
                        {
                            new_Npc = npc_Pool.Pool.Get().GetComponent<NpcCivilian>();
                        }
                        else
                        {
                            new_Npc = Instantiate(npc_Driver).GetComponent<NpcCivilian>();
                            new_Npc.gameObject.SetActive(false);
                        }
                        //Create position target
                        Vector3 randomPosition = Random.insideUnitCircle * radiusSpawn;
                        randomPosition = new Vector3(randomPosition.x, centerSpawn.y, randomPosition.y) + centerSpawn;
                        new_Npc.transform.position = randomPosition;
                        Vector3 centerFollow = Random.insideUnitCircle * radiusCenterFollow;
                        centerFollow = new Vector3(centerFollow.x, new_Npc.transform.position.y, centerFollow.y) + new_Npc.transform.position;
                        new_Npc.gameObject.SetActive(true);
                        if (countTargetCenter == 0)
                        {
                            new_Npc.SetUpTarget(portal.transform.position, 5);
                        }
                        else
                        {
                            new_Npc.SetUpTarget(centerFollow, 0);
                        }
                        countTargetCenter++;
                        new_Npc.ResetStatus();
                    }
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
