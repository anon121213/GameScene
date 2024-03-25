using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CherecterInputController : MonoBehaviour
{
    private IControleble _controleble;
    private PlayerInput _gameinput;

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _palyer;
    [SerializeField] protected float sensitivity = 1.5f;
    [SerializeField] private float smoth = 10f;

    private float yRotation;
    private float xRotation;
 
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _gameinput = new PlayerInput();
        _gameinput.Enable();

        _controleble = GetComponent<IControleble>();

        if(_controleble == null)
        {
            throw new Exception($"There is no IControleble component on the object: {gameObject.name}");
        }
    }

    private void OnEnable()
    {
        _gameinput.GamePlay.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        _gameinput.GamePlay.Jump.performed -= OnJumpPerformed;
    }

    private void OnJumpPerformed(InputAction.CallbackContext obj)
    {
        _controleble.Jump();
    }

    private void Update()
    {
        xRotation += Input.GetAxis("Mouse Y") * sensitivity;
        yRotation -= Input.GetAxis("Mouse X") * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        ReadMovement();
        RotateCherecter();
    }

    private void RotateCherecter()
    {
        _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, Quaternion.Euler(-xRotation, -yRotation, 0), Time.deltaTime * smoth);
        _palyer.rotation = Quaternion.Lerp(_palyer.rotation, Quaternion.Euler(0, -yRotation, 0), Time.deltaTime * smoth);
    }

    private void ReadMovement()
    {
        var inputDirection = _gameinput.GamePlay.Movement.ReadValue<Vector3>();

        if (_camera != null)
        {
            var forward = _camera.transform.forward;
            var right = _camera.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            var desiredMoveDirection = forward * inputDirection.z + right * inputDirection.x;
            desiredMoveDirection.y = inputDirection.y;

            _controleble.Move(desiredMoveDirection);
        }

        else
        {
            throw new Exception($"There is no camera in object {gameObject.name}");
        }
    }

}
