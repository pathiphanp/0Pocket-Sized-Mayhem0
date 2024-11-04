using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;
using UnityEngine.AI;

public class NPCnormal : MonoBehaviour, TakeDamage, Fear
{
    GameObject target;
    [HideInInspector] public GameObject targetOut;
    [SerializeField] GameObject targetFear;
    NavMeshAgent navMeshAgent;

    [Header("Fear Seting")]
    [SerializeField] float radiusFear;
    [SerializeField] float fearDuration;
    [SerializeField] float cooldownFear;
    bool canFear = true;
    Coroutine callfear;

    TargetType type = TargetType.NPC;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 5f;
        target = targetOut;
    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.destination = target.transform.position;
    }

    public TargetType TakeDamage()
    {
        Destroy(this.gameObject);
        return type;
    }

    public void AddFear()
    {
        if (canFear)
        {
            StartCoroutine(CooldownFearStatus());
            navMeshAgent.speed = 15f;
            Vector3 randomPosition = Random.insideUnitCircle * radiusFear;
            randomPosition = new Vector3(randomPosition.x, 0, randomPosition.y) + transform.position;

            targetFear.transform.position = randomPosition;
            target = targetFear;
            if (callfear == null)
            {
                callfear = StartCoroutine(DurationFear());
            }
        }

    }
    IEnumerator CooldownFearStatus()
    {
        canFear = false;
        yield return new WaitForSeconds(cooldownFear);
    }
    IEnumerator DurationFear()
    {
        yield return new WaitForSeconds(fearDuration);
        target = targetOut;
        navMeshAgent.speed = 10f;
        callfear = null;
    }
}
