using System.Collections;
using Interface;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum NpcState
{
    Walk, Run, Car
}
public class NpcCivilian : MonoBehaviour, TakeDamage, Fear, AddInCar<GameObject>, Invite, Dodge, OutCar
{
    Rigidbody rb;
    [SerializeField] GameObject myModel;
    protected Vector3 target;
    protected Vector3 afterTarget;
    [HideInInspector] public GameObject targetOut;
    [SerializeField] protected Vector3 newTargetOut;
    [SerializeField] GameObject targetFear;

    protected NavMeshAgent navMeshAgent;
    [SerializeField] float heartbeatDuration;
    Coroutine heartbeat;
    bool findTarget = true;

    [Header("Speed Setting")]
    [SerializeField] float bornSpeed;
    [SerializeField] float fearSpeed;
    [SerializeField] float afterFearSpeed;
    [SerializeField] float dodgeSpeed;
    float afterSpeed;
    float speed;
    [Header("Fear Setting")]
    [SerializeField] float radiusFear;
    [SerializeField] float fearDuration;
    [SerializeField] float cooldownFear;
    bool canFear = true;
    Coroutine callfear;

    [Header("Car")]
    [SerializeField] float getInCarDistance;
    protected Car car;
    public Car Car
    {
        get
        {
            return car;
        }
    }
    protected bool onInvite = false;
    TargetType type = TargetType.NPC;
    public Coroutine callGoToCar;
    bool onCar = false;
    public bool OnCar
    {
        get
        {
            return onCar;
        }
    }
    protected bool onGoToCar = false;
    public bool OnGoToCar
    {
        get
        {
            return onGoToCar;
        }
    }
    [Header("Dodge")]
    [SerializeField] float dodgeDistance;
    [SerializeField] float dodgeDuration;
    Coroutine coroutineDodge = null;


    bool isPaused = false;
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
    public virtual void SetUpHumansBorn()
    {
        speed = bornSpeed;
        afterSpeed = speed;
        navMeshAgent.speed = speed;
        //play animation walk
    }
    public virtual void SetUpTarget()
    {
        navMeshAgent.avoidancePriority = Random.Range(0, 100);
        newTargetOut = Random.insideUnitCircle * 5;
        newTargetOut = new Vector3(newTargetOut.x, 0, newTargetOut.y) + targetOut.transform.position;
        target = newTargetOut;
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
            FastSetNewTargetNavMash(targetFear.transform.position, fearSpeed);
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
        FastSetNewTargetNavMash(newTargetOut, afterFearSpeed);
        callfear = null;
    }

    protected void StopMove()
    {
        navMeshAgent.speed = 0;
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
        if (navMeshAgent != null)
        {
            navMeshAgent.destination = target;
        }
        yield return new WaitForSeconds(heartbeatDuration);
        findTarget = true;
        heartbeat = null;
        HeartbeatNavMash();
    }
    #region  Car
    public void InviteToCar(Car _car)
    {
        if (!onInvite && !onCar && !onGoToCar)
        {
            onInvite = true;
            callGoToCar = StartCoroutine(GoToCar(_car));
        }
    }
    public virtual IEnumerator GoToCar(Car _carTarget)
    {
        //play animation run
        onGoToCar = true;
        car = _carTarget;
        bool canGetInCar = false;
        FastSetNewTargetNavMash(_carTarget.transform.position, afterFearSpeed);
        yield return new WaitForSeconds(0.5f);
        while (transform.position != target && !canGetInCar)
        {
            while (isPaused)
            {
                yield return true;
            }
            if (navMeshAgent.remainingDistance <= getInCarDistance && navMeshAgent.remainingDistance > 0)
            {
                canGetInCar = true;
            }
            yield return true;
        }
        StopMove();
        yield return new WaitForSeconds(1f);
        _carTarget.AddHumansToCar(this.gameObject);
        ExtarActionInCar(_carTarget);
        callGoToCar = null;
    }
    public virtual void ExtarActionInCar(Car _carTarget) { }

