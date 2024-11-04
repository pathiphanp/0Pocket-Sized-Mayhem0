using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using FMODUnity;
public class CivilianDeath : MonoBehaviour
{
    [field: SerializeField] public EventReference DeathSFX { get; set; }

    [SerializeField] private GameObject _bloodEffect;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private CapsuleCollider _triggerCollider;
    [SerializeField] private RagdollOnOffController _ragdollController;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rb;
    //[SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private NewNpcBehavior _civilianBehaviour;
    [SerializeField] private int _animNo;
    
    public event EventHandler<GameObject> OnKilled;
    public event EventHandler<GameObject> OnCaptured;
    private bool _pointGiven;
    
    private void Start()
    {
        _pointGiven = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Mallet")
        {
            
            _capsuleCollider.enabled = false;
            _civilianBehaviour.enabled = false;

            _ragdollController.RagdollModeOn();
            _ragdollController.DeathBounce();
            
            GameObject blood = Instantiate(_bloodEffect, this.gameObject.transform.position, this.gameObject.transform.rotation);
            blood.GetComponent<VisualEffect>().Play();
            OnKilled?.Invoke(this, this.gameObject);
            
            if (!_pointGiven)
            {
                PlayerStats.Points += _civilianBehaviour.GetPoint();
                _pointGiven= true;
            }
            if (!DeathSFX.IsNull)
            {
                RuntimeManager.PlayOneShot(DeathSFX, this.gameObject.transform.position);
            }

            this.enabled = false;
            _triggerCollider.enabled = false;
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    { if (collision.gameObject.tag == "Vacuum")
        {
            this.gameObject.SetActive(false);
            PlayerStats.Hunger += 5;
        }
    }
    public void DeathByCriminal()
    {
        int animNo = UnityEngine.Random.Range(0, _animNo);

        _animator.SetFloat("DeathNo",(float) animNo);
        _animator.SetTrigger("Death");
        _rb.isKinematic = true;
        _capsuleCollider.enabled = false;
        _civilianBehaviour.Stop();

        OnKilled?.Invoke(this, this.gameObject);

        if (!DeathSFX.IsNull)
        {
            RuntimeManager.PlayOneShot(DeathSFX, this.gameObject.transform.position);
        }
    }
}
