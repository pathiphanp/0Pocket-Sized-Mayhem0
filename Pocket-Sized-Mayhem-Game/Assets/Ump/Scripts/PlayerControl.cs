using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer _renderer;
    [Header("HemmerControl")]
    [SerializeField] GameObject hammerHandlePivot;
    [SerializeField] GameObject hammerFacePivot;
    [SerializeField] GameObject targetHitObject;
    [SerializeField] GameObject targetHitPosition;
    [SerializeField] LayerMask ground;

    [SerializeField] HammerAttack hammerAttack;
    [Header("Hammer Setting")]
    [Header("Hammer hight Position Setting")]
    [SerializeField] float hightHammerFromGround;
    [Header("Target Attack Radius Setting")]
    [SerializeField] float targetHammerRadius;
    [Header("Hammer Attack Setting")]
    [Header("Fade Weapon")]
    [SerializeField] float speedFadeIn;
    [SerializeField] float speedFadeOut;
    Coroutine startFade;
    [Header("Charge")]
    [SerializeField] float hammerChargeSpeed;
    [SerializeField] float chargeBack;
    [SerializeField] float chargeUp;
    [SerializeField] float chargeDuration;
    [SerializeField] float hammerChargeRotationSpeed;
    [Header("AttackDown")]
    [SerializeField] float hammerMoveSpeed;
    [SerializeField] float attackUpDuration;
    [SerializeField] float hammerDownRotationSpeed;
    [Header("HammerUp")]
    [SerializeField] float hammerUpRotationSpeed;

    [SerializeField] bool canMove = false;
    bool canAttack = true;
    bool onAttack = false;


    public float sensitivity = 0.1f;
    private Vector3 lastMousePosition;


    // Start is called before the first frame update
    void Start()
    {
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.lockState = CursorLockMode.Confined;
        SetUpHammer();
        lastMousePosition = Input.mousePosition;

    }
    void SetUpHammer()
    {
        targetHitObject.transform.localScale = new Vector3(targetHammerRadius, targetHitObject.transform.localScale.y, targetHammerRadius);
        Vector3 _hammer = hammerHandlePivot.transform.position;
        hammerHandlePivot.transform.position = new Vector3(_hammer.x, hightHammerFromGround, _hammer.z);

    }
    // Update is called once per frame
    void Update()
    {
        Move();
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            HammerAttack();
        }
    }
    void Move()
    {
        if (!onAttack)
        {
            // targetHitPosition.transform.position += new Vector3(sensitivity, 0, sensitivity).normalized;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                if (lastMousePosition != Input.mousePosition)
                {
                    canMove = true;
                }
                else
                {
                    canMove = false;
                }

            }
            if (canMove)
            {
                targetHitPosition.transform.position = hit.point;
                // Vector3 currentMousePosition = Input.mousePosition;
                // float deltaX = currentMousePosition.x - lastMousePosition.x;
                // float deltaY = currentMousePosition.y - lastMousePosition.y;
                // float mouseX = Input.GetAxis("Mouse X"); // การเปลี่ยนแปลงของตำแหน่งเมาส์ในแกน X
                // float mouseY = Input.GetAxis("Mouse Y"); // การเปลี่ยนแปลงของตำแหน่งเมาส์ในแกน Y
                // //move Target
                // targetHitPosition.transform.position += new Vector3(mouseX * sensitivity, 0,
                // mouseY * sensitivity).normalized;
                // lastMousePosition = currentMousePosition;
            }
        }
    }

    void HammerAttack()
    {
        canMove = false;
        canAttack = false;
        onAttack = true;
        StartCoroutine(HammerAction());
    }

    IEnumerator HammerAction()
    {
        Vector3 startHammerPoint = hammerHandlePivot.transform.position;
        Vector3 hammerChargePos = new Vector3(hammerHandlePivot.transform.localPosition.x + chargeBack, hammerHandlePivot.transform.localPosition.y + chargeUp, hammerHandlePivot.transform.localPosition.z);
        //Hammer Charge
        startFade = StartCoroutine(FadeWeapon(true));
        while (hammerHandlePivot.transform.eulerAngles.z <= 60)
        {
            //Hammer Move Charge
            hammerHandlePivot.transform.localPosition = Vector3.MoveTowards(hammerHandlePivot.transform.localPosition,
            hammerChargePos, hammerChargeSpeed * Time.deltaTime);
            //Hammer Rotate Charge
            float ChargeAngleZ = Mathf.MoveTowardsAngle(hammerHandlePivot.transform.eulerAngles.z, 60, hammerChargeRotationSpeed * Time.deltaTime);
            hammerHandlePivot.transform.eulerAngles = new Vector3(hammerHandlePivot.transform.eulerAngles.x, hammerHandlePivot.transform.eulerAngles.y
            , ChargeAngleZ);
            yield return true;
        }
        yield return new WaitForSeconds(chargeDuration);
        Vector3 newTarget = new Vector3(targetHitPosition.transform.position.x,
        targetHitPosition.transform.position.y, targetHitPosition.transform.position.z - 12);
        while (hammerHandlePivot.transform.position != newTarget)
        {
            //Hammer Move To target Attack
            hammerHandlePivot.transform.position = Vector3.MoveTowards(hammerHandlePivot.transform.position,
            newTarget, hammerMoveSpeed * Time.deltaTime);
            //Hammer Rotate Down
            float ChargeAngleZ = Mathf.MoveTowardsAngle(hammerHandlePivot.transform.eulerAngles.z, -90, hammerDownRotationSpeed * Time.deltaTime);
            hammerHandlePivot.transform.eulerAngles = new Vector3(hammerHandlePivot.transform.eulerAngles.x, hammerHandlePivot.transform.eulerAngles.y
            , ChargeAngleZ);
            yield return true;
        }
        hammerAttack.CallHammerAttack();
        for (int i = 0; i <= 6; i++)
        {
            if (i % 2 == 0)
            {
                hammerHandlePivot.transform.eulerAngles -= new Vector3(0, 0.3f, 0);
            }
            else
            {
                hammerHandlePivot.transform.eulerAngles += new Vector3(0, 0.7f, 0);
            }
            yield return new WaitForSeconds(0.05f);
        }
        if (startFade != null)
        {
            StopCoroutine(startFade);
        }
        StartCoroutine(FadeWeapon(false));
        yield return new WaitForSeconds(attackUpDuration);
        while (hammerHandlePivot.transform.position != startHammerPoint)
        {
            //Hammer Move To Start point
            hammerHandlePivot.transform.position = Vector3.MoveTowards(hammerHandlePivot.transform.position,
            startHammerPoint, hammerMoveSpeed * Time.deltaTime);
            //Hammer Rotate Up
            float ChargeAngleZ = Mathf.MoveTowardsAngle(hammerHandlePivot.transform.eulerAngles.z, 0, hammerUpRotationSpeed * Time.deltaTime);
            hammerHandlePivot.transform.eulerAngles = new Vector3(hammerHandlePivot.transform.eulerAngles.x, hammerHandlePivot.transform.eulerAngles.y
            , ChargeAngleZ);
            yield return true;
        }

        hammerHandlePivot.transform.eulerAngles = new Vector3(0, -90, 0);
        canMove = true;
        canAttack = true;
        onAttack = false;
        // Debug.Log("End");
    }

    IEnumerator FadeWeapon(bool choose)
    {
        Debug.Log("A");
        float _fadeStart = 1;
        float _fadeTarget = 0.1f;
        float _speedFade = speedFadeIn;
        if (!choose)
        {
            _fadeStart = 0;
            _fadeTarget = 1;
            _speedFade = speedFadeOut;
        }
        while (_fadeStart != _fadeTarget)
        {
            _fadeStart = Mathf.MoveTowards(_fadeStart, _fadeTarget, Time.deltaTime * _speedFade);
            _renderer.material.SetFloat("_Fade", _fadeStart);

            yield return true;
        }
        startFade = null;
    }
}
