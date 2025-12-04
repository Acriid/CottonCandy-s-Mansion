using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "ScriptableObjects/InputReader")]
public class InputReader : ScriptableObject
{
    private InputActions _inputActions;   
    private InputAction _moveAction;
    private InputAction _lookAction;

    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;

    private Action<InputAction.CallbackContext> movePerformed;
    private Action<InputAction.CallbackContext> moveCancled; 

    private Action<InputAction.CallbackContext> lookPerformed;
    private Action<InputAction.CallbackContext> lookCancled;

    void OnEnable()
    {
        _inputActions = new();

        _moveAction = _inputActions.Player.Move;
        _lookAction = _inputActions.Player.Look;

        movePerformed = ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        moveCancled = ctx => OnMove?.Invoke(Vector2.zero);

        lookPerformed = ctx => OnLook?.Invoke(ctx.ReadValue<Vector2>());
        lookCancled = ctx => OnLook?.Invoke(Vector2.zero);

        SubscribeActions();
    }
    void OnDisable()
    {
        UnSubscrubeActions();
    }

    void OnDestroy()
    {
        OnDisable();
    }

    
    
    public void EnableMoveAction()
    {
        _moveAction.Enable();
    }
    public void DisableMoveAction()
    {
        _moveAction.Disable();
    }
    public void EnableLookAction()
    {
        _lookAction.Enable();
    }
    public void DisableLookAction()
    {
        _lookAction.Disable();
    }



    public void SubscribeActions()
    {
        _moveAction.performed += movePerformed;
        _moveAction.canceled += moveCancled;

        _lookAction.performed += lookPerformed;
        _lookAction.canceled += lookCancled;
    }

    public void UnSubscrubeActions()
    {
        _moveAction.performed -= movePerformed;
        _moveAction.canceled -= moveCancled;

        _lookAction.performed -= lookPerformed;
        _lookAction.canceled -= lookCancled;
    }
}
