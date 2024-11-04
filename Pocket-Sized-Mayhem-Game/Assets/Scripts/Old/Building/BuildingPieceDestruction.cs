using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPieceDestruction : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Mallet")
        {
            this.GetComponent<Rigidbody>().useGravity = true;
            Vector3 direction = Random.insideUnitCircle.normalized;
            this.GetComponent<Rigidbody>().AddForce(direction * 100, ForceMode.Impulse);
        }
    }
}
