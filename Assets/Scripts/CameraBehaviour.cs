using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Vector3 _endPosition;
    [SerializeField] private float _cameraEntranceSpeed;
    [SerializeField] private float _cameraManualMovementSpeed;
    [SerializeField] private float _mouseSensitivity;
    private Camera _camera;
    private bool _isLerping = true;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (_isLerping)
        {
            _camera.transform.position =
                Vector3.Lerp(_camera.transform.position, _endPosition, _cameraEntranceSpeed * Time.deltaTime);

            if (Vector3.Distance(_camera.transform.position, _endPosition) < 0.1f)
            {
                _camera.transform.position = _endPosition;
                _isLerping = false;
            }
        }
        else
        {
            HandleMovement();
            HandleMouseRotation();
        }
    }

    private void HandleMouseRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        _camera.transform.Rotate(Vector3.up, mouseX, Space.World);
        _camera.transform.Rotate(Vector3.right, -mouseY);
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal + _camera.transform.forward * vertical;

        moveDirection.y = 0f;

        _camera.transform.position += moveDirection * _cameraManualMovementSpeed * Time.deltaTime;
    }
}
