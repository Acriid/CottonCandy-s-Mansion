using UnityEngine;

/// <summary>
/// A script to allow the camera to move in first person. Changes the orientation of the player
/// </summary>
public class FirstPersonPlayerCamera : MonoBehaviour
{
    #region Variables
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private float _lookSensitivity = 20f;


    [SerializeField] private Transform _orientation;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _firstPersonCameraTransform;
    private float _yRotation = 0f;
    private float _xRotation = 0f;
    private float _maxLookRange = 90f;
    #endregion
    void OnEnable()
    {
        InitializeInputs();
        LockCursor();
    }
    void OnDisable()
    {
        DisableInputs();
    }

    void Update()
    {
        RotateCamera();
    }
    private void InitializeInputs()
    {
        InitializeMouseInputs();
    }
    private void DisableInputs()
    {
        DisableMouseInputs();
    }
    private void InitializeMouseInputs()
    {
        if(_inputReader == null) return;
        _inputReader.EnableLookAction();
        _inputReader.OnLook += CameraMovement;
    }
    private void DisableMouseInputs()
    {
        if(_inputReader == null) return;
        _inputReader.DisableLookAction();
        _inputReader.OnLook -= CameraMovement;       
    }
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    /// <summary>
    /// Used to move the camera to look in the direction the player moves their mouse
    /// </summary>
    /// <param name="direction">The direction the player moves their mouse</param>
    private void CameraMovement(Vector2 direction)
    {
        float lookX = direction.x * _lookSensitivity * Time.deltaTime;
        float lookY = direction.y * _lookSensitivity * Time.deltaTime;

        _yRotation += lookX;
        _xRotation -= lookY;

        _xRotation = Mathf.Clamp(_xRotation,-_maxLookRange,_maxLookRange);
    }
    /// <summary>
    /// Rotates the player in the direction of the camera
    /// </summary>
    private void RotateCamera()
    {
        if (Mathf.Abs(_yRotation) > Mathf.Epsilon)
        {
            // rotate the player around its up axis by the delta yaw
            _playerTransform.Rotate(_playerTransform.up, _yRotation, Space.World);
        }

        // Make movement orientation match player's yaw (so movement aligns with facing)
        _orientation.rotation = Quaternion.Euler(_playerTransform.eulerAngles.x, _playerTransform.eulerAngles.y, _playerTransform.eulerAngles.z);

        // Apply camera pitch as local rotation relative to the player
        _firstPersonCameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f,0f);


        // reset per-frame yaw delta so nothing accumulates if CameraMovement isn't called
        _yRotation = 0f;
    }
}
