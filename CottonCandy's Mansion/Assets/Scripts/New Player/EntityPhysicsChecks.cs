using UnityEngine;

public static class EntityPhysicsChecks
{
    public static bool CheckIfGrounded(Transform checkTransform, LayerMask groundLayer, float checkRadius = 0.2f)
    {
        if (checkTransform == null) return false;
        Vector3 groundPoint = checkTransform.position;
        bool isGrounded = Physics.CheckSphere(groundPoint, checkRadius, groundLayer);
        return isGrounded;
    }
}

