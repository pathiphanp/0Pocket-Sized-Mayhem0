using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Claw : Weapon
{
    [SerializeField] private GameObject _target;
    [SerializeField] private GameObject _grabbedObject;
    [SerializeField] private FixedJoint _joint;

    [SerializeField] private bool _fired = false;
    [SerializeField] private bool _grabbed = false;
    [SerializeField] private int _descendSpeed;
    [SerializeField] private float _originalStartY;

    private Vector3 _mousePos;
    private Vector3 hitpoint;
    private RaycastHit hit;
    [SerializeField] private Animator _animator;
    LayerMask _layerMask;

    private void Start()
    {
        _layerMask |= (1 << LayerMask.NameToLayer("Enemy"));
        _layerMask |= (1 << LayerMask.NameToLayer("Civilian"));
    }
    public override void Fire()
    {
        _animator.SetTrigger("Fired");
    }
   
    IEnumerator DisableCollider()
    {
        this.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(2);
        //this.GetComponent<Collider>().enabled = true;
    }

    public  void OnRelease()
    {
        StartCoroutine(DisableCollider());
        Destroy(_grabbedObject.GetComponent<FixedJoint>());
        if (_grabbedObject.GetComponent<RagdollOnOffController>())
        {
            //_grabbedObject.GetComponent<NavMeshAgent>().enabled = true;
            _grabbedObject.GetComponent<Animator>().enabled = true;
            _grabbedObject.GetComponent<Rigidbody>().useGravity = true;
            _grabbedObject.GetComponent<RagdollOnOffController>().enabled = true;
            _grabbedObject.GetComponent<CapsuleCollider>().enabled = true;
        }
        _grabbedObject = null;

        _fired = false;
    }

    private void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("ClawClose"))
        {
            
        }
        else
        {
            _mousePos = Input.mousePosition;
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(_mousePos), out hit))
            {
                return;
            }

            _target.transform.position = hit.point;

            this.transform.position = hit.point;
            hitpoint = hit.point;
            hitpoint.y = _originalStartY;
            transform.position = hitpoint;
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    _fired = false;
    //    _grabbed = true;
    //    if (_grabbedObject == null)
    //    {
    //        _fired = false;
    //        _grabbed = true;
    //    }
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    _fired = false;
    //    _grabbed = true;
    //    if (_grabbedObject == null)
    //    {
    //        if (other.gameObject.GetComponent<Animator>())
    //        {
    //            _grabbedObject = other.gameObject;
    //            _grabbedObject.GetComponent<CapsuleCollider>().enabled = false;
    //            _grabbedObject.GetComponent<Rigidbody>().useGravity = false;
    //            _grabbedObject.GetComponent<NavMeshAgent>().enabled = false;
    //            _grabbedObject.GetComponent<Animator>().enabled = false;
    //            _grabbedObject.AddComponent<FixedJoint>();
    //            _grabbedObject.GetComponent<FixedJoint>().connectedBody = this.GetComponent<Rigidbody>();
    //            _grabbedObject.GetComponent<RagdollOnOffController>().enabled = false;
    //        }
    //        else if (other.gameObject.GetComponent<BuildingPieceDestruction>())
    //        {
    //            _grabbedObject = other.gameObject;
    //            _grabbedObject.AddComponent<FixedJoint>();
    //            _grabbedObject.GetComponent<FixedJoint>().connectedBody = this.GetComponent<Rigidbody>();
    //        }
    //    }
    //}
}
