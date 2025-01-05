using System.Collections;
using Interface;
using UnityEngine;

public class NPCGuard : NpcCivilian, GuardProtectAction
{
    [SerializeField] GuardEffect guardEffect;
    [SerializeField] float durationStun;
    [SerializeField] int countDefent = 0;
    int maxDefent = 3;
    Vector3 centarRndMove;

    [Header("Random Positon")]
    [SerializeField] float radiusRnd;
    [Header("Scrap")]
    [HideInInspector] public GuardCheckScrap checkScrap;
    GameObject scrapTarget;

    [Header("Coroutine")]
    Coroutine guardRunToProtect;
    Coroutine stun;
    Coroutine attackScrap = null;
    Coroutine checkMoveToTarget;
    private void Start()
    {
        // type = TargetType.Guard; //Test
    }
    #region SetUP
    public void SetUpTarget(Vector3 _newtarget)
    {
        centarRndMove = _newtarget;
        target = _newtarget;
    }
    public override void SetUpHumansBorn()
    {
        base.SetUpHumansBorn();
        canFear = false;
        type = TargetType.Guard;
    }
    #endregion

    public override void ExtraEffetNotDie()
    {
        if (stun != null)
        {
            StopCoroutine(stun);
            stun = null;
            stun = StartCoroutine(StunDuration());
        }
        else if (stun == null)
        {
            stun = StartCoroutine(StunDuration());
        }
    }
    IEnumerator StunDuration()
    {
        //Stop Move | play animation Stun
        StopMove();
        //return to target | play animation Walk
        countDefent++;
        if (countDefent < maxDefent)
        {
            yield return new WaitForSeconds(durationStun);
            FastSetNewTargetNavMash(target, afterSpeed);
        }
        else
        {
            Die();
        }
        stun = null;
    }
    public override void ExtraEffetDie()
    {
        BrokenGuard();
    }
    //Condition Guard To NPC
    void BrokenGuard()
    {
        guardEffect.OffGuard();
    }

    void CallCheckMoveToTarget()
    {
        if (navMeshAgent.enabled)
        {
            checkMoveToTarget = StartCoroutine(CheckMoveToTarget());
        }
    }
    IEnumerator CheckMoveToTarget()
    {
        while (navMeshAgent.remainingDistance != 0f)
        {
            yield return true;
        }
        yield return new WaitForSeconds(0.5f);
        RndMoveInArea();
    }
    void RndMoveInArea()
    {
        Vector3 rndPoisMove = Random.insideUnitCircle * radiusRnd;
        rndPoisMove = new Vector3(rndPoisMove.x, 0, rndPoisMove.y) + centarRndMove;
        FastSetNewTargetNavMash(rndPoisMove, speed);
        Invoke("CallCheckMoveToTarget", 0.5f);

    }
    public override void CallCheckNotMove()
    {
        //Star Game Set Target
        Invoke("CallCheckMoveToTarget", 0.5f);
    }

    public void GuardProtect(Vector3 _attackArea)
    {
        //Play Animation run Guard
        if (checkMoveToTarget != null)
        {
            StopCoroutine(checkMoveToTarget);
            checkMoveToTarget = null;
        }
        navMeshAgent.enabled = false;
        if (guardRunToProtect == null)
        {
            guardRunToProtect = StartCoroutine(RunToHammerAttack(_attackArea));
        }
    }
    IEnumerator RunToHammerAttack(Vector3 _attackArea)
    {
        Vector3 direction = (_attackArea - transform.position).normalized;
        direction.y = 0; // ล็อกแกน Y
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;
        Vector3 _diraction = (transform.position - _attackArea).normalized;
        _diraction.y = 0;
        rb.AddForce(-_diraction * 30, ForceMode.Impulse);
        float _distance = Vector3.Distance(transform.position, _attackArea);
        while (_distance >= 1f)
        {
            _distance = Vector3.Distance(transform.position, _attackArea);
            yield return true;
        }
        rb.Sleep();
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.5f);
        navMeshAgent.enabled = true;
        RndMoveInArea();
        guardRunToProtect = null;
    }
    public void CallAttackScarp(GameObject _scrap)
    {
        StopMove();
        FastSetNewTargetNavMash(_scrap.transform.position, walkSpeed);
        if (attackScrap == null)
        {
            scrapTarget = _scrap;
            checkScrap.gameObject.SetActive(false);
            attackScrap = StartCoroutine(MoveToTargetScrap());
        }
    }
    private void Update()
    {
        // Debug.Log(navMeshAgent.remainingDistance);
    }
    IEnumerator MoveToTargetScrap()
    {
        // yield return new WaitForSeconds(0.5f);
        while (navMeshAgent.remainingDistance == 0)//Check remainingDistance = 0
        {
            yield return true;
        }
        while (navMeshAgent.velocity.magnitude != 0)//Check move character
        {
            yield return true;
        }
        //Play animation attack
        attackScrap = null;
        Attack();
    }
    void Attack()
    {
        if (scrapTarget != null)
        {
            scrapTarget.GetComponent<TakeDamage>().TakeDamage();
        }
        scrapTarget = null;
        RndMoveInArea();
        checkScrap.gameObject.SetActive(true);
    }

    #region NotWork
    public override void SetUpTarget(Vector3 _targetOut, float radiusTarget) { }
    public override void OutPotal() { }
    public override IEnumerator GoToCar(Car _carTarget) { return null; }
    #endregion
}
