using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player_New : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Transform _orientation;
    private Vector3 _gravityDirection = new(0f,1f,0f);
    [SerializeField] private float _gravityModifier = 9.8f;
    [SerializeField] private float _playerSpeed = 12.5f;

    void Awake()
    {
        if(!TryGetComponent(out _rigidBody))
        {
            Debug.LogWarning("No RigidBody found on the player");
        }

        if(_orientation == null)
        {
            Debug.LogWarning("No player orientation");
        }
    }
    public void MovePlayer(Vector2 direction)
    {
        Vector3 playerUp = -_gravityDirection.normalized;

        Vector3 moveDirection = _orientation.forward * direction.y + _orientation.right * direction.x;

        _rigidBody.AddForce(_playerSpeed * moveDirection.normalized, ForceMode.Force);
    }
    private void SpeedControl()
    {
        Vector3 gravityComp = Vector3.Project(_rigidBody.linearVelocity, _gravityDirection);
        if(gravityComp.magnitude > 2f * _gravityModifier)
        {
            gravityComp = _gravityModifier * 2f * gravityComp.normalized;
        }

        Vector3 nonGravityComp = _rigidBody.linearVelocity - gravityComp;
        if (nonGravityComp.magnitude > _playerSpeed)
        {
            Vector3 clampedNonGravity = nonGravityComp.normalized * _playerSpeed;
            _rigidBody.linearVelocity = clampedNonGravity + gravityComp;
        }
    }
}
