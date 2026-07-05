using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private PlayerSettingsSO _playerSettings;
    [SerializeField] private PlayerSlopeSettingsSO _playerSlopeSettings;
    [SerializeField] private Camera _mainCamera;


    private ISlopeDetection _slopeDetection;
    private IPhysicsChecks _physicsChecks;
    private Vector2 _moveInput;

    const float PLAYERSPEEDOFFSET = 10f;
    const float PLAYERGRAVITYOFFSET = 2f;
    void Awake()
    {
        Physics.simulationMode = SimulationMode.FixedUpdate;
        InitializePlayer();
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
        _rigidBody.AddForce(_playerSettings.PlayerSpeed * PLAYERSPEEDOFFSET * moveDirection.normalized, ForceMode.Force);

        if(!_physicsChecks.CheckIfGrounded(_groundCheckPoint,_playerSettings.PlayerGroundLayerMask) 
            && !_slopeDetection.IsOnWalkableSlope())
        {
            _rigidBody.linearVelocity += _playerSettings.PlayerGravityModifier * PLAYERGRAVITYOFFSET * 
            Time.fixedDeltaTime * _playerSettings.PlayerGravityDirection.normalized;
        }


        _rigidBody.linearDamping = _physicsChecks.CheckIfGrounded(_groundCheckPoint,_playerSettings.PlayerGroundLayerMask) ? 
                                    _playerSettings.PlayerDrag : 0f;
        SpeedControl();
    }
    /// <summary>
    /// Controls the speed of the player is always called at the end of MovePlayer
    /// </summary>
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


    void OnCollisionStay(Collision collision)
    {
        _slopeDetection.OnCollisionStayLogic(collision);
    }
    void OnCollisionExit(Collision collision)
    {
        _slopeDetection.OnCollisionExitLogic(collision);
    }

    //Public API
    public void InitializePlayer()
    {
        _slopeDetection = new SlopeDetection(_playerSlopeSettings,this.transform);
        _physicsChecks = new PhysicsChecks();

    }

    public void InitializeSettings()
    {
        _playerSettings.Initialize();
        _playerSlopeSettings.Initialize();        
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
    #region getters and setters
    public void SetInputReader(InputReader inputReader)
    {
        _inputReader = inputReader;
    }
    public InputReader GetInputReader()
    {
        return _inputReader;
    }
    public void SetRigidBody(Rigidbody rigidBody)
    {
        _rigidBody = rigidBody;
    }
    public Rigidbody GetRigidBody()
    {
        return _rigidBody;
    }
    public void SetGroundCheckPoint(Transform newCheckPoint)
    {
        _groundCheckPoint = newCheckPoint;
    }
    public Transform GetGroundCheckPoint()
    {
        return _groundCheckPoint;
    }
    public void SetPlayerSettings(PlayerSettingsSO playerSettingsSO)
    {
        _playerSettings = playerSettingsSO;
    }
    public PlayerSettingsSO GetPlayerSettings()
    {
        return _playerSettings;
    }
    public void SetPlayerSlopeSettings(PlayerSlopeSettingsSO playerSlopeSettingsSO)
    {
        _playerSlopeSettings = playerSlopeSettingsSO;
    }
    public PlayerSlopeSettingsSO GetPlayerSlopeSettings()
    {
        return _playerSlopeSettings;
    }
    public void SetMainCamera(Camera camera)
    {
        _mainCamera = camera;
    }
    public Camera GetMainCamera()
    {
        return _mainCamera;
    }
    public void SetSlopeDetection(ISlopeDetection slopeDetection)
    {
        _slopeDetection = slopeDetection;
    }
    public ISlopeDetection GetSlopeDetection()
    {
        return _slopeDetection;
    }
    public void SetPhysicsChecks(IPhysicsChecks physicsChecks)
    {
        _physicsChecks = physicsChecks;
    }
    public IPhysicsChecks GetPhysicsChecks()
    {
        return _physicsChecks;
    }

    #endregion
}
