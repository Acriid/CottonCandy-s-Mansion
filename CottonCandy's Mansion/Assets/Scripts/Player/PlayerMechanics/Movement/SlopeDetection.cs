using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SlopeDetection : MonoBehaviour
{
    [Header("Slope Settings")]
    [SerializeField] private float _maxSlopeAngle = 45f;
    [SerializeField] private float _minDotProduct = 0.7f;

    [Header("Ground Check")]
    [SerializeField] private float _checkDistance = 0.3f;
    [SerializeField] private float _groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask _groundLayerMasks;

    [Header("References")]
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private CapsuleCollider _capsule;

    private Vector3 _groundNormal = Vector3.up;
    private float _currentSlopeAngle;
    private bool _isGrounded;
    private bool _isOnSlope;

    private readonly List<ContactPoint> _groundContacts = new(8);
    private bool _hasCollisionData;
    void Start()
    {
        float slopeRadians = _maxSlopeAngle * math.PI / 180;
        _minDotProduct = math.cos(slopeRadians);
    }
    void FixedUpdate()
    {
        UpdateGroundAndSlopeInfo();
    }

    private void UpdateGroundAndSlopeInfo()
    {
        if (_hasCollisionData)
            CalculateSlopeFromContacts();
        else
            CalculateSlopeFromCast();

        _isOnSlope =
            _isGrounded &&
            _currentSlopeAngle > 0.1f &&
            _currentSlopeAngle <= _maxSlopeAngle;
    }

    private void CalculateSlopeFromContacts()
    {
        Vector3 avgNormal = Vector3.zero;

        foreach (var contact in _groundContacts)
            avgNormal += contact.normal;

        _groundNormal = (avgNormal / _groundContacts.Count).normalized;
        _currentSlopeAngle = Vector3.Angle(transform.up, _groundNormal);
        _isGrounded = true;
    }

    private void CalculateSlopeFromCast()
    {
        Vector3 origin = transform.position + transform.up * 0.1f;

        if (Physics.SphereCast(
            origin,
            _groundCheckRadius,
            -transform.up,
            out RaycastHit hit,
            _checkDistance,
            _groundLayerMasks,
            QueryTriggerInteraction.Ignore))
        {
            _groundNormal = hit.normal;
            _currentSlopeAngle = Vector3.Angle(transform.up, hit.normal);
            _isGrounded = true;
        }
        else
        {
            _groundNormal = transform.up;
            _currentSlopeAngle = 0f;
            _isGrounded = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & _groundLayerMasks) == 0)
            return;

        _groundContacts.Clear();

        foreach (var contact in collision.contacts)
        {
            float dot = Vector3.Dot(contact.normal, transform.up);
            if (dot >= _minDotProduct)
                _groundContacts.Add(contact);
        }

        _hasCollisionData = _groundContacts.Count > 0;
    }

    void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & _groundLayerMasks) == 0)
            return;

        _groundContacts.Clear();
        _hasCollisionData = false;
    }

    // --- Public API ---
    public bool IsGrounded() => _isGrounded;
    public bool IsOnSlope() => _isOnSlope;
    public float GetSlopeAngle() => _currentSlopeAngle;
    public Vector3 GetGroundNormal() => _groundNormal;
    public bool IsOnWalkableSlope() => IsOnSlope() && _currentSlopeAngle <= _maxSlopeAngle;

    public Vector3 GetSlopeMoveDirection(Vector3 inputDirection)
    {
        if (!_isGrounded) return inputDirection;
        return Vector3.ProjectOnPlane(inputDirection, _groundNormal).normalized;
    }
}
