using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    void Awake()
    {

    }
    void OnDestroy()
    {
        OnDisable();
    }
    void OnDisable()
    {

    }
    private void InitializeActions()
    {
        _inputReader.EnableLookAction();
        _inputReader.EnableMoveAction(); 
    }
}
