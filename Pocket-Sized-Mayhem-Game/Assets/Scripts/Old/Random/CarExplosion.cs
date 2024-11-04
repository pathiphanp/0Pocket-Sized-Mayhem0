using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarExplosion : MonoBehaviour
{
    [field: SerializeField] public EventReference AttackSFX { get; set; }
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _wheelBounce;
    [SerializeField] private MeshRenderer _meshRenderer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Mallet")
        {
            RuntimeManager.PlayOneShot(AttackSFX, this.gameObject.transform.position);
            _wheelBounce.gameObject.SetActive(true);
            _explosion.gameObject.SetActive(true);

            _meshRenderer.enabled = false;
            this.enabled = false;
        }
    }
}
