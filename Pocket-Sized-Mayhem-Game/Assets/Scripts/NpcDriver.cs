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

    public List<Invite> otherHumans = new List<Invite>();

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
        if (other.gameObject.GetComponent<Car>() != null && other.gameObject.GetComponent<Car>().npcDriver == null && !findCar)
        {
            StopMove();
            carTarget.Add(other.gameObject.GetComponent<Car>());

            if (chooseCar != null)
            {
                StopCoroutine(chooseCar);
                chooseCar = null;
                chooseCar = StartCoroutine(ChooseCar(carTarget));
            }
            else
            {
                chooseCar = StartCoroutine(ChooseCar(carTarget));
            }
        }
        if (other.gameObject.GetComponent<Invite>() != null && invite)
        {
            otherHumans.Add(other.gameObject.GetComponent<Invite>());
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
        yield return new WaitForSeconds(1f);
        if (carTarget.Count > 1)
        {
            // UnityEngine.Debug.Log("many car");
            for (int i = 0; i < _carTarget.Count; i++)
            {
                float carDistance = Vector3.Distance(transform.position, _carTarget[i].gameObject.transform.position);
                if (lastCarDistance == 0)
                {
                    lastCarDistance = carDistance;
                    newCarTarget = _carTarget[i];
                }
                if (carDistance < lastCarDistance)
                {
                    lastCarDistance = carDistance;
                    newCarTarget = _carTarget[i];
                }
            }
        }
        else
        {
            // UnityEngine.Debug.Log("1 car");
            newCarTarget = _carTarget[0];
        }
        if (newCarTarget != null)
        {
            if (!newCarTarget.carOnStart)
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
        yield return new WaitForSeconds(0.5f);
        InviteOtherHumans();
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
                // UnityEngine.Debug.Log(oh);
                oh.InviteToCar(car);
            }
        }
    }
    void RemoveInviteOtherHumans()
    {
        if (otherHumans.Count > 0)
        {
            for (int i = 0; i < otherHumans.Count; i++)
            {
                if (otherHumans[i].myCarTarget() == car)
                {
                    otherHumans[i].ChangeTargetToPortal();
                }
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
    public override void OutCar(Vector3 _diraction)
    {
        base.OutCar(_diraction);
        findCar = false;
    }

    public override void ChangeTargetToPortal()
    {
        if (!onGoToCar && !OnCar)
        {
            findCar = false;
            onGoToCar = false;
            base.ChangeTargetToPortal();
            ResetCheckCar();
        }
    }

    void ResetCheckCar()
    {
        checkCarCollider.SetActive(false);
        checkCarCollider.SetActive(true);
    }
}
