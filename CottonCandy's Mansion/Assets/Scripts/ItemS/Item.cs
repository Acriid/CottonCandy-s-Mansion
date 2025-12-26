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
    void OnEnable()
    {
        Initialize();
    }

    void FixedUpdate()
    {
        if(_useGravity)
        {
            _rigidBody.AddForce(_gravityDirection * ItemSO.ItemGravity,ForceMode.Force);
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

    public void ChangeUsedLayerMasks(LayerMask layerMask)
    {
        
    }

    public void UseGravity(bool newValue)
    {
        _useGravity = newValue;
    }
}
