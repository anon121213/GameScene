using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharecterInputController : MonoBehaviour
{
    private IControleble _controleble;
    private PlayerInput _gameinput;

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _palyer;
    [SerializeField] protected float sensitivity = 1.5f;
    [SerializeField] private float _cameraSmoth = 10f;
    [SerializeField] private float _fovSmoth = 3f;
    [SerializeField] private float _moveSmoth = 15f;

    private float yRotation;
    private float xRotation;
    private bool _run = false;
    private Vector3 _normal;
    private Vector3 _normalDirection;
    private Vector3 _currentDirection;
    private Vector3 _currentMoveDirection = Vector3.zero;
    private Collider[] _normalArr;

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

    private void OnJumpPerformed(InputAction.CallbackContext obj)
    {
        _controleble.Jump();
    }

    private void Runperformed(InputAction.CallbackContext obj)
    {
        _run = true;
    }

    private void NotRunperformed(InputAction.CallbackContext obj)
    {
        _run = false;
    }

    private void OnEnable()
    {
        _gameinput.GamePlay.Jump.performed += OnJumpPerformed;
        _gameinput.GamePlay.Run.performed += Runperformed;
        _gameinput.GamePlay.Run.canceled += NotRunperformed;
    }

    private void OnDisable()
    {
        _gameinput.GamePlay.Jump.performed -= OnJumpPerformed;
        _gameinput.GamePlay.Run.performed -= Runperformed;
        _gameinput.GamePlay.Run.canceled -= NotRunperformed;
    }

    private Vector3 DirectionWithNormal(Vector3 forward)
    {
        Vector3 direction = forward - Vector3.Dot(forward, _normal) * _normal;
        return direction.normalized + _currentDirection;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + _normal * 3);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + DirectionWithNormal(_currentDirection));
    }
    
    private Vector3 Collision()
    {
        RaycastHit hit;
        
        if (Physics.SphereCast(transform.position, 0.4f, -transform.up , out hit, 10))
        {
            _normal = hit.normal;
        }

        return _normal;
    }

    private void Update()
    {
        xRotation += Input.GetAxis("Mouse Y") * sensitivity;
        yRotation -= Input.GetAxis("Mouse X") * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        _controleble.Run(_run);
        _controleble.Move(DirectionWithNormal(_currentDirection));
        ReadMovement();
        RotateCherecter();
        Collision();
    }

    private void RotateCherecter()
    {
        _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, Quaternion.Euler(-xRotation, -yRotation, 0), Time.deltaTime * _cameraSmoth);
        _palyer.rotation = Quaternion.Lerp(_palyer.rotation, Quaternion.Euler(0, -yRotation, 0), Time.deltaTime * _cameraSmoth);
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

            _currentMoveDirection = Vector3.Lerp(_currentMoveDirection, desiredMoveDirection, _moveSmoth * Time.deltaTime);
            

            if (_run && desiredMoveDirection != new Vector3(0f, 0f, 0f))
            {
                _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, 70, _fovSmoth * Time.deltaTime);
            }
            else
            {
                _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, 60, _fovSmoth * Time.deltaTime);
            }

            _currentDirection = _currentMoveDirection;
        }

        else
        {
            throw new Exception($"There is no camera in object {gameObject.name}");
        }
    }


}
