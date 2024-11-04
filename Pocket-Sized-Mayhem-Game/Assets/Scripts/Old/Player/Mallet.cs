
using CharacterMovement;
using FMODUnity;
using PrimeTween;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using static UnityEngine.Timeline.DirectorControlPlayable;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class Mallet : Weapon
{
    [field: SerializeField] public EventReference AttackSFX { get; set; }
    [field: SerializeField] public EventReference PukeSFX { get; set; }
    
    [SerializeField] private GameObject _debrisVFX;
    
    [SerializeField] private GameObject _impact;
    [SerializeField] private GameObject _target;
    [SerializeField] private float _originalStartY;
    [SerializeField] private float _targetOffset;

    [SerializeField] private Animator _malletAnimator;
    [SerializeField] private GameObject _impactPos;
    [SerializeField] private GameObject _malletHandle;
    [SerializeField] private GameObject _floor;
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] private GameObject _mouth;
    private InputAction vacuumAction;
    private InputAction pukeAction;
    private Vector3 _mousePos;
    private Vector3 hitpoint;
    private RaycastHit _hit;
    private LayerMask _layerMask;
    private LayerMask _vacuumLayerMask;
    private Vector3 _hitTargetpos;
    [SerializeField] private float _impactRadius;
    [SerializeField] private float _malletMovementSpeed ;
    private bool _isAttacking = false;
    private int layerAsLayerMask;
    private int _attackMode;
    private bool isVacuuming = false;
    [SerializeField]private int pullIntensity;
    List<GameObject> enemies;
    [SerializeField] private Vacuum _vacuum;
    [SerializeField] private float _hungerExpense;
    
    [SerializeField] private ParticleSystem _particleSystem;
    private bool puking = false;
    private void Start()
    {
        
        _attackMode = 0;
        _vacuumLayerMask |= (1 << LayerMask.NameToLayer("Enemy"));
        _vacuumLayerMask |= (1 << LayerMask.NameToLayer("Civilian"));
        enemies = new List<GameObject>();
    }
    public void OnRestartScene()
    {
        SceneManager.LoadScene("GameScene - M3");
    }

    public void OnIncreasePoint()
    {
        PlayerStats.Points += 10;
    }
    public void OnDecreasePoint()
    {
        Debug.Log("hehe");
        PlayerStats.Points -= 10;
    }
    private void OnEnable()
    {
        // Get the action map and the specific action for the mouse click
        var playerActionMap = inputActions.FindActionMap("Player");
        vacuumAction = playerActionMap.FindAction("Vacuum");
        pukeAction = playerActionMap.FindAction("Release");
        
        vacuumAction.canceled += OnMouseRelease;
        pukeAction.canceled += OnMouseReleasePuke;

        // Enable the action
        vacuumAction.Enable();
        pukeAction.Enable();
    }
    private void OnMouseRelease(InputAction.CallbackContext context)
    {
        _malletAnimator.SetBool("VacuumReleased", true);
        _vacuum.VacuumOff();
    }
    private void OnMouseReleasePuke(InputAction.CallbackContext context)
    {
        var emission = _particleSystem.emission;
        puking = false;
        emission.rateOverTime = 0;
    }
    public void OnRelease()
    {
        if(PlayerStats.Hunger >0)
        {

            RuntimeManager.PlayOneShot(PukeSFX, this.gameObject.transform.position);
            _particleSystem.Play();
            var emission = _particleSystem.emission;
            emission.rateOverTime = 100;
            puking = true;
        }
        
    }

    private void OnDisable()
    {
        // Unsubscribe from the events and disable the action
        vacuumAction.canceled -= OnMouseRelease;
        vacuumAction.Disable();
        pukeAction.canceled -= OnMouseReleasePuke;
        pukeAction.Disable();
    }
    
    public override void Fire()
    {
        _malletAnimator.SetFloat("Direction", 1);
        if ( _attackMode == 0)
        {
            
            _malletAnimator.SetTrigger("Swing");
            _layerMask = LayerMask.GetMask("Floor");
        }
    }

    public void OnVacuum()
    {
        if(_attackMode==1)
        {
             
            _vacuum.VacuumOn();
            isVacuuming = true;
            _malletAnimator.SetTrigger("Vacuum");
            _malletAnimator.SetBool("VacuumReleased", false);
        }
        
    }
    public void OnSelectMallet()
    {
        if (_attackMode == 1)
        {
            gameObject.tag = "Mallet";
            _attackMode = 0;
            _malletAnimator.SetTrigger("SwitchMallet");
            _vacuum.VacuumOff();
        }
    }

    public void OnSelectClaw()
    {
        if (_attackMode == 0)
        {
            gameObject.tag = "Vacuum";
            _attackMode = 1;
            _malletAnimator.SetTrigger("SwitchVacuum");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_mouth.transform.position, 15);
    }
    private void Update()
    {
        if(puking)
        {
            PlayerStats.Hunger-=0.5f;
        }
        if (PlayerStats.Hunger<=0)
        {
            var emission = _particleSystem.emission;
            emission.rateOverTime = 0;
            puking = false;
        }
        _mousePos = Input.mousePosition;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(_mousePos), out _hit, Mathf.Infinity, _layerMask))
        {
            return;
        }
        
        _hitTargetpos = _hit.point;
        _target.transform.position = _hitTargetpos;

        this.gameObject.transform.position = _hit.point;
        hitpoint = _hit.point;
        hitpoint.z -= _targetOffset;
        hitpoint.y = _originalStartY;
        _malletHandle.gameObject.transform.position = Vector3.MoveTowards(_malletHandle.gameObject.transform.position, hitpoint, _malletMovementSpeed);
    }

    public void DisableColliders()
    {
        GetComponentInChildren<Collider>().enabled = false;
        //_isAttacking = false;
    }

    public void EnableColldiers()
    {
        GetComponentInChildren<Collider>().enabled = true;
        //_isAttacking = true;

    }
    public void ImpactEffects()
    {
        //Play Effect Attack
        GameObject impact = Instantiate(_impact, _impactPos.transform.position+Vector3.up, _impact.transform.rotation);
        impact.GetComponent<VisualEffect>().Play();
        if (!AttackSFX.IsNull)
        {
            RuntimeManager.PlayOneShot(AttackSFX, this.gameObject.transform.position);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Debris"))
        {
            Instantiate(_debrisVFX, other.transform.position, _debrisVFX.transform.rotation);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            
            enemies.Add(other.gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Shield"))
        {
            _malletAnimator.SetFloat("Direction", -1);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        enemies.Remove(other.gameObject);
    }
}

