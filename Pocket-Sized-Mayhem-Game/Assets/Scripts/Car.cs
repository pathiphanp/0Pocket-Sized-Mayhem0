using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Interface;
using UnityEngine;
using UnityEngine.AI;

public class Car : MonoBehaviour, TakeDamage
{
    Rigidbody rb;
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
    [SerializeField] CarWaitPosition[] waitPosition;
    [SerializeField] Transform[] backCar;
    [SerializeField] float backCarcheckDistance;
    [SerializeField] LayerMask layerMaskBuildubg;
    [SerializeField] float fuel;
    [Header("Dodge Triger")]
    [SerializeField] GameObject dodgeTrigerObj;
    [SerializeField] float dodgeTrigerRadius;
    [Header("Explode")]
    [SerializeField] float explodeRadius;
    [SerializeField] LayerMask npc;

    Vector3 targetOut;
    [HideInInspector] public bool carOnStart = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        carBroken = GetComponent<NavMeshObstacle>();
        carNavMeshAgent = GetComponent<NavMeshAgent>();
        carNavMeshAgent.speed = 0;
        dodgeTrigerObj.GetComponent<SphereCollider>().radius = dodgeTrigerRadius;
    }

    public void StartCar(Vector3 _targetOut)
    {
        fuel = Random.Range(7f, 12f);
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
        carNavMeshAgent.destination = _targetOut;
        targetOut = _targetOut;
        StartCoroutine(CheckForwordToTarget(_targetOut));
        StartCoroutine(FuelDuration());
    }
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
    IEnumerator FuelDuration()
    {
        yield return new WaitForSeconds(fuel);
        carNavMeshAgent.speed = 0;
        carNavMeshAgent.isStopped = true;
        dodgeTrigerObj.SetActive(false);
        StopAllCoroutines();
        rb.Sleep();
        rb.velocity = Vector3.zero;
        EjectHumans();
    }
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
    IEnumerator Heartbeat()
    {
        carNavMeshAgent.destination = targetOut;
        yield return new WaitForSeconds(heartbeatDuration);
        heartbeatNav = StartCoroutine(Heartbeat());
    }

    public TargetType TakeDamage()
    {
        StopCoroutine(heartbeatNav);
        Explode();
        Destroy(this.gameObject);
        return TargetType.Building;
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform rbc in backCar)
        {
            Gizmos.DrawLine(rbc.transform.position, rbc.transform.position + rbc.transform.forward * backCarcheckDistance);
            Gizmos.DrawWireSphere(transform.position, explodeRadius);
        }
    }
}
