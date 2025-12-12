using System;
using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //REMOVELATER
    public TMP_Text textthing;
    [Header("Input")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Rigidbody _rigidBody;
    [Header("Movement")]
    [SerializeField] private float _playerSpeed;
    [SerializeField] private float _groundDrag;
    [SerializeField] private float _jumpForce;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayerMask;
    [Header("Camera")]
    [SerializeField] private Transform _orientation;
    [SerializeField] private float _lookSensitivity;

    private Vector2 _moveInput;
    public Vector3 _gravityDirection;
    public float _gravityModifier;
    private Vector3 gravityVelocity = Vector3.zero;
    public float RotationSpeed;
    private Coroutine _rotateCoroutine;

    private float _xRotation = 0f;
    private float _yRotation = 0f;
    [SerializeField] private float _maxLookRange;
    [SerializeField] private Transform _cameraTransform;


    public PlayerStateMachine StateMachine {get; set;}
    public PlayerWalkingState WalkingState {get; set;}

    private GameObject _lastObjectHit;
    const float PLAYERSPEEDOFFSET = 10f;
    const float PLAYERGRAVITYOFFSET = 5f;

    void Awake()
    {
        if(_inputReader == null){Debug.LogError("InputReader does not exists"); return;}

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _inputReader.EnableLookAction();


        StateMachine = new();
        WalkingState = new(this,StateMachine);

        SubscirbeToEvents();

        _rigidBody.freezeRotation = true;
    }

    void Start()
    {
        StateMachine.Initialize(WalkingState);
    }
    void OnDestroy()
    {
        OnDisable();
    }
    void OnDisable()
    {
        UnSubscribeFromEvents();
    }
    void Update()
    {
        StateMachine.CurrentState.FrameUpdate();
        RotateCamera();
    }
    void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    public void MovePlayer(Vector2 direction)
    {
        
        Vector3 moveDirection = _orientation.forward * direction.y + _orientation.right * direction.x;
        _rigidBody.AddForce(_playerSpeed * PLAYERSPEEDOFFSET * moveDirection.normalized, ForceMode.Force);

        if (!CheckIfGrounded())
        {
            _rigidBody.linearVelocity += _gravityModifier * PLAYERGRAVITYOFFSET * Time.fixedDeltaTime * _gravityDirection.normalized;
        }
        else
        {
            Vector3 gravityComp = Vector3.Project(_rigidBody.linearVelocity, _gravityDirection);
            if (Vector3.Dot(gravityComp, _gravityDirection) < 0f)
            {
                _rigidBody.linearVelocity -= gravityComp;
            }
        }

        _rigidBody.linearDamping = CheckIfGrounded() ? _groundDrag : 0f;
        SpeedControl();
        textthing.text = _rigidBody.linearVelocity.magnitude.ToString();
    }
    private void SpeedControl()
    {
        Vector3 gravityComp = Vector3.Project(_rigidBody.linearVelocity, _gravityDirection);
        if(gravityComp.magnitude > 2f * _gravityModifier)
        {
            gravityComp = _gravityModifier * 2f * gravityComp.normalized;
        }

        Vector3 nonGravityComp = _rigidBody.linearVelocity - gravityComp;
        if (nonGravityComp.magnitude > _playerSpeed)
        {
            Vector3 clampedNonGravity = nonGravityComp.normalized * _playerSpeed;
            _rigidBody.linearVelocity = clampedNonGravity + gravityComp;
        }
    }



    #region  First Person Camera
    private void CameraMovement(Vector2 direction)
    {
        float lookX = direction.x * _lookSensitivity * Time.deltaTime * 0.25f;
        float lookY = direction.y * _lookSensitivity * Time.deltaTime * 0.25f;

        _yRotation = lookX;
        _xRotation -= lookY;

        _xRotation = Mathf.Clamp(_xRotation,-_maxLookRange,_maxLookRange);
    }
    private void RotateCamera()
    {
        // Apply body yaw only if there was mouse movement this frame
        if (Mathf.Abs(_yRotation) > Mathf.Epsilon)
        {
            // rotate the player around its up axis by the delta yaw
            transform.Rotate(transform.up, _yRotation, Space.World);
        }

        // Make movement orientation match player's yaw (so movement aligns with facing)
        _orientation.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

        // Apply camera pitch as local rotation relative to the player
        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        // reset per-frame yaw delta so nothing accumulates if CameraMovement isn't called
        _yRotation = 0f;
    }
    #endregion


    public void ChangeGravityDirection(Vector3 newDirection)
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, newDirection) * transform.rotation;

        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);

        _rotateCoroutine = StartCoroutine(RotateToGravity(targetRotation));
    }

    private IEnumerator RotateToGravity(Quaternion targetRotation)
    {
        // Rotate until we're effectively at the target.
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            float step = RotationSpeed * Time.deltaTime; // degrees per second -> degrees this frame
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
            yield return null;
        }

        // Snap exactly and update gravity direction
        transform.rotation = targetRotation;
        _gravityDirection = -transform.up;
        _rotateCoroutine = null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("CollisionAble")) {return;}
        if(_lastObjectHit == collision.gameObject) {return;}
        _lastObjectHit = collision.gameObject;
        if(_lastObjectHit.TryGetComponent(out GravityChanger gravityChanger))
        {
            ChangeGravityDirection(gravityChanger.GetGravityDirection());
        }
        
    }

    private bool CheckIfGrounded()
    {
        Vector3 groundPoint = _groundCheckPoint.transform.position;
        bool isGrounded = Physics.CheckSphere(groundPoint, _groundCheckRadius, _groundLayerMask);
        return isGrounded;
    }



    private void OnJump()
    {
        Vector3 gravityComp = Vector3.Project(_rigidBody.linearVelocity, _gravityDirection);
        _rigidBody.linearVelocity -= gravityComp;

        _rigidBody.AddForce(transform.up * _jumpForce,ForceMode.Impulse);
    }



    #region  MoveInput
    public Vector2 GetMoveInput()
    {
        return _moveInput;
    }
    private void UpdateMoveInput(Vector2 newValue)
    {
        _moveInput = newValue;
    }
    public void DisableMovement()
    {
        _inputReader.DisableMoveAction();
    }
    public void EnableMovement()
    {
        _inputReader.EnableMoveAction();
    }
    #endregion

    public void EnableJump()
    {
        _inputReader.EnableJumpAciton();
    }
    public void DisableJump()
    {
        _inputReader.DisabelJumpAction();
    }

    private void SubscirbeToEvents()
    {
        _inputReader.OnMove += UpdateMoveInput;
        _inputReader.OnLook += CameraMovement;
        _inputReader.OnJump += OnJump;
    }
    private void UnSubscribeFromEvents()
    {
        _inputReader.OnMove -= UpdateMoveInput;
        _inputReader.OnLook -= CameraMovement;
        _inputReader.OnJump -= OnJump;
    }
}
