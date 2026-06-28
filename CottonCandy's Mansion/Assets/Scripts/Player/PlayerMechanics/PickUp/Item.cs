using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
    public ItemSO ItemSO;
    [SerializeField] private Vector3 _gravityDirection = Vector3.down;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Collider _collider;
    [SerializeField] private bool _useGravity;
    [SerializeField] private float _gravityModifier;
    void OnEnable()
    {
        Initialize();
    }

    void FixedUpdate()
    {
        if(_useGravity)
        {
            _rigidBody.AddForce(_gravityDirection * _gravityModifier,ForceMode.Force);
        }
    }
    private void Initialize()
    {
        if (_rigidBody == null) _rigidBody = GetComponent<Rigidbody>();

        if(_rigidBody.useGravity)
        {
            _rigidBody.useGravity = false;
        }

        if(_collider == null) _collider = GetComponent<Collider>();
        
    }

    public void ChangeExcludeLayerMasks(LayerMask layerMask)
    {
        _collider.excludeLayers = layerMask;
    }

    public void UseGravity(bool newValue)
    {
        _useGravity = newValue;
    }
    public void ChangeGravityDirection(Vector3 newDirection)
    {
        _gravityDirection = newDirection;
    }
    public void ChangeGravityModifier(float newValue)
    {
        _gravityModifier = newValue;
    }

    public void KinematicRigidBody(bool newValue)
    {
        _rigidBody.isKinematic = newValue;
    }
}
