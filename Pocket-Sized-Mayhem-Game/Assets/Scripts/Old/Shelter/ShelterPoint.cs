using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer.Equals(6))
        {
           // GameManager.AddPoint();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(6))
        {
            // GameManager.LosePoint();
        }
    }
}
