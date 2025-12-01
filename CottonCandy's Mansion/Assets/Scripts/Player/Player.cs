using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    void Awake()
    {
        InputManager.Instance.OnInitialized += InitializeActions;
    }
    void OnDestroy()
    {
        OnDisable();
    }
    void OnDisable()
    {
        InputManager.Instance.OnInitialized -= InitializeActions;
    }
    private void InitializeActions()
    {
        
    }
}
