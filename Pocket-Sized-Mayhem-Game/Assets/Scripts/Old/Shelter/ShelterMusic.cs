using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class ShelterMusic : MonoBehaviour
{
    [field: SerializeField] public EventReference AttackSFX { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        if (!AttackSFX.IsNull)
        {
            RuntimeManager.PlayOneShot(AttackSFX, this.gameObject.transform.position);
        }
    }

}
