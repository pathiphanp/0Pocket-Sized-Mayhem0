using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Vacuum : MonoBehaviour
{
    [SerializeField] List<GameObject> pulledObjects;
    [SerializeField] List<GameObject> capturedObjects;
    private LayerMask _vacuumableObjects;
    private bool _vacuumOn = false;
    [SerializeField]private MeshCollider _rayCollider;
    [SerializeField]private CapsuleCollider _storeCollider;
    [SerializeField]private GameObject _Ray;
    Vector3 endScale;
    Vector3 RayStartScale;
    [SerializeField] float raychangespeed;
    [SerializeField] private GameObject _pukeVFX;
    [field: SerializeField] public EventReference VacuumSFX { get; set; }
    private void Start()
    {
        RayStartScale = _Ray.transform.localScale;
        _Ray.transform.localScale = Vector3.zero;
        endScale = new Vector3(3,1,3);
        pulledObjects = new List<GameObject>();
        capturedObjects = new List<GameObject>();
        _vacuumableObjects |= (1 << LayerMask.NameToLayer("Civilian"));
    }
    private void Update()
    {
        
        if (_vacuumOn)
        {
            _Ray.transform.localScale = Vector3.Lerp(_Ray.transform.localScale, RayStartScale, raychangespeed * Time.deltaTime);
        }
        else
        {
            _Ray.transform.localScale = Vector3.Lerp(_Ray.transform.localScale, Vector3.zero, raychangespeed * Time.deltaTime);
        }
        if(PlayerStats.Hunger >= 100)
        {
            PlayerStats.Hunger = 100;
        }
    }
    private void FixedUpdate()
    {
        if (_vacuumOn)
        {
            foreach (GameObject enemy in pulledObjects)
            {
                Vector3 pullForce = (this.gameObject.transform.position - enemy.transform.position).normalized/ Vector3.Distance(this.gameObject.transform.position, enemy.transform.position) * 50;
                enemy.GetComponent<Rigidbody>().velocity  = (new Vector3(pullForce.x * 2, pullForce.y *4, pullForce.z * 2));
                enemy.GetComponent<NewNpcBehavior>().AssignVacuumPos(this.gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_vacuumableObjects == (_vacuumableObjects | (1 << other.gameObject.layer)))
        {
            pulledObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<NewNpcBehavior>().AssignVacuumPos(null);
        pulledObjects.Remove(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_vacuumableObjects == (_vacuumableObjects | (1 << collision.gameObject.layer)) && _vacuumOn)
        {
            capturedObjects.Add(collision.gameObject);
            collision.rigidbody.velocity = Vector3.zero;
            if (!collision.gameObject.TryGetComponent<FixedJoint>(out FixedJoint joint))
            {
                collision.gameObject.AddComponent<FixedJoint>();
                collision.gameObject.GetComponent<FixedJoint>().connectedBody = this.GetComponent<Rigidbody>();
                collision.gameObject.transform.parent = this.gameObject.transform;
            }
        }
    }
    public void ReleaseAll()
    {
        foreach (GameObject enemy in capturedObjects)
        {
            Destroy(enemy.GetComponent<FixedJoint>());
            enemy.transform.parent = null;
        }
    }
    public void Puke()
    {
        Instantiate(_pukeVFX, this.gameObject.transform.position, this.gameObject.transform.rotation);
    }
    public void VacuumOn()
    {
        RuntimeManager.PlayOneShot(VacuumSFX, this.gameObject.transform.position);
        _vacuumOn = true;
        _storeCollider.enabled = true;
        _rayCollider.enabled = true;
    }

    public void VacuumOff()
    {
        _vacuumOn = false;
        _storeCollider.enabled = false;
        _rayCollider.enabled = false;
    }
}
