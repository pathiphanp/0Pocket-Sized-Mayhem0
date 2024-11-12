using System.Collections;
using System.Collections.Generic;
using FMOD;
using Interface;
using UnityEngine;

public class NpcDriver : NpcCivilian
{
    [Header("Driver Setting")]
    [SerializeField] float durationWaitOtherHumans;
    bool findCar = false;
    bool invite = false;
    [SerializeField] GameObject checkCarCollider;

    [SerializeField] public List<Invite> otherHumans = new List<Invite>();

    List<Car> carTarget = new List<Car>();
    float lastCarDistance;
    Coroutine chooseCar;

    public override void SetUpHumansBorn()
    {
        base.SetUpHumansBorn();
        ResetCheckCar();
    }
    public override void SetUpTarget()
    {
        targetOut = FindAnyObjectByType<Portal>().gameObject;//Test
        base.SetUpTarget();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Car>() != null)
        {
            // StopMove();
            // carTarget.Add(other.gameObject.GetComponent<Car>());
            // if (chooseCar != null)
            // {
            //     StopCoroutine(chooseCar);
            //     chooseCar = null;
            //     chooseCar = StartCoroutine(ChooseCar());
            // }
            // else
            // {
            //     chooseCar = StartCoroutine(ChooseCar());
            // }
            // float carDistance = Vector3.Distance(transform.position, other.gameObject.transform.position);
            // if (lastCarDistance == 0)
            // {
            //     lastCarDistance = carDistance;
            //     carTarget = other.gameObject.GetComponent<Car>();
            // }
            // if (carDistance < lastCarDistance)
            // {
            //     lastCarDistance = carDistance;
            //     carTarget = other.gameObject.GetComponent<Car>();
            // }
        }
        if (other.gameObject.GetComponent<Invite>() != null && invite)
        {
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
    IEnumerator ChooseCar(List<Car> _carTarget)
    {
        Car newCarTarget = null;
        UnityEngine.Debug.Log("check many car");
        yield return new WaitForSeconds(1f);
        if (carTarget.Count > 1)
        {
            UnityEngine.Debug.Log("many car");
            for (int i = 0; i < _carTarget.Count; i++)
            {
                float carDistance = Vector3.Distance(transform.position, _carTarget[i].gameObject.transform.position);
                
            }
        }
        else
        {
            UnityEngine.Debug.Log("1 car");
            newCarTarget = _carTarget[0];
        }
        UnityEngine.Debug.Log("Choose car");
        if (!findCar && !newCarTarget.carOnStart)
        {
            findCar = true;
            onInvite = true;
            bool carNotDriver = newCarTarget.CheckHaveNpcDriver(this);//if driver null
            if (carNotDriver)
            {
                StartCoroutine(GoToCar(newCarTarget));
            }
        }
        chooseCar = null;
    }
    public override void ExtarActionInCar(Car _carTarget)
    {
        findCar = true;
        if (_carTarget.AddDriver(this))
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
                oh.ChangeTargetToPortal();
            }
        }
    }
    public void StartDriveCar()
    {
        StartCoroutine(DelayStartCar());
    }

    IEnumerator DelayStartCar()
    {
        invite = false;
        RemoveInviteOtherHumans();
        otherHumans.Clear();
        yield return new WaitForSeconds(1f);
        car.StartCar(newTargetOut);
    }
    public override void OutCar()
    {
        base.OutCar();
        findCar = false;
    }

    public override void ChangeTargetToPortal()
    {
        findCar = false;
        base.ChangeTargetToPortal();
        ResetCheckCar();
    }

    void ResetCheckCar()
    {
        checkCarCollider.SetActive(false);
        checkCarCollider.SetActive(true);
    }
}
