using UnityEngine;

public class PhysicsChecks : IPhysicsChecks
{
    public bool CheckIfGrounded(Transform checkTransform, LayerMask groundLayer, float checkRadius = 0.2f)
    {
        if (checkTransform == null) return false;
        Vector3 groundPoint = checkTransform.position;
        bool isGrounded = Physics.CheckSphere(groundPoint, checkRadius, groundLayer);
        return isGrounded;
    }
}

