using System.Collections;
using Interface;
using UnityEngine;
using UnityEngine.AI;

public class Car : MonoBehaviour, TakeDamage
{
    [SerializeField] float speedMove;
    [SerializeField] float speedRotation;
    [SerializeField] float speedMoveBack;
    [SerializeField] float speedMoveForward;
    [SerializeField] float durationMoveForward;
    [SerializeField] float heartbeatDuration;
    Coroutine heartbeatNav;
    [SerializeField] CarWaitPosition[] waitPosition;
    NavMeshAgent carNavMeshAgent;
    Rigidbody rb;

    [SerializeField] float fuel;

    NpcDriver npcDriver = null;

    int countHumansInCar = 0;
    [Header("CarPart")]
    [SerializeField] Transform[] backCar;
    [SerializeField] float backCarcheckDistance;
    [SerializeField] LayerMask layerMaskBuildubg;

    [Header("Explode")]
    [SerializeField] float explodeRadius;
    [SerializeField] LayerMask npc;


    Vector3 targetOut;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        carNavMeshAgent = GetComponent<NavMeshAgent>();
        carNavMeshAgent.isStopped = true;
        carNavMeshAgent.speed = 0;
        Explode();
    }
    public void StartCar(Vector3 _targetOut)
    {
        //off all humans
        foreach (CarWaitPosition w in waitPosition)
        {
            if (w.humans != null)
            {
                w.humans.GetComponent<AddInCar<GameObject>>().CarStar();
                w.humans.gameObject.SetActive(false);
            }
        }
        carNavMeshAgent.isStopped = false;
        carNavMeshAgent.destination = _targetOut;
        targetOut = _targetOut;
        StartCoroutine(CheckForwordToTarget(_targetOut));
        StartCoroutine(FuelDuration());
    }
    public void AddHumans(GameObject _human)
    {
        CarWaitPosition _carWaitPosition = FindEmptyWaitPosition();
        _carWaitPosition.humans = _human;
        _human.GetComponent<AddInCar<GameObject>>().AddInCar(_carWaitPosition.waitPosition);
        _carWaitPosition.empty = false;
        countHumansInCar++;
        if (countHumansInCar == 4)
        {
            npcDriver.StartDriveCar();
        }
    }
    public void AddNpcDriver(NpcDriver _npcDriver)
    {
        if (npcDriver == null)
        {
            npcDriver = _npcDriver;
        }
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
        while (angle >= 25f)
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
                    carNavMeshAgent.speed = 0;
                    StartCoroutine(MoveForward());
                    yield return new WaitForSeconds(durationMoveForward);
                }
            }
            //move back not hit anything
            carNavMeshAgent.speed = speedRotation;
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
        //kick Humans
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
