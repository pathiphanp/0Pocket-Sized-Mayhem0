using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("HemmerControl")]
    [SerializeField] GameObject hammerPivot;
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
    [SerializeField] float hammerMoveSpeed;
    [SerializeField] float hammerRotationSpeed;

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
        Cursor.lockState = CursorLockMode.Confined;
        SetUpHammer();
        lastMousePosition = Input.mousePosition;
    }
    void SetUpHammer()
    {
        targetHitObject.transform.localScale = new Vector3(targetHammerRadius, targetHitObject.transform.localScale.y, targetHammerRadius);
        Vector3 _hammer = hammerPivot.transform.position;
        hammerPivot.transform.position = new Vector3(_hammer.x, hightHammerFromGround, _hammer.z);

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
        onAttack = true;
        StartCoroutine(HammerAction());
    }

    IEnumerator HammerAction()
    {
        Vector3 startHammerPoint = hammerPivot.transform.position;
        while (hammerPivot.transform.position != targetHitPosition.transform.position)
        {
            //Hammer Move To target Attack
            hammerPivot.transform.position = Vector3.MoveTowards(hammerPivot.transform.position,
            targetHitPosition.transform.position, hammerMoveSpeed * Time.deltaTime);
            //Hammer Rotate Down
            float newAngleX = Mathf.MoveTowardsAngle(hammerPivot.transform.eulerAngles.x, 90, hammerRotationSpeed * Time.deltaTime);
            hammerPivot.transform.eulerAngles = new Vector3(newAngleX, hammerPivot.transform.eulerAngles.y
            , hammerPivot.transform.eulerAngles.y); ;
            yield return true;
        }
        hammerAttack.CallHammerAttack();
        while (hammerPivot.transform.position != startHammerPoint)
        {
            //Hammer Move To Start point
            hammerPivot.transform.position = Vector3.MoveTowards(hammerPivot.transform.position,
            startHammerPoint, hammerMoveSpeed * Time.deltaTime);
            //Hammer Rotate Up
            float newAngleX = Mathf.MoveTowardsAngle(hammerPivot.transform.eulerAngles.x, 0, hammerRotationSpeed * Time.deltaTime);
            hammerPivot.transform.eulerAngles = new Vector3(newAngleX, hammerPivot.transform.eulerAngles.y
            , hammerPivot.transform.eulerAngles.z);
            yield return true;
        }
        canMove = true;
        canAttack = true;
        onAttack = false;
    }
}
