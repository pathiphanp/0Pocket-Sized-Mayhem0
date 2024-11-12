using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interface;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class NpcCivilian : MonoBehaviour, TakeDamage, Fear, AddInCar<GameObject>, Invite, Dodge
{
    Rigidbody rb;
    [SerializeField] GameObject myModel;
    protected Vector3 target;
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
    protected bool onInvite = false;
    TargetType type = TargetType.NPC;
    [HideInInspector] bool onCar = false;
    [Header("Dodge")]
    [SerializeField] float dodgeDistance;
    [SerializeField] float dodgeDuration;
    Coroutine coroutineDodge = null;
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
            FastSetNewTargetNavMash(targetFear.transform.position,fearSpeed);
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
        FastSetNewTargetNavMash(newTargetOut,afterFearSpeed);
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
        navMeshAgent.destination = target;
        yield return new WaitForSeconds(heartbeatDuration);
        findTarget = true;
        heartbeat = null;
        HeartbeatNavMash();
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
        bool canGetInCar = false;
        FastSetNewTargetNavMash(_carTarget.transform.position,afterFearSpeed);
        yield return new WaitForSeconds(0.5f);
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
        _carTarget.AddHumansToCar(this.gameObject);
        ExtarActionInCar(_carTarget);
    }
    public virtual void ExtarActionInCar(Car _carTarget) { }

    public void AddInCar(GameObject _waitPosition)
    {
        onCar = true;
        onInvite = true;
        transform.SetParent(_waitPosition.transform);
        transform.localPosition = Vector3.zero;
        rb.interpolation = RigidbodyInterpolation.None;
    }
    public virtual void OutCar()
    {
        onCar = false;
        transform.SetParent(null);
        ChangeTargetToPortal();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    public void CarStar()
    {
        StopMove();
        myModel.SetActive(false);
    }

    public virtual void ChangeTargetToPortal()
    {
        onInvite = false;
        findTarget = true;
        FastSetNewTargetNavMash(newTargetOut,speed);
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
        Vector3 _newtargetDoge = Vector3.zero;
        if (_direction == 0)
        {
            _newtargetDoge += Vector3.left * dodgeDistance;
        }
        else
        {
            _newtargetDoge -= Vector3.left * dodgeDistance;
        }
        _newtargetDoge += transform.position;
        FastSetNewTargetNavMash(_newtargetDoge, dodgeSpeed);
        yield return new WaitForSeconds(dodgeDuration);
        ChangeTargetToPortal();
        coroutineDodge = null;
    }

    void FastSetNewTargetNavMash(Vector3 _targetPosition, float _speed)
    {
        speed = _speed;
        navMeshAgent.speed = speed;
        target = _targetPosition;
        navMeshAgent.destination = _targetPosition;
    }
}
