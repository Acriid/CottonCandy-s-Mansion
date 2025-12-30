using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SlopeDetection))]
[RequireComponent(typeof(PickUpMechanic))]
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    #region Other Scripts
    [SerializeField] private SlopeDetection SlopeDetection;
    [SerializeField] private PickUpMechanic PickUpMechanic;
    [SerializeField] private InventoryMechanic InventoryMechanic;
    #endregion

    [SerializeField] private GameObject _itemHolder;
    #region Input
    [Header("Input")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Rigidbody _rigidBody;
    private Vector2 _moveInput;
    #endregion

    #region Movement
    [Header("Movement")]
    [SerializeField] private float _playerSpeed;
    [SerializeField] private float _groundDrag;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayerMask;

    [SerializeField] private float _maxSlopeAngle;
    

    #region Jump
    [Header("Jump")]
    [SerializeField] private float _jumpForce;
    private float _coyoteTimer = 0f;
    private float _jumpBufferTimer = 0f;
    [SerializeField] private float _coyoteTime = 0.1f;
    [SerializeField] private float _jumpBufferTime = 0.15f;
    private bool _wasGroundedLastFrame = false;
    #endregion
    #endregion

    #region Camera
    [Header("Camera")]
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _orientation;
    [SerializeField] private float _lookSensitivity;
    [SerializeField] private float _maxLookRange;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _rotationSpeed;
    private float _xRotation = 0f;
    private float _yRotation = 0f;
    
    #endregion

    #region Gravity
    [Header("Gravity")]
    [SerializeField] private float _gravityModifier;
    [SerializeField] private float RotationSpeed;
    private Coroutine _rotateCoroutine;
    private Vector3 _gravityDirection;
    #endregion

    #region StateMachine
    public PlayerStateMachine StateMachine {get; set;}
    public PlayerWalkingState WalkingState {get; set;}
    #endregion

    #region Collision
    private GameObject _lastObjectHit;
    #endregion

    #region Constants
    const float PLAYERSPEEDOFFSET = 10f;
    const float PLAYERGRAVITYOFFSET = 5f;
    #endregion


  
    #region StartFunctions
    void Awake()
    {
        if(_inputReader == null){Debug.LogError("InputReader does not exists"); return;}
        if(_rigidBody == null){_rigidBody = GetComponent<Rigidbody>();}
        if(SlopeDetection == null){SlopeDetection = GetComponent<SlopeDetection>();}
        if(PickUpMechanic == null){PickUpMechanic = GetComponent<PickUpMechanic>();}

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
    #endregion

    #region Disabled functions
    void OnDestroy()
    {
        OnDisable();
    }
    void OnDisable()
    {
        UnSubscribeFromEvents();
    }
    #endregion

    #region UpdateFunctions
    void Update()
    {
        StateMachine.CurrentState.FrameUpdate();
        _cameraTarget.transform.position = transform.position;
        //RotateCamera();
    }
    void FixedUpdate()
    {
        bool isGrounded = CheckIfGrounded();
        if (isGrounded)
        {
            _coyoteTimer = _coyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.fixedDeltaTime;
        }

        _wasGroundedLastFrame = isGrounded;
        
        _jumpBufferTimer -= Time.fixedDeltaTime;

        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Movement Functions
    public void MovePlayer(Vector2 direction)
    {

        // Get camera directions
        Vector3 cameraForward = _mainCamera.transform.forward;
        Vector3 cameraRight = _mainCamera.transform.right;
        
        // Project onto player's movement plane (perpendicular to gravity)
        Vector3 playerUp = -_gravityDirection.normalized;
        Vector3 projectedForward = Vector3.ProjectOnPlane(cameraForward, playerUp);
        Vector3 projectedRight = Vector3.ProjectOnPlane(cameraRight, playerUp);
        
        if (projectedForward.sqrMagnitude > 0.01f) projectedForward.Normalize();
        if (projectedRight.sqrMagnitude > 0.01f) projectedRight.Normalize();
        
        Vector3 moveDirection = projectedForward * direction.y + projectedRight * direction.x;


        if(SlopeDetection.IsOnWalkableSlope())
        {
            moveDirection = SlopeDetection.GetSlopeMoveDirection(moveDirection);
        }
        _rigidBody.AddForce(_playerSpeed * PLAYERSPEEDOFFSET * moveDirection.normalized, ForceMode.Force);

        //ThirdPerson Rotation
        RotatePlayer(playerUp);


        if (!CheckIfGrounded() && !SlopeDetection.IsOnWalkableSlope())
        {
            _rigidBody.linearVelocity += _gravityModifier * PLAYERGRAVITYOFFSET * Time.fixedDeltaTime * _gravityDirection.normalized;
        }

        _rigidBody.linearDamping = CheckIfGrounded() ? _groundDrag : 0f;
        SpeedControl();

        
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

    #endregion

    #region First Person Camera
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

    #region Third Person Rotation
    private void RotatePlayer(Vector3 playerUp)
    {
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(_rigidBody.linearVelocity, playerUp);
        if (horizontalVelocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity, playerUp);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
        }
    }
    #endregion

    #region Gravity Functions
    public void ChangeGravityDirection(Vector3 newDirection)
    {
        DisableMovement();
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

            _cameraTarget.position = transform.position;
            _cameraTarget.transform.up = transform.up;

            yield return null;
        }

        // Snap exactly and update gravity direction
        transform.rotation = targetRotation;
        _cameraTarget.position = transform.position;
        _cameraTarget.transform.up = transform.up;
        _gravityDirection = -transform.up;
        _rotateCoroutine = null;
        EnableMovement();

    }
    #endregion
    #region  Interaction
    private void OnInteract()
    {
        Item pickUpItem = PickUpMechanic.GetTargetItem();

        if(pickUpItem != null)
        { 
            Debug.Log(pickUpItem.gameObject.name);

            pickUpItem.UseGravity(false);

            PickUpMechanic.PickUpItem(_itemHolder.transform,false,pickUpItem);

            pickUpItem.ChangeExcludeLayerMasks(-1);

            InventoryMechanic.AddToInventory(pickUpItem.gameObject);
        }
    }

    private void OnDrop()
    {
        Item itemToDrop = InventoryMechanic.GetCurrentHeldItem();
        if(itemToDrop != null)
        {
            PickUpMechanic.DropItem(null,true,itemToDrop);

            itemToDrop.ChangeExcludeLayerMasks(0);

            InventoryMechanic.RemoveFromInventory(itemToDrop);
        }
    }
    #endregion
    #region CollisionChecks
    void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("CollisionAble")) {return;}
        if(_lastObjectHit == collision.gameObject) {return;}
        _lastObjectHit = collision.gameObject;
        if(_lastObjectHit.TryGetComponent(out GravityChanger gravityChanger))
        {
            ChangeGravityDirection(gravityChanger.GetGravityDirection());
            InventoryMechanic.ChangeItemGravityDirections(-gravityChanger.GetGravityDirection());
        }
        
    }

    private bool CheckIfGrounded()
    {
        Vector3 groundPoint = _groundCheckPoint.transform.position;
        bool isGrounded = Physics.CheckSphere(groundPoint, _groundCheckRadius, _groundLayerMask);
        return isGrounded;
    }
    #endregion

    #region Jump Functions
    private void OnJump()
    {
        _jumpBufferTimer = _jumpBufferTime;
    }
    public bool CanJump()
    {
        return _jumpBufferTimer > 0f && _coyoteTimer > 0f;
    }
    public void ExecuteJump()
    {
        Vector3 gravityComp = Vector3.Project(_rigidBody.linearVelocity, _gravityDirection);
        _rigidBody.linearVelocity -= gravityComp;
        _rigidBody.AddForce(transform.up * _jumpForce,ForceMode.Impulse);

        _jumpBufferTimer = 0f;
        _coyoteTimer = 0f;
    }
    #endregion

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

    #region JumpInput
    public void EnableJump()
    {
        _inputReader.EnableJumpAciton();
    }
    public void DisableJump()
    {
        _inputReader.DisabelJumpAction();
    }
    #endregion
    #region  Interact
    public void EnableInteract()
    {
        _inputReader.EnableInteractAction();
        EnableDrop();
    }
    public void DisableInteract()
    {
        _inputReader.DiasbleInteractAction();
        DisableDrop();
    }
    #endregion
    #region  Drop
    public void EnableDrop()
    {
        _inputReader.EnableDropAction();
    }
    public void DisableDrop()
    {
        _inputReader.DisableDropAction();
    }
    #endregion
    #region Subcriptions
    private void SubscirbeToEvents()
    {
        _inputReader.OnMove += UpdateMoveInput;
        _inputReader.OnLook += CameraMovement;
        _inputReader.OnJump += OnJump;
        _inputReader.OnInteract += OnInteract;
        _inputReader.OnDrop += OnDrop;
    }
    private void UnSubscribeFromEvents()
    {
        _inputReader.OnMove -= UpdateMoveInput;
        _inputReader.OnLook -= CameraMovement;
        _inputReader.OnJump -= OnJump;
        _inputReader.OnInteract -= OnInteract;
        _inputReader.OnDrop -= OnDrop;
    }
    #endregion
}
