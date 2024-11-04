using Cinemachine;
using FMOD;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Keyboard Move Setting")]
    [SerializeField] private int _cameraSpeed;

    private CinemachineVirtualCamera _playerVirtualCamera;

    private Vector2 _cameraMoveInput2D;
    private Vector3 _cameraMoveTarget;
    [Header("Mouse Move Setting")]
    public float scrollSpeed = 20f;  // ความเร็วในการเลื่อนกล้อง
    public float edgeSize = 10f;     // ระยะขอบจอที่เริ่มเลื่อน  
    bool onMouseMove = false;
    bool onMoveUpDown;
    bool onMoveLeftRight;
    private void Start()
    {
        _playerVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cameraMoveTarget = _playerVirtualCamera.transform.position;
    }

    // Takes input from player input manager and transforms it to Vector2
    public void OnMove(InputValue value)
    {
        _cameraMoveInput2D = value.Get<Vector2>();
    }

    // Moves camera with the input we created on OnMove function
    void Update()
    {
        // MoveWithKeyboard();
        MoveWithMouse();
    }

    void MoveWithKeyboard()
    {
        Vector3 cameraMovementDirection = new Vector3(_cameraMoveInput2D.x, 0f, _cameraMoveInput2D.y).normalized;
        _cameraMoveTarget += ((Vector3.forward + Vector3.right) * _cameraMoveInput2D.y +
                              transform.right.normalized * _cameraMoveInput2D.x +
                              Vector3.zero) * Time.deltaTime * _cameraSpeed;
        _playerVirtualCamera.transform.position = Vector3.Lerp(_playerVirtualCamera.transform.position, _cameraMoveTarget, Time.deltaTime * _cameraSpeed);
    }

    void MoveWithMouse()
    {
        Vector3 direction = Vector3.zero;
        // ตรวจสอบการเลื่อนในแนวแกน X (ซ้าย-ขวา)
        if (Input.mousePosition.x <= edgeSize)
        {
            direction += Vector3.left; // ไปทางซ้าย
            onMoveLeftRight = true;
            if (Input.mousePosition.y <= Screen.height / 2)
            {
                // UnityEngine.Debug.Log("ล่าง");

            }
            else
            {
                // UnityEngine.Debug.Log("บน");

            }
        }
        else if (Input.mousePosition.x >= Screen.width - edgeSize)
        {
            direction += Vector3.right; // ไปทางขวา
            onMoveLeftRight = true;
            if (Input.mousePosition.y <= Screen.height / 2)
            {
                // UnityEngine.Debug.Log("ล่าง");

            }
            else
            {
                // UnityEngine.Debug.Log("บน");

            }
        }
        else
        {
            onMoveLeftRight = false;
        }
        // ตรวจสอบการเลื่อนในแนวแกน Z (ขึ้น-ลง)
        if (Input.mousePosition.y <= edgeSize)
        {
            direction += Vector3.back; // ลง
            onMoveUpDown = true;
        }
        else if (Input.mousePosition.y >= Screen.height - edgeSize)
        {
            direction += Vector3.forward * 1.5f; // ขึ้น
            onMoveUpDown = true;
        }
        else
        {
            onMoveUpDown = false;
        }
        if (onMoveLeftRight || onMoveUpDown)
        {
            onMouseMove = true;
        }
        else
        {
            onMouseMove = false;
        }
        Vector3 subdiractionX = Vector3.zero;
        Vector3 subdiractionZ = Vector3.zero;
        if (onMouseMove)
        {
            if (Input.mousePosition.x < Screen.width / 2)
            {
                // UnityEngine.Debug.Log("ซ้าย");
                subdiractionX = Vector3.left;
            }
            else
            {
                // UnityEngine.Debug.Log("ขวา");
                subdiractionX = Vector3.right;
            }
        }
        UnityEngine.Debug.Log(subdiractionX);
        // เคลื่อนที่กล้องตามทิศทางที่คำนวณได้
        transform.Translate((direction + subdiractionX + subdiractionZ) * scrollSpeed * Time.deltaTime, Space.World);
    }
}


