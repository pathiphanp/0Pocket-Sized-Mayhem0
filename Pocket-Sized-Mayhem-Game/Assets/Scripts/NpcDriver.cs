using System.Collections;
using System.Collections.Generic;
using FMOD;
using Interface;
using UnityEngine;

public class NpcDriver : NpcCivilian
{
    [Header("Driver Setting")]
    [SerializeField] float durationWaitOtherHumans;
    Vector3 carTarget;
    bool findCar = true;
    bool invite = false;
    [SerializeField] GameObject checkCarCollider;

    List<Invite> otherHumans = new List<Invite>();
    public override void SetUpTarget()
    {
        targetOut = FindAnyObjectByType<Portal>().gameObject;//Test
        base.SetUpTarget();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Car>() != null && findCar)
        {
            findCar = false;
            StartCoroutine(GoToCar(other.gameObject.GetComponent<Car>()));
        }
        if (other.gameObject.GetComponent<Invite>() != null && invite)
        {
            UnityEngine.Debug.Log(other.gameObject.name);
            otherHumans.Add(other.gameObject.GetComponent<Invite>());
            InviteOtherHumans();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Invite>() != null && invite)
        {
            otherHumans.Remove(other.gameObject.GetComponent<Invite>());
        }
    }
    public override void ExtarActionInCar()
    {
        int rndInviteOtherHumans = UnityEngine.Random.Range(0, 1);
        if (rndInviteOtherHumans == 0)//Invite other Humans
        {
            invite = true;
            checkCarCollider.gameObject.SetActive(false);
            checkCarCollider.gameObject.SetActive(true);
            //Play animIvite
            StartCoroutine(WaitOtherHumans());

        }
        else//go alone
        {
            StartDriveCar();
        }
    }
    IEnumerator WaitOtherHumans()
    {
        yield return new WaitForSeconds(durationWaitOtherHumans);
        invite = false;
        //Stop Play Animation
        StartDriveCar();
    }

    void InviteOtherHumans()
    {
        if (otherHumans.Count > 0)
        {
            foreach (Invite oh in otherHumans)
            {
                oh.InviteToCar(car);
            }
        }
    }
    void RemoveInviteOtherHumans()
    {
        if (otherHumans.Count > 0)
        {
            foreach (Invite oh in otherHumans)
            {
                oh.OutCar();
            }
        }
    }
    public void StartDriveCar()
    {
        invite = false;
        RemoveInviteOtherHumans();
        otherHumans.Clear();
        car.StartCar(newTargetOut);
    }
}
