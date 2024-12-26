using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Interface;
using UnityEngine;
using UnityEngine.AI;

public class Car : MonoBehaviour, TakeDamage, SetGuardEffectProtect
{
    Collider coll;
    Rigidbody rb;
    [SerializeField] GameObject carModel;

    [Header("Speed")]
    [SerializeField] float speedMove;
    [SerializeField] float speedRotation;
    [SerializeField] float speedMoveBack;
    [SerializeField] float speedMoveForward;
    [SerializeField] float durationMoveForward;
    [Header("Navmesh")]
    [SerializeField] float heartbeatDuration;
    Coroutine heartbeatNav;
    NavMeshAgent carNavMeshAgent;
    NavMeshObstacle carBroken;

    [Header("Driver")]
    public NpcDriver npcDriver = null;
    List<NpcDriver> npcDriverTargetCar = new List<NpcDriver>();
    int countHumansInCar = 0;
    [Header("CarPart")]
    [SerializeField] public CarWaitPosition[] waitPosition;
    [SerializeField] Transform[] backCar;
    [SerializeField] float backCarcheckDistance;
    [SerializeField] LayerMask layerMaskBuildubg;
    [SerializeField] float fuel;
    [SerializeField] float fuelMinDuration;
    [SerializeField] float fuelMaxDuration;
    [Header("Dodge Triger")]
    [SerializeField] GameObject dodgeTrigerObj;
    [SerializeField] float dodgeTrigerRadius;
    [Header("Explode")]
    [SerializeField] ParticleSystem explodeEffect;
    [SerializeField] float explodeRadius;
    [SerializeField] LayerMask npc;
    bool canDie = true;
    bool isDie = false;
    Vector3 targetOut;
    [HideInInspector] public bool carOnStart = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        carBroken = GetComponent<NavMeshObstacle>();
        carNavMeshAgent = GetComponent<NavMeshAgent>();
        carNavMeshAgent.speed = 0;
        dodgeTrigerObj.GetComponent<SphereCollider>().radius = dodgeTrigerRadius;
    }
    #region CarStart
    public void StartCar(Vector3 _targetOut)
    {
        StartCoroutine(CallStartCar(_targetOut));
    }
    IEnumerator CallStartCar(Vector3 _targetOut)
    {
        carNavMeshAgent.enabled = true;
        fuel = Random.Range(fuelMinDuration, fuelMaxDuration);
        dodgeTrigerObj.SetActive(true);
        carOnStart = true;
        //off all humans
        foreach (CarWaitPosition w in waitPosition)
        {
            if (w.humans != null)
            {
                w.humans.GetComponent<AddInCar<GameObject>>().DoActionOnCarStar();
                w.humans.SetActive(false);
            }
        }
        yield return new WaitForSeconds(0.5f);
        carNavMeshAgent.destination = _targetOut;
        targetOut = _targetOut;
        StartCoroutine(CheckForwordToTarget(_targetOut));
        StartCoroutine(FuelDuration());
    }
    #endregion
    #region AddHuman InCar
    public void AddHumansToCar(GameObject _human)
    {
        CarWaitPosition _carWaitPosition = FindEmptyWaitPosition();
        if (_carWaitPosition != null && !carOnStart)
        {
            if (_carWaitPosition.empty)
            {
                _carWaitPosition.humans = _human;
                _human.GetComponent<AddInCar<GameObject>>().AddInCar(_carWaitPosition.waitPosition);
                _carWaitPosition.empty = false;
                if (npcDriver != null && _human != npcDriver.gameObject)
                {
                    npcDriver.otherHumans.Remove(_human.GetComponent<Invite>());
                }
                countHumansInCar++;
                if (countHumansInCar == 4)
                {
                    npcDriver.StartDriveCar();
                }
            }
        }
        else
        {
            _human.GetComponent<Invite>().ChangeTargetToPortal();
        }
    }
    #endregion
    public bool CheckHaveNpcDriver(NpcDriver _npcDriver)
    {
        if (npcDriver == null)
        {
            npcDriverTargetCar.Add(_npcDriver);
            return true;
        }
        return false;
    }
    public bool AddDriver(NpcDriver _npcDriver)
    {
        if (npcDriver == null)
        {
            npcDriver = _npcDriver;
            npcDriverTargetCar.Remove(_npcDriver);
            if (npcDriverTargetCar.Count > 0)
            {
                foreach (NpcDriver nDc in npcDriverTargetCar)
                {
                    // Debug.Log(nDc.name);
                    // Remove other Driver
                    nDc.ChangeTargetToPortal();
                }
            }
            npcDriverTargetCar.Clear();
            return true;
        }
        return false;
    }
    CarWaitPosition FindEmptyWaitPosition()
    {
        //Find empty waitPosition
        for (int i = 0; i < waitPosition.Length; i++)
        {
            if (waitPosition[i].empty)
            {
                return waitPosition[i];
            }
        }
        return null;
    }
    IEnumerator CheckForwordToTarget(Vector3 _target)
    {
        carNavMeshAgent.speed = speedRotation;
        Vector3 directionToTarget = FindAnyObjectByType<Portal>().gameObject.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        while (angle >= 30f)
        {
            int checkBackCarCount = 0;
            //Check angle
            directionToTarget = FindAnyObjectByType<Portal>().gameObject.transform.position - transform.position;
            angle = Vector3.Angle(transform.forward, directionToTarget);
            for (int i = 0; i < backCar.Length; i++)
            {
                if (Physics.Raycast(backCar[i].transform.position, backCar[i].transform.forward,
                out RaycastHit hit, backCarcheckDistance, layerMaskBuildubg))
                {
                    //move forward if back Car hit anything
                    checkBackCarCount++;
                    StartCoroutine(MoveForward());
                    yield return new WaitForSeconds(durationMoveForward);
                }
            }
            //move back not hit anything
            transform.position -= transform.forward * speedMoveBack;
            yield return true;
        }
        speedMoveBack = 0;
        carNavMeshAgent.speed = speedMove;
        heartbeatNav = StartCoroutine(Heartbeat());
    }
    IEnumerator MoveForward()
    {
        rb.AddForce(transform.forward * speedMoveForward, ForceMode.VelocityChange);
        yield return new WaitForSeconds(durationMoveForward);
        rb.velocity = Vector3.zero;
        rb.Sleep();

    }
    #region Fuel Control
    IEnumerator FuelDuration()
    {
        yield return new WaitForSeconds(fuel);
        carNavMeshAgent.speed = 0;
        carNavMeshAgent.enabled = false;
        dodgeTrigerObj.SetActive(false);
        StopAllCoroutines();
        rb.Sleep();
        rb.velocity = Vector3.zero;
        EjectHumans();
    }
    #endregion

    #region EjectHumans
    void EjectHumans()
    {
        bool leftRigth = false;
        Vector3 targetEject = Vector3.zero;
        foreach (CarWaitPosition w in waitPosition)
        {
            if (w.humans != null)
            {
                w.humans.SetActive(true);
                //kick Humans
                if (leftRigth)
                {
                    targetEject = -w.waitPosition.transform.right;
                }
                else
                {
                    targetEject = w.waitPosition.transform.right;
                }
                leftRigth = !leftRigth;
                w.humans.gameObject.GetComponent<OutCar>().OutCar(targetEject);
            }
        }
        carBroken.enabled = true;
    }
    #endregion
    IEnumerator Heartbeat()
    {
        carNavMeshAgent.destination = targetOut;
        yield return new WaitForSeconds(heartbeatDuration);
        heartbeatNav = StartCoroutine(Heartbeat());
    }
    #region  Take Damage
    public bool TakeDamage()
    {
        if (!isDie && canDie)
        {
            isDie = true;
            carModel.SetActive(false);
            explodeEffect.Play();
            StopAllCoroutines();
            carNavMeshAgent.speed = 0;
            carNavMeshAgent.enabled = false;
            StartCoroutine(CarDestroy());
            return true;
        }
        return false;
    }
    public TargetType ThisType()
    {
        return TargetType.Building;
    }
    IEnumerator CarDestroy()
    {
        coll.enabled = false;
        dodgeTrigerObj.SetActive(false);
        if (heartbeatNav != null)
        {
            StopCoroutine(heartbeatNav);
            heartbeatNav = null;
        }
        foreach (CarWaitPosition w in waitPosition)
        {
            if (w.humans != null)
            {
                w.humans.SetActive(true);
                w.humans.transform.SetParent(null);
                w.humans.GetComponent<TakeDamage>().TakeDamage();
            }
        }
        yield return new WaitForSeconds(0.5f);
        Explode();
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    void Explode()
    {
        // ค้นหา Collider ทั้งหมดในรัศมีที่กำหนด
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explodeRadius, npc);
        // วนลูปทำลาย Object ที่อยู่ในรัศมี
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject)
            {
                hitCollider.GetComponent<TakeDamage>().TakeDamage();
            }
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform rbc in backCar)
        {
            Gizmos.DrawLine(rbc.transform.position, rbc.transform.position + rbc.transform.forward * backCarcheckDistance);
            Gizmos.DrawWireSphere(transform.position, explodeRadius);
        }
    }

    public void AddGuardEffect()
    {
        canDie = false;
    }

    public void RemoveGuardEffect()
    {
        canDie = true;
    }
}
