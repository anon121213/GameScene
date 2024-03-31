using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Charecter : MonoBehaviour, IControleble
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _runSpeed = 13f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _JumpHeight = 3f;
    [SerializeField] private Transform _groundCheckerPivot;
    [SerializeField] private float _checGroundRadius = 0.4f;
    [SerializeField] private LayerMask _playerMask;

    private Rigidbody _rb;
    private float _velocity;
    private Vector3 _moveDirection;
    private bool _isGrounded;
    private bool _isRuning = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _isGrounded = isOnTheGround();
    
        if(_isGrounded && _velocity < 0)
        {
            _velocity = -2f;
        }

        if (!_isRuning)
        {
            MoveInternal();
        }else
        {
            RunInternal();
        }

        DoGravity();
    }

    private bool isOnTheGround()
    {
        bool result = Physics.CheckSphere(_groundCheckerPivot.position, _checGroundRadius, ~_playerMask);

        return result;
    }

    public void Move(Vector3 direction)
    {
        _moveDirection = direction;
    }

    public void Run(bool _run)
    {
        _isRuning = _run;
    }

    public void Jump()
    {
        if (isOnTheGround())
        {
            _velocity = Mathf.Sqrt(_JumpHeight * -2 * _gravity);
        }       
    }

    private void MoveInternal()
    {
        _rb.MovePosition(_rb.position + _moveDirection * _speed * Time.fixedDeltaTime);
    }

    private void RunInternal()
    {
        _rb.MovePosition(_rb.position + _moveDirection * _runSpeed * Time.fixedDeltaTime);
    }

    private void DoGravity()
    {
        _velocity += _gravity * Time.fixedDeltaTime;

        _rb.MovePosition(_rb.position + Vector3.up * _velocity * Time.fixedDeltaTime);
    }
}