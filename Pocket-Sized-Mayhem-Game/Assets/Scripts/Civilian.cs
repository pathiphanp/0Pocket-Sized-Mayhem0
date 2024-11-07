using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interface;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Civilian : MonoBehaviour, TakeDamage, Fear, AddInCar<GameObject>
{
    [SerializeField] GameObject myModel;
    Vector3 target;
    [HideInInspector] public GameObject targetOut;
    [SerializeField] Vector3 newTargetOut;
    [SerializeField] GameObject targetFear;
    NavMeshAgent navMeshAgent;

    [SerializeField] float heartbeatDuration;
    bool findTarget = true;

    [Header("Speed Setting")]
    [SerializeField] float bornSpeed;
    [SerializeField] float fearSpeed;
    [SerializeField] float afterFearSpeed;
    [Header("Fear Setting")]
    [SerializeField] float radiusFear;
    [SerializeField] float fearDuration;
    [SerializeField] float cooldownFear;
    bool canFear = true;
    Coroutine callfear;

    TargetType type = TargetType.NPC;

    Coroutine heartbeat;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetUpHumansBorn();
        SetUpTarget();
        HeartbeatNavMash();
    }
    void SetUpHumansBorn()
    {
        navMeshAgent.speed = bornSpeed;
        //play animation walk
    }
    public virtual void SetUpTarget()
    {
        navMeshAgent.avoidancePriority = Random.Range(0, 100);
        newTargetOut = Random.insideUnitCircle * 5;
        newTargetOut = new Vector3(newTargetOut.x, 0, newTargetOut.y) + targetOut.transform.position;
        target = newTargetOut;
    }

    // Update is called once per frame
    void Update()
    {
        HeartbeatNavMash();
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
            navMeshAgent.speed = 0;
            //play animation fear run
            StartCoroutine(CooldownFearStatus()); if (callfear == null)
            {
                callfear = StartCoroutine(DurationFear());
            }
            Vector3 randomPosition = Random.insideUnitCircle * radiusFear;
            randomPosition = new Vector3(randomPosition.x, 0, randomPosition.y) + transform.position;
            targetFear.transform.position = randomPosition;
            target = targetFear.transform.position;
            navMeshAgent.speed = fearSpeed;
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
        //play animation run
        target = newTargetOut;
        // navMeshAgent.destination = targetOut.transform.position;
        navMeshAgent.speed = afterFearSpeed;
        callfear = null;
    }
    void HeartbeatNavMash()
    {
        if (findTarget)
        {
            findTarget = false;
            heartbeat = StartCoroutine(Heartbeat());
        }
    }
    IEnumerator Heartbeat()
    {
        navMeshAgent.destination = target;
        yield return new WaitForSeconds(heartbeatDuration);
        findTarget = true;
        heartbeat = null;
    }

    public void AddInCar(GameObject _waitPosition)
    {
        if (heartbeat != null)//stop find npc
        {
            StopCoroutine(heartbeat);
        }
        navMeshAgent.isStopped = true;//stop move npc
        transform.position = _waitPosition.transform.position;
        transform.SetParent(_waitPosition.transform);
    }

    public void invisible()
    {
        myModel.SetActive(false);
    }
}
