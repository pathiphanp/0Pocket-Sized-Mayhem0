using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interface;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class NpcCivilian : MonoBehaviour, TakeDamage, Fear, AddInCar<GameObject>, Invite
{
    Rigidbody rb;
    [SerializeField] GameObject myModel;
    protected Vector3 target;
    [HideInInspector] public GameObject targetOut;
    [SerializeField] protected Vector3 newTargetOut;
    [SerializeField] GameObject targetFear;
    protected NavMeshAgent navMeshAgent;

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

    [Header("Car")]
    [SerializeField] float getInCarDistance;
    protected Car car;
    bool onInvite = false;
    TargetType type = TargetType.NPC;

    Coroutine heartbeat;
    // Start is called before the first frame update
    void Start()
    {
        targetOut = FindAnyObjectByType<Portal>().gameObject;//Test
        rb = GetComponent<Rigidbody>();
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
        StopCoroutine(heartbeat);
        Destroy(this.gameObject);
        return type;
    }

    public void AddFear()
    {
        if (canFear)
        {
            navMeshAgent.speed = 0;
            //play animation fear run
            StartCoroutine(CooldownFearStatus());
            if (callfear == null)
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
    protected void StopMove()
    {
        if (heartbeat != null)
        {
            findTarget = false;
            StopCoroutine(heartbeat);
        }
        navMeshAgent.speed = 0;
        navMeshAgent.isStopped = true;//stop move npc
    }
    IEnumerator Heartbeat()
    {
        navMeshAgent.destination = target;
        yield return new WaitForSeconds(heartbeatDuration);
        findTarget = true;
        heartbeat = null;
    }
    #region  Car
    public void InviteToCar(Car _car)
    {
        if (!onInvite)
        {
            onInvite = true;
            StartCoroutine(GoToCar(_car));
        }
    }
    public virtual IEnumerator GoToCar(Car _carTarget)
    {
        //play animation run
        car = _carTarget;
        navMeshAgent.speed = afterFearSpeed;
        bool canGetInCar = false;
        target = _carTarget.transform.position;
        while (transform.position != target && !canGetInCar)
        {
            if (navMeshAgent.remainingDistance <= getInCarDistance && navMeshAgent.remainingDistance > 0)
            {
                canGetInCar = true;
            }
            yield return true;
        }
        StopMove();
        yield return new WaitForSeconds(1f);
        _carTarget.AddHumans(this.gameObject);
        ExtarActionInCar();
    }
    public virtual void ExtarActionInCar() { }

    public void AddInCar(GameObject _waitPosition)
    {
        transform.SetParent(_waitPosition.transform);
        transform.localPosition = Vector3.zero;
        rb.interpolation = RigidbodyInterpolation.None;
    }
    public void CarStar()
    {
        navMeshAgent.enabled = false;
        myModel.SetActive(false);
    }
    public void OutCar()
    {
        findTarget = true;
        target = newTargetOut;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    #endregion
}
