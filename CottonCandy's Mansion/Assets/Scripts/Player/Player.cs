using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _playerSpeed;
    [SerializeField] private Transform _orientation;

    private Vector2 _moveInput;
    


    public PlayerStateMachine StateMachine {get; set;}
    public PlayerWalkingState WalkingState {get; set;}

    void Awake()
    {
        if(_inputReader == null){Debug.LogError("InputReader does not exists"); return;}

        StateMachine = new();
        WalkingState = new(this,StateMachine);

        _inputReader.OnMove += UpdateMoveInput;
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
        _inputReader.OnMove += UpdateMoveInput;
    }
    void Update()
    {
        StateMachine.CurrentState.FrameUpdate();
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
}
