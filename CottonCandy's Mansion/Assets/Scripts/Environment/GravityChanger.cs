using UnityEngine;

public class GravityChanger : MonoBehaviour, IRooms
{
    [SerializeField] private GameObject _gravityObject;
    private Vector3 _gravityDirection;
    void OnEnable()
    {
        Initialize();
    }
    public void Initialize()
    {
        _gravityDirection = _gravityObject.transform.forward;
    }
    public Vector3 GetGravityDirection()
    {
        return _gravityDirection;
    }
}
