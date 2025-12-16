using System.Collections.Generic;
using UnityEngine;

public class SlopeDetection : MonoBehaviour
{
    [Header("Slope Settings")]
    [SerializeField] private float _maxSlopeAngle = 45f;//Degres
    [SerializeField] private float _minDotProduct = 0.7f;

    [Header("Ground Check")]
    [SerializeField] private float _checkDistance = 0.3f;
    [SerializeField] private LayerMask _groundLayerMasks;

    [Header("References")]
    [SerializeField] private Rigidbody _rigidBody;

    private Vector3 _groundNormal = Vector3.up;
    private float _currentSlopeAngle = 0f;
    private bool _isGrounded = false;
    private bool _isOnSlope = false;


    private List<ContactPoint> _groundContacts = new(8);
    private bool _hasCollisionData = false;

    void FixedUpdate()
    {
        UpdateGroundAndSlopeInfo();
    }

    private void UpdateGroundAndSlopeInfo()
    {
        if(_hasCollisionData && _groundContacts.Count > 0)
        {
            CalculateSlopeFromContacts();
        }
        else
        {
            CalculateSlopeFromRaycast();
        }

        _isOnSlope = _isGrounded 
        && _currentSlopeAngle > 0.1f 
        && _currentSlopeAngle <= _maxSlopeAngle;
    }
    private void CalculateSlopeFromContacts()
    {
        Vector3 avgNormal = Vector3.zero;

        foreach(ContactPoint contact in _groundContacts)
        {
            avgNormal += contact.normal;
        }

        _groundNormal = (avgNormal/_groundContacts.Count).normalized;
        _currentSlopeAngle = Vector3.Angle(transform.up,_groundNormal);
        _isGrounded = true;
    }
    private void CalculateSlopeFromRaycast()
    {
        RaycastHit hit;
        Vector3 rayCastStart = transform.position;

        if(Physics.Raycast(rayCastStart, -transform.up, out hit, _checkDistance))
        {
            _groundNormal = hit.normal;
            _currentSlopeAngle = Vector3.Angle(transform.up, hit.normal);
            _isGrounded = true;
        }
        else
        {
            _groundNormal = Vector3.up;
            _currentSlopeAngle = 0f;
            _isGrounded = false;
        }
    }

    void OCollisionStay(Collision collision)
    {
        _groundContacts.Clear();

        foreach(ContactPoint contact in collision.contacts)
        {
            float dot = Vector3.Dot(contact.normal,transform.up);

            if(dot > _minDotProduct)
            {
                _groundContacts.Add(contact);
            }
        }
        _hasCollisionData = _groundContacts.Count > 0;
    }

    void OlisionExit(Collision collision)
    {
        _groundContacts.Clear();
        _hasCollisionData = false;
    }

    public bool IsGrounded() => _isGrounded;
    public bool IsOnSlope() => _isOnSlope;
    public float GetSlopeAngle() => _currentSlopeAngle;
}
