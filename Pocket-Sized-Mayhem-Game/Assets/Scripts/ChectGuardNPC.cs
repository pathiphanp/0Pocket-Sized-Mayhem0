using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class ChectGuardNPC : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GuardProtectAction>() != null)
        {
            other.GetComponent<GuardProtectAction>().GuardProtect(transform.position);
            gameObject.SetActive(false);
        }
    }
}
