using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private int _cameraSpeed;

    private CinemachineVirtualCamera _playerVirtualCamera;

    private Vector2 _cameraMoveInput2D;
    private Vector3 _cameraMoveTarget;

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
        Vector3 cameraMovementDirection = new Vector3(_cameraMoveInput2D.x, 0f, _cameraMoveInput2D.y).normalized;
        _cameraMoveTarget += ((Vector3.forward + Vector3.right) * _cameraMoveInput2D.y +
                              transform.right.normalized * _cameraMoveInput2D.x +
                              Vector3.zero) * Time.deltaTime * _cameraSpeed;
        _playerVirtualCamera.transform.position = Vector3.Lerp(_playerVirtualCamera.transform.position, _cameraMoveTarget, Time.deltaTime * _cameraSpeed);
    }
}


