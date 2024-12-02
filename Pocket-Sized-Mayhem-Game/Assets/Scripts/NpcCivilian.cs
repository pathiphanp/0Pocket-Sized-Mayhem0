using System.Collections;
using Interface;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public enum NpcState
{
    Walk, Run, Car
}
public class NpcCivilian : MonoBehaviour, TakeDamage, Fear, AddInCar<GameObject>,
Invite, Dodge, OutCar, SetObjectPool<IObjectPool<GameObject>>, SetGuardEffectProtect
{
    Rigidbody rb;
    Collider myCollider;
    [SerializeField] GameObject myModel;
    protected Vector3 target;
    protected Vector3 afterTarget;

    protected TargetType type = TargetType.NPC;

    [HideInInspector] public GameObject targetOut;
    [SerializeField] protected Vector3 newTargetOut;
    [SerializeField] GameObject targetFear;
    [Header("NavMash")]
    protected NavMeshAgent navMeshAgent;
    [SerializeField] float heartbeatDuration;
    Coroutine heartbeat;
    bool findTarget = true;

    [Header("Speed Setting")]
    [SerializeField] float bornSpeed;
    [SerializeField] float fearSpeed;
    [SerializeField] float runAfterFearSpeed;
    [SerializeField] float durationRunAfterFear;
    [SerializeField] float dodgeSpeed;
    protected float afterSpeed;
    float speed;
    [Header("Fear Setting")]
    [SerializeField] float radiusFear;
    [SerializeField] float fearDuration;
    [SerializeField] float cooldownFear;
    protected bool canFear = true;
    Coroutine callfear;
    Coroutine callCooldownFear;
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

    [Header("Die")]
    [SerializeField] GameObject bloodEffect;
    bool onDie = false;

    [Header("Object Pool")]
    IObjectPool<GameObject> myPool { get; set; }
    [HideInInspector] public GameObject poolPosition;
    protected bool onObjectPool = false;
    bool isPaused = false;

    [Header("NotMove")]
    Coroutine checkNotMove;
    Coroutine countNotMove;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        myCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        ResetStatus(); //Test
    }
    #region SetUp
    public virtual void SetUpHumansBorn()
    {
        if (navMeshAgent.enabled)
        {
            speed = bornSpeed;
            afterSpeed = speed;
            navMeshAgent.speed = speed;
            bloodEffect.SetActive(false);
            bloodEffect.gameObject.transform.SetParent(this.transform);
            bloodEffect.transform.localPosition = new Vector3(0, 1.7f, 0);
        }
        //play animation walk
    }
    public virtual void SetUpTarget()
    {
        navMeshAgent.avoidancePriority = Random.Range(0, 100);
        newTargetOut = Random.insideUnitCircle * 5;
        newTargetOut = new Vector3(newTargetOut.x, 0, newTargetOut.y) + targetOut.transform.position;
        target = newTargetOut;
    }

    IEnumerator LoopCheckNotMove()
    {
        yield return new WaitForSeconds(1f);
        CheckNotMove();
        yield return new WaitForSeconds(1f);
        if (checkNotMove != null)
        {
            StopCoroutine(checkNotMove);
            checkNotMove = null;
        }
        checkNotMove = StartCoroutine(LoopCheckNotMove());
    }
    void CheckNotMove()
    {
        if (navMeshAgent.velocity.sqrMagnitude < 0.01f && !onCar)
        {
            StopCoroutine(checkNotMove);
            checkNotMove = null;
            countNotMove = StartCoroutine(CountNotMove());
        }
    }
    IEnumerator CountNotMove()
    {
        float countTime = 1;
        while (countTime >= 0)
        {
            countTime -= Time.deltaTime;
            yield return true;
        }
        FastSetNewTargetNavMash(newTargetOut, bornSpeed);
        checkNotMove = StartCoroutine(LoopCheckNotMove());
    }
    #endregion
    #region TakeDmage
    public TargetType TakeDamage()
    {
        if (!onDie && type == TargetType.NPC)
        {
            Die();
            GameManager._instance.AddPointPlayerKill(type);
            return type;
        }
        else
        {
            ExtraEffetNotDie();
        }
        return TargetType.None;
    }
    public virtual void ExtraEffetNotDie() { }
    #endregion
    #region Die
    public virtual void ExtraEffetDie() { }
    void Die()
    {
        ExtraEffetDie();
        SetStatus(false);
        bloodEffect.transform.SetParent(null);
        bloodEffect.SetActive(true);
        StopMove();
        StopAllCoroutines();
        ReturnToPool();
    }
    #endregion
    #region Add Fear
    public void AddFear()
    {
        if (canFear && !onCar && type != TargetType.Guard)
        {
            navMeshAgent.speed = 0;
            //play animation fear run
            callCooldownFear = StartCoroutine(CooldownFearStatus());
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
        canFear = true;
    }
    IEnumerator DurationFear()
    {
        yield return new WaitForSeconds(fearDuration);
        //play animation run
        StartCoroutine(Run());
        FastSetNewTargetNavMash(newTargetOut, runAfterFearSpeed);
        callfear = null;
    }
    #endregion
    IEnumerator Run()
    {
        yield return new WaitForSeconds(durationRunAfterFear);
        navMeshAgent.speed = bornSpeed;
    }
    #region NavMash
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
        if (navMeshAgent.enabled)
        {
            FastSetNewTargetNavMash(target, speed);
        }
        yield return new WaitForSeconds(heartbeatDuration);
        findTarget = true;
        heartbeat = null;
        HeartbeatNavMash();
    }
    #endregion

    #region  Car
    #region  GoToCar
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
        if (_carTarget != null)
        {
            FastSetNewTargetNavMash(_carTarget.transform.position, runAfterFearSpeed);
        }
        yield return new WaitForSeconds(0.5f);
        while (transform.position != target && !canGetInCar)
        {
            while (isPaused)
            {
                yield return true;
            }
            if (navMeshAgent.enabled)
            {
                if (navMeshAgent.remainingDistance <= getInCarDistance && navMeshAgent.remainingDistance > 0)
                {
                    canGetInCar = true;
                }
            }
            yield return true;
        }
        StopMove();
        yield return new WaitForSeconds(1f);
        _carTarget.AddHumansToCar(this.gameObject);
        ExtraActionInCar(_carTarget);
        callGoToCar = null;
    }
    public virtual void ExtraActionInCar(Car _carTarget) { }
    #endregion

    #region  AddInCar
    public void AddInCar(GameObject _waitPosition)
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        onGoToCar = true;
        onCar = true;
        onInvite = true;
        if (_waitPosition != null)
        {
            transform.SetParent(_waitPosition.transform);
        }
        transform.localPosition = Vector3.zero;
        rb.interpolation = RigidbodyInterpolation.None;
    }
    #endregion
    public Car myCarTarget()
    {
        return car;
    }
    #region OutCar
    public virtual void OutCar(Vector3 _diraction)
    {
        ResetStatus();
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
    #endregion  
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
    #region Dodge
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
            bool onDodge = true;
            // Debug.Log("Dodge Car");
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
            while (!onDodge)
            {
                yield return true;
            }

        }
    }
    public void RemoveDodge()
    {
        if (coroutineDodge != null)
        {
            StopCoroutine(coroutineDodge);
            coroutineDodge = null;
        }
        FastSetNewTargetNavMash(afterTarget, bornSpeed);
        isPaused = false;
    }
    #endregion
    protected void FastSetNewTargetNavMash(Vector3 _targetPosition, float _speed)
    {
        afterSpeed = speed;
        speed = _speed;
        target = _targetPosition;
        if (navMeshAgent.enabled)
        {
            navMeshAgent.destination = _targetPosition;
            navMeshAgent.speed = speed;
        }
    }
    #region Object Pool
    public void AddPool(IObjectPool<GameObject> _objectPool)
    {
        myPool = _objectPool;
    }
    public void ReturnToPool()
    {
        if (myPool != null && !onObjectPool)
        {
            onObjectPool = true;
            myPool.Release(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void ResetStatus()
    {
        targetOut = FindAnyObjectByType<Portal>().gameObject;//Test
        SetStatus(true);
        myCollider.enabled = true;
        SetUpHumansBorn();
        SetUpTarget();
        HeartbeatNavMash();
        checkNotMove = StartCoroutine(LoopCheckNotMove());
    }
    public void SetUpStartGame()
    {
        myPool.Release(this.gameObject);
    }
    #endregion
    #region SetStatus
    void SetStatus(bool _status)
    {
        navMeshAgent.enabled = _status;
        myCollider.enabled = _status;
        myModel.SetActive(_status);
        findTarget = _status;
        canFear = _status;
        onObjectPool = !_status;
        onDie = !_status;
    }
    #endregion
    public void OutPotal()
    {
        //Effect Potal
        Die();
        ReturnToPool();
    }

    public void AddGuardEffect()
    {
        type = TargetType.Guard;
    }

    public void RemoveGuardEffect()
    {
        AddFear();
        type = TargetType.NPC;
    }
}
