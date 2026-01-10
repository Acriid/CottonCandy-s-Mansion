using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Inputs/InputReader")]
public class InputReader : ScriptableObject
{
    private InputActions _inputActions;   
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _interactAction;
    private InputAction _dropAction;
    private InputAction _navigateAction;
    private InputAction _submitAction;

    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;
    public event Action OnJump;
    public event Action OnInteract;
    public event Action OnDrop;
    public event Action OnNavigate;
    public event Action OnSubmit;

    private Action<InputAction.CallbackContext> movePerformed;
    private Action<InputAction.CallbackContext> moveCancled; 

    private Action<InputAction.CallbackContext> lookPerformed;
    private Action<InputAction.CallbackContext> lookCancled;

    private Action<InputAction.CallbackContext> jumpPerformed;


    private Action<InputAction.CallbackContext> interactPerformed;
    private Action<InputAction.CallbackContext> dropPerformed;

    private Action<InputAction.CallbackContext> navigatePerformed;
    private Action<InputAction.CallbackContext> submitPerformed;


    void OnEnable()
    {
        _inputActions = new();

        //Player
        _moveAction = _inputActions.Player.Move;
        _lookAction = _inputActions.Player.Look;
        _jumpAction = _inputActions.Player.Jump;
        _interactAction = _inputActions.Player.Interact;
        _dropAction = _inputActions.Player.Drop;

        movePerformed = ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        moveCancled = ctx => OnMove?.Invoke(Vector2.zero);

        lookPerformed = ctx => OnLook?.Invoke(ctx.ReadValue<Vector2>());
        lookCancled = ctx => OnLook?.Invoke(Vector2.zero);

        jumpPerformed = ctx => OnJump?.Invoke();

        interactPerformed = ctx => OnInteract?.Invoke();

        dropPerformed = ctx => OnDrop?.Invoke();

        //Ui
        _navigateAction = _inputActions.UI.Navigate;
        _submitAction = _inputActions.UI.Submit;

        navigatePerformed = ctx => OnNavigate?.Invoke();
        submitPerformed = ctx => OnSubmit?.Invoke();

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

    public void EnableJumpAciton()
    {
        _jumpAction.Enable();
    }
    public void DisabelJumpAction()
    {
        _jumpAction.Disable();
    }

    public void EnableInteractAction()
    {
        _interactAction.Enable();
    }
    public void DiasbleInteractAction()
    {
        _interactAction.Disable();
    }

    public void EnableDropAction()
    {
        _dropAction.Enable();
    }
    public void DisableDropAction()
    {
        _dropAction.Disable();
    }

    public void EnableNavigateAction()
    {
        _navigateAction.Enable();
    }
    public void DisableNavigateAction()
    {
        _navigateAction.Disable();
    }
    public void EnableSubmitAction()
    {
        _submitAction.Enable();
    }
    public void DisableSubmitAction()
    {
        _submitAction.Disable();
    }
    public void SubscribeActions()
    {
        _moveAction.performed += movePerformed;
        _moveAction.canceled += moveCancled;

        _lookAction.performed += lookPerformed;
        _lookAction.canceled += lookCancled;

        _jumpAction.started += jumpPerformed;

        _interactAction.started += interactPerformed;
        _dropAction.started += dropPerformed;

        _navigateAction.performed += navigatePerformed;
        _submitAction.performed += submitPerformed;
    }

    public void UnSubscrubeActions()
    {
        _moveAction.performed -= movePerformed;
        _moveAction.canceled -= moveCancled;

        _lookAction.performed -= lookPerformed;
        _lookAction.canceled -= lookCancled;

        _jumpAction.started -= jumpPerformed;

        _interactAction.started -= interactPerformed;
        _dropAction.started -= dropPerformed;

        _navigateAction.performed -= navigatePerformed;
        _submitAction.performed -= submitPerformed;
    }
}
