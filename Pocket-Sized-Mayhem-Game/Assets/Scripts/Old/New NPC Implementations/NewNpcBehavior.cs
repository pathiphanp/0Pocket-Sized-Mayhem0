using CharacterMovement;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class NewNpcBehavior : CharacterMovement3D
{
    [Header("Throw Attributes")]
    [SerializeField] private float _upForce;
    [SerializeField] private float _forwardForce;

    [Header("NPC Attributes")]
    [SerializeField] private float point;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private float _fadeSpeed;
    

    [Header("Escape Attributes")]
    [SerializeField] private EndTarget _endTarget;
    [SerializeField] private State _state;
    [SerializeField] private Pattern _pattern;
    [SerializeField] private float _escapeRangeFindRadius;

    [Header("Pattern Attributes")]
    [SerializeField] private float _zigzagHorizontalDistance;
    [SerializeField] private float _zigzagVerticalDistance;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _radius;

    [Header("Fading Attributes")]
    private Renderer _objectRenderer;
    private Material _objectMaterial;

    private float _alpha;
    private float _fadeAmount = 1;
    private float _timer = 0;
    private float _timer2 = 0;
    private float angle = 0;
    private float _cycleCount = 0;
    private bool _fadingOut;

    private bool _targetAssigned = false;
    private GameObject _target;
    private Vector3 _newDirectionVector;

    LayerMask _layerMask;
    LayerMask _civilianTargetLayerMask;
    private bool Stoppep = false;
    [SerializeField] public GameObject _vacuum;
    void Start()
    {
        _objectRenderer = GetComponentInChildren<Renderer>();
        _objectMaterial = _objectRenderer.material;
        
        _layerMask |= (1 << 22);
        _civilianTargetLayerMask |= (1 << 6);
        
        //_alpha = _objectMaterial.GetFloat("_Alpha");
        _newDirectionVector = new Vector3(_zigzagHorizontalDistance,0, _zigzagVerticalDistance);

        _endTarget = (EndTarget) Random.Range(0, 2);
        _state = (State) Random.Range(0, 2);
        _pattern = (Pattern) Random.Range(0, 2);
        //this.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * _upForce + transform.forward * _forwardForce, ForceMode.Impulse);
        //NavMeshAgent.isStopped = true;
        SetEscapeDestination();
    }

    private void SetEscapeDestination()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _escapeRangeFindRadius, _layerMask);
        _target = hitColliders[Random.Range(0, hitColliders.Length)].gameObject;
        MoveTo(_target.transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,_escapeRangeFindRadius);
    }

    private void SetCivilianTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _escapeRangeFindRadius, _civilianTargetLayerMask);
        _target = hitColliders[Random.Range(0, hitColliders.Length)].gameObject;
        MoveTo(_target.transform.position);
    }
    public void AssignVacuumPos(GameObject vacuum)
    {
        if(vacuum != null)
        {
            _objectMaterial.SetVector("_Target", vacuum.transform.position);
        }
    }
    protected override void Update()
    {
        if(Stoppep)
        {
            Stop();
        }
        else
        {
            base.Update();

            angle += Time.deltaTime * _rotationSpeed;
            if (!Stoppep)
            {
                if (_endTarget == EndTarget.NO_TARGET && NavMeshAgent.hasPath)
                {
                    if (_pattern == Pattern.ZIGZAG)
                    {
                        if (NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
                        {
                            if (_newDirectionVector.x > 0)
                            {
                                _newDirectionVector.x = -1 * Random.Range(3, _zigzagHorizontalDistance);
                                _newDirectionVector.z = Random.Range(3, _zigzagVerticalDistance);
                            }
                            else
                            {
                                _newDirectionVector.x *= -1;
                                _newDirectionVector.z = 0;
                            }
                            MoveTo(transform.position + _newDirectionVector);
                        }
                    }
                    else if (_pattern == Pattern.CIRCLE)
                    {
                        _newDirectionVector.x = Mathf.Cos(angle) * _radius;
                        _newDirectionVector.z = Mathf.Sin(angle) * _radius;
                        MoveTo(transform.position + _newDirectionVector);
                    }
                }
                else if ((_endTarget == EndTarget.TARGET || _endTarget == EndTarget.CIVILIAN) && NavMeshAgent.hasPath)
                {
                    MoveTo(_target.transform.position);
                }
            }
        }
        
        
    }

    private IEnumerator Fadeout()
    {
        if (_timer < fadeOutTime)
        {
            
            _fadeAmount = Mathf.Lerp(1, 0, _timer / fadeOutTime);
            _objectMaterial.SetFloat("_Alpha", _fadeAmount);
        }
        yield return null;
    }

    private IEnumerator ColorFadeOut()
    {
        float newTimer = 0;
        while (newTimer < _timer2)
        {
            _fadeAmount = Mathf.Lerp(1, 0, 1 - newTimer / _timer2);
            _objectMaterial.SetFloat("_Alpha", _fadeAmount);
            newTimer += Time.deltaTime;
        }
        yield return StartCoroutine(ColorFadeIn());
    }

    private IEnumerator ColorFadeIn()
    {
        float newTimer = 0;
        while (newTimer < _timer2)
        {
            _fadeAmount = Mathf.Lerp(0, 1,  newTimer / _timer2);
            _objectMaterial.SetFloat("_Alpha", _fadeAmount);
            newTimer += Time.deltaTime;
        }
        yield return StartCoroutine(ColorFadeOut());
    }
    private void AssignTarget()
    {
        if (point < 0)
        {
            _endTarget = EndTarget.CIVILIAN;
        }

        if (_endTarget == EndTarget.TARGET)
        {
            SetEscapeDestination();
        }
        else if (_endTarget == EndTarget.CIVILIAN)
        {
            SetCivilianTarget();
        }
        else
        {
            if (_pattern == Pattern.ZIGZAG)
            {
                MoveTo(transform.position + _newDirectionVector);
            }
        }
        
    }
    public enum EndTarget
    {
        TARGET,
        NO_TARGET,
        CIVILIAN
    }
    public enum Pattern
    {
        ZIGZAG,
        CIRCLE
    }

    public enum State
    {
        PRECISE,
        FRENZY
    }

    public float GetPoint()
    {
        return point;
    }

    protected override void  OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        
    }
    private void OnParticleCollision(GameObject other)
    {
        Stop();
        Debug.Log(other);
        Debug.Log("xd");
    }
}
