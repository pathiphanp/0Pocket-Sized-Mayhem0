using System.Collections;
using Interface;
using UnityEngine;

public class NPCGuard : NpcCivilian
{
    [SerializeField] GuardEffect guardEffect;
    [SerializeField] float durationStun;
    int countDefent = 0;
    int maxDefent = 3;
    public override void SetUpHumansBorn()
    {
        base.SetUpHumansBorn();
        canFear = false;
        type = TargetType.Guard;
    }
    public override IEnumerator GoToCar(Car _carTarget)
    {
        return null;
    }
    public override void ExtraEffetNotDie()
    {
        StartCoroutine(StunDuration());
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
}
