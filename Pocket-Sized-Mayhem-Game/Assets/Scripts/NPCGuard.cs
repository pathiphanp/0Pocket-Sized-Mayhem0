using System.Collections;
using UnityEngine;

public class NPCGuard : NpcCivilian
{
    [SerializeField] GuardEffect guardEffect;
    [SerializeField] float durationStun;

    public override void SetUpHumansBorn()
    {
        base.SetUpHumansBorn();
        canFear = false;
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
        FastSetNewTargetNavMash(target, afterSpeed);
    }

    //Condition Guard To NPC
    void BrokenGuard()
    {
        // guardEffect.OffGuard();
    }
    //
}
