using FMODUnity;
using Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDestruction : MonoBehaviour, TakeDamage
{
    [SerializeField] private int _force;
    [SerializeField] private List<GameObject> _pieces;
    [SerializeField] private GameObject _civilians;
    [SerializeField] private TestSpawner _spawner;
    private bool _isDestoyed;
    [field: SerializeField] public EventReference DeathSFX { get; set; }
    void Start()
    {
        _isDestoyed = false;
        _pieces = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject piece = transform.GetChild(i).gameObject;
            _pieces.Add(piece);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Mallet" && !_isDestoyed)
        {
            RuntimeManager.PlayOneShot(DeathSFX, this.gameObject.transform.position);
            this.GetComponent<Rigidbody>().isKinematic = false;
            foreach (GameObject piece in _pieces)
            {
                piece.AddComponent<Rigidbody>();
                Vector3 direction = Random.insideUnitCircle.normalized;
                piece.GetComponent<Rigidbody>().AddForce(direction * _force, ForceMode.Impulse);
            }
            _isDestoyed = true;
            StartCoroutine(AssignDebri());
        }
    }

    IEnumerator AssignDebri()
    {
        yield return new WaitForSeconds(1f);
        foreach (GameObject piece in _pieces)
        {
            piece.layer = LayerMask.NameToLayer("Debris");
        }
    }

    public void TakeDamage()
    {
        RuntimeManager.PlayOneShot(DeathSFX, this.gameObject.transform.position);
        this.GetComponent<Rigidbody>().isKinematic = false;
        foreach (GameObject piece in _pieces)
        {
            piece.AddComponent<Rigidbody>();
            Vector3 direction = Random.insideUnitCircle.normalized;
            piece.GetComponent<Rigidbody>().AddForce(direction * _force, ForceMode.Impulse);
        }
    }

    // IEnumerator private void OnDestroy()
    // {

    // }
}
