using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SlopeDetection
{
    private PlayerSlopeSettingsSO _slopeSettings;
    private float _minDotProduct;

    private Vector3 _groundNormal = Vector3.up;
    private float _currentSlopeAngle;
    private bool _isGrounded;
    private bool _isOnSlope;

    private readonly List<ContactPoint> _groundContacts = new(8);
    private bool _hasCollisionData;

    private Transform _transform;

    public SlopeDetection(PlayerSlopeSettingsSO slopeSettingsSO, Transform transformEntity)
    {
        _slopeSettings = slopeSettingsSO;

        float slopeRadians = _slopeSettings.MaxSlopeAngle * math.PI / 180;
        _minDotProduct = math.cos(slopeRadians);

        _transform = transformEntity;
    }
    public void FixedUpdateLogic()
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
            _currentSlopeAngle <= _slopeSettings.MaxSlopeAngle;
    }

    private void CalculateSlopeFromContacts()
    {
        Vector3 avgNormal = Vector3.zero;

        foreach (var contact in _groundContacts)
            avgNormal += contact.normal;

        _groundNormal = (avgNormal / _groundContacts.Count).normalized;
        _currentSlopeAngle = Vector3.Angle(_transform.up, _groundNormal);
        _isGrounded = true;
    }

    private void CalculateSlopeFromCast()
    {
        Vector3 origin = _transform.position + _transform.up * 0.1f;

        bool sphereHit = Physics.SphereCast(
            origin,
            _slopeSettings.CheckRadius,
            -_transform.up,
            out RaycastHit hit,
            _slopeSettings.CheckDistance,
            _slopeSettings.PlayerGroundLayerMask,
            QueryTriggerInteraction.Ignore);

        if (sphereHit)
        {
            _groundNormal = hit.normal;
            _currentSlopeAngle = Vector3.Angle(_transform.up, hit.normal);
            _isGrounded = true;
        }
        else
        {
            _groundNormal = _transform.up;
            _currentSlopeAngle = 0f;
            _isGrounded = false;
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & _slopeSettings.PlayerGroundLayerMask) == 0)
            return;

        _groundContacts.Clear();

        foreach (var contact in collision.contacts)
        {
            float dot = Vector3.Dot(contact.normal, _transform.up);
            if (dot >= _minDotProduct)
                _groundContacts.Add(contact);
        }

        _hasCollisionData = _groundContacts.Count > 0;
    }

    public void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & _slopeSettings.PlayerGroundLayerMask) == 0)
            return;

        _groundContacts.Clear();
        _hasCollisionData = false;
    }

    // --- Public API ---
    public bool IsGrounded() => _isGrounded;
    public bool IsOnSlope() => _isOnSlope;
    public float GetSlopeAngle() => _currentSlopeAngle;
    public Vector3 GetGroundNormal() => _groundNormal;
    public bool IsOnWalkableSlope() => IsOnSlope() && _currentSlopeAngle <= _slopeSettings.MaxSlopeAngle;

    public Vector3 GetSlopeMoveDirection(Vector3 inputDirection)
    {
        if (!_isGrounded) return inputDirection;
        return Vector3.ProjectOnPlane(inputDirection, _groundNormal).normalized;
    }
}
