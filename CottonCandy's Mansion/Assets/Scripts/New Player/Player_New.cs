using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player_New : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Transform _orientation;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private PlayerSettingsSO _playerSettings;
    [SerializeField] private PlayerSlopeSettingsSO _playerSlopeSettings;
    [SerializeField] private Camera _mainCamera;


    private SlopeDetection _slopeDetection;
    private Vector2 _moveInput;
    void Awake()
    {
        // if(!TryGetComponent(out _rigidBody))
        // {
        //     Debug.LogWarning("No RigidBody found on the player");
        // }

        // if(_orientation == null)
        // {
        //     Debug.LogWarning("No player orientation");
        // }
        _slopeDetection = new(_playerSlopeSettings,this.transform);
    }
    void FixedUpdate()
    {
        _slopeDetection.FixedUpdateLogic();
        MovePlayer(_moveInput);
    }
    void OnEnable()
    {
        EnableMoveInput();
    }
    void OnDisable()
    {
        DisableMoveInput();
    }
    public void EnableMoveInput()
    {
        _inputReader.OnMove += UpdateMoveInput;
        _inputReader.EnableMoveAction();
    }
    public void DisableMoveInput()
    {
        _inputReader.OnMove -= UpdateMoveInput;
        _inputReader.DisableMoveAction();
    }
    private void UpdateMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
    }

    /// <summary>
    /// Moves the player towards the direction given while also adding gravity into the player's current
    /// gravity direction.
    /// </summary>
    /// <param name="direction">The direction to move the player towards</param>
    public void MovePlayer(Vector2 direction)
    {
        if(_rigidBody == null)
        {
            Debug.LogError("No RigidBody on the player");
            return;
        }

        // Get camera directions
        Vector3 cameraForward = _mainCamera.transform.forward;
        Vector3 cameraRight = _mainCamera.transform.right;

        // Project onto player's movement plane (perpendicular to gravity)
        Vector3 playerUp = -_playerSettings.PlayerGravityDirection.normalized;
        Vector3 projectedForward = Vector3.ProjectOnPlane(cameraForward, playerUp);
        Vector3 projectedRight = Vector3.ProjectOnPlane(cameraRight, playerUp);
        
        if (projectedForward.sqrMagnitude > 0.01f) projectedForward.Normalize();
        if (projectedRight.sqrMagnitude > 0.01f) projectedRight.Normalize();
        
        Vector3 moveDirection = projectedForward * direction.y + projectedRight * direction.x;


        if(_slopeDetection.IsOnWalkableSlope())
        {
            moveDirection = _slopeDetection.GetSlopeMoveDirection(moveDirection);
        }
        _rigidBody.AddForce(_playerSettings.PlayerSpeed * moveDirection.normalized, ForceMode.Force);

        if(!EntityPhysicsChecks.CheckIfGrounded(_groundCheckPoint,_playerSettings.PlayerGroundLayerMask) 
            && !_slopeDetection.IsOnWalkableSlope())
        {
            _rigidBody.linearVelocity += _playerSettings.PlayerGravityModifier * 
            Time.fixedDeltaTime * _playerSettings.PlayerGravityDirection.normalized;
        }


        _rigidBody.linearDamping = EntityPhysicsChecks.CheckIfGrounded(_groundCheckPoint,_playerSettings.PlayerGroundLayerMask) ? 
                                    _playerSettings.PlayerDrag : 0f;
        SpeedControl();
    }
    private void SpeedControl()
    {
        Vector3 gravityComp = Vector3.Project(_rigidBody.linearVelocity, _playerSettings.PlayerGravityDirection);
        if(gravityComp.magnitude > 2f * _playerSettings.PlayerGravityModifier)
        {
            gravityComp = _playerSettings.PlayerGravityModifier * 2f * gravityComp.normalized;
        }

        Vector3 nonGravityComp = _rigidBody.linearVelocity - gravityComp;
        if (nonGravityComp.magnitude > _playerSettings.PlayerSpeed)
        {
            Vector3 clampedNonGravity = nonGravityComp.normalized * _playerSettings.PlayerSpeed;
            _rigidBody.linearVelocity = clampedNonGravity + gravityComp;
        }
    }
    public void SetRigidBody(Rigidbody rigidBody)
    {
        _rigidBody = rigidBody;
    }
    public Rigidbody GetRigidBody()
    {
        return _rigidBody;
    }

    void OnCollisionStay(Collision collision)
    {
        _slopeDetection.OnCollisionStay(collision);
    }
    void OnCollisionExit(Collision collision)
    {
        _slopeDetection.OnCollisionExit(collision);
    }
}
