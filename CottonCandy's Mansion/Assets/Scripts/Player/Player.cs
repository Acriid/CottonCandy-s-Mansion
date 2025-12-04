using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _playerSpeed;
    [SerializeField] private Transform _orientation;
    [SerializeField] private float _lookSensitivity;

    private Vector2 _moveInput;
    

    private float _xRotation = 0f;
    private float _yRotation = 0f;
    [SerializeField] private float _maxLookRange;
    [SerializeField] private Transform _cameraTransform;


    public PlayerStateMachine StateMachine {get; set;}
    public PlayerWalkingState WalkingState {get; set;}

    void Awake()
    {
        if(_inputReader == null){Debug.LogError("InputReader does not exists"); return;}

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _inputReader.EnableLookAction();


        StateMachine = new();
        WalkingState = new(this,StateMachine);

        _inputReader.OnMove += UpdateMoveInput;
        _inputReader.OnLook += CameraMovement;
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
        _inputReader.OnMove -= UpdateMoveInput;
        _inputReader.OnLook -= CameraMovement;
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
        _characterController.Move(moveDirection * _playerSpeed);
    }






    private void CameraMovement(Vector2 direction)
    {
        float lookX = direction.x * _lookSensitivity * Time.deltaTime * 0.25f;
        float lookY = direction.y * _lookSensitivity * Time.deltaTime * 0.25f;

        _yRotation += lookX;
        _xRotation -= lookY;

        _xRotation = Mathf.Clamp(_xRotation,-_maxLookRange,_maxLookRange);
    }
    private void RotateCamera()
    {
        _cameraTransform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        _orientation.rotation = Quaternion.Euler(0f, _yRotation, 0f);
        transform.rotation = Quaternion.Euler(0f, _yRotation, 0f);
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
}
