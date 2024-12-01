using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class DodgeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Dodge>() != null)
        {
            other.GetComponent<Dodge>().Dodge(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Dodge>() != null)
        {
            other.GetComponent<Dodge>().RemoveDodge();
        }
    }
}
