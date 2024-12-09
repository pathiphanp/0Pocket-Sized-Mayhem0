using System.Collections;
using Interface;
using UnityEngine;

public class NPCGuard : NpcCivilian, GuardProtectAction
{
    [SerializeField] GuardEffect guardEffect;
    [SerializeField] float durationStun;
    int countDefent = 0;
    int maxDefent = 3;

    Vector3 centarRndMove;

    [HideInInspector] public GuardCheckScrap checkScrap;

    [Header("Random Positon")]
    [SerializeField] float radiusRnd;
    [Header("CheckScrap")]
    [SerializeField] float checkRadius;
    [Header("Coroutine")]
    Coroutine guardRunToProtect;
    Coroutine stun;
    Coroutine attackScrap;

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
        if (stun == null)
        {
            stun = StartCoroutine(StunDuration());
        }
    }
    IEnumerator StunDuration()
    {
        //Stop Move | play animation Stun
        StopMove();
        yield return new WaitForSeconds(durationStun);
        //return to target | play animation Walk
        countDefent++;
        if (countDefent < maxDefent)
        {
            FastSetNewTargetNavMash(target, afterSpeed);
        }
        else
        {
            type = TargetType.NPC;
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
        StartCoroutine(CheckMoveToTarget());
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
        navMeshAgent.enabled = false;
        if (guardRunToProtect == null)
        {
            guardRunToProtect = StartCoroutine(RunToHammerAttack(_attackArea));
        }
    }
    IEnumerator RunToHammerAttack(Vector3 _attackArea)
    {
        transform.LookAt(_attackArea);
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
            attackScrap = StartCoroutine(MoveToTargetScrap());
        }
    }
    IEnumerator MoveToTargetScrap()
    {
        if (navMeshAgent.remainingDistance >= 1)
        {
            yield return true;
        }
        //Play animation attack
        attackScrap = null;
    }
    void Attack(GameObject _scrap)
    {
        _scrap.GetComponent<TakeDamage>();
        checkScrap.gameObject.SetActive(true);
    }

    #region NotWork
    public override void SetUpTarget() { }
    public override void OutPotal() { }
    public override IEnumerator GoToCar(Car _carTarget) { return null; }
    #endregion
}
