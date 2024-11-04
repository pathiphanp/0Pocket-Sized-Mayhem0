using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ClawGrab : MonoBehaviour
{
    LayerMask _layerMask;
    private void Start()
    {
        _layerMask |= (1 << LayerMask.NameToLayer("Enemy"));
        _layerMask |= (1 << LayerMask.NameToLayer("Civilian"));
    }
    public void GrabObjects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.transform.position, 55, _layerMask);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider.gameObject);
            hitCollider.gameObject.GetComponent<CivilianBehaviour>().Stop();
            hitCollider.gameObject.GetComponent<RagdollOnOffController>().RagdollModeOn();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(this.gameObject.transform.position, 25);
    }
}