    public void AddInCar(GameObject _waitPosition)
    {
        onGoToCar = true;
        onCar = true;
        onInvite = true;
        transform.SetParent(_waitPosition.transform);
        transform.localPosition = Vector3.zero;
        rb.interpolation = RigidbodyInterpolation.None;
    }
    public Car myCarTarget()
    {
        return car;
    }
    public virtual void OutCar(Vector3 _diraction)
    {
        navMeshAgent.isStopped = true;
        onCar = false;
        onGoToCar = false;
        onInvite = false;
        myModel.SetActive(true);
        transform.SetParent(null);
        navMeshAgent.enabled = false;
        StartCoroutine(EjectedFromCar(_diraction));
    }
    IEnumerator EjectedFromCar(Vector3 _diraction)
    {
        rb.useGravity = true;
        rb.AddForce((transform.up * 10) + (_diraction * 3), ForceMode.Impulse);
        yield return new WaitForSeconds(0.64f);
        rb.useGravity = false;
        rb.Sleep();
        navMeshAgent.enabled = true;
        StopMove();
        yield return new WaitForSeconds(1f);
        ChangeTargetToPortal();
    }
    public void DoActionOnCarStar()
    {
        StopMove();
    }

    public virtual void ChangeTargetToPortal()
    {
        if (callGoToCar != null)
        {
            StopCoroutine(callGoToCar);
            callGoToCar = null;
        }
        onInvite = false;
        findTarget = true;
        onGoToCar = false;
        onCar = false;
        FastSetNewTargetNavMash(newTargetOut, bornSpeed);
    }
    #endregion
    public void Dodge(GameObject _targetDodge)
    {
        Vector3 directionToTarget = transform.position - _targetDodge.transform.position;
        float crossProduct = Vector3.Cross(transform.transform.forward, directionToTarget).y;
        int _direction = 0;
        if (crossProduct < 0)
        {
            // Debug.Log("Target อยู่ทางขวาของ Reference");
            _direction = 0;
        }
        else if (crossProduct > 0)
        {
            // Debug.Log("Target อยู่ทางซ้ายของ Reference");
            _direction = 1;
        }
        else
        {
            // Debug.Log("Target อยู่ตรงกลางกับ Reference (ด้านหน้า/ด้านหลัง)");
            _direction = Random.Range(0, 2);
        }
        if (coroutineDodge == null)
        {
            coroutineDodge = StartCoroutine(DodgeDuration(_direction));
        }
        else
        {
            StopCoroutine(coroutineDodge);
            coroutineDodge = null;
            coroutineDodge = StartCoroutine(DodgeDuration(_direction));
        }
    }
    IEnumerator DodgeDuration(int _direction)
    {
        if (!onCar)
        {
            isPaused = true;
            Vector3 _newtargetDoge = Vector3.zero;
            if (_direction == 0)
            {
                // Debug.Log("ไปทางซ้าย");
                _newtargetDoge += (-transform.right + transform.forward).normalized * dodgeDistance;
            }
            else
            {
                // Debug.Log("ไปทางขวา");
                _newtargetDoge += (transform.right + transform.forward).normalized * dodgeDistance;
            }
            _newtargetDoge += transform.position;
            afterTarget = target;
            FastSetNewTargetNavMash(_newtargetDoge, dodgeSpeed);
            yield return new WaitForSeconds(dodgeDuration);
            FastSetNewTargetNavMash(afterTarget, afterSpeed);
            coroutineDodge = null;
            isPaused = false;
        }
    }

    void FastSetNewTargetNavMash(Vector3 _targetPosition, float _speed)
    {
        afterSpeed = speed;
        speed = _speed;
        target = _targetPosition;
        navMeshAgent.destination = _targetPosition;
        navMeshAgent.speed = speed;
    }


}
