using UnityEngine;

public class GravityChanger : MonoBehaviour, IGravity
{
    [SerializeField] private Vector3 _gravityDirection;
    [SerializeField] private GameObject _gravityObject;
    public void ChangeGravity(ref Vector3 originalDirection)
    {
        if(_gravityDirection != null)
        originalDirection = _gravityDirection;
    }

    public void Initialize()
    {
        if(_gravityObject != null)
        {
            _gravityDirection = _gravityObject.transform.forward;
        }
    }

    public void SetGravityDirection(Vector3 newDirection)
    {
        _gravityDirection = newDirection;
    }

    public Vector3 GetGravityDirection()
    {
        return _gravityDirection;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            if(collision.gameObject.TryGetComponent(out Player playerComponent))
            {
                ChangeGravity(ref playerComponent.GetPlayerSettings().PlayerGravityDirection);
            }
        }
    }
}
