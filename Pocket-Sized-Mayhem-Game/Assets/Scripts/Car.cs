using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] float speedMove;
    [SerializeField] CarWaitPosition[] waitPosition;

    void StartCar()
    {
        //off all humans
        foreach (CarWaitPosition w in waitPosition)
        {
            w.humans.GetComponent<AddInCar<GameObject>>().invisible();
        }
    }
    public void AddHumans(GameObject _human)
    {
        CarWaitPosition _carWaitPosition = null;
        //Find empty waitPosition
        foreach (CarWaitPosition w in waitPosition)
        {
            if (w.empty)
            {
                _carWaitPosition = w;
            }
        }
        _carWaitPosition.humans = _human;
        _human.GetComponent<AddInCar<GameObject>>().AddInCar(_carWaitPosition.waitPosition);
    }
}
