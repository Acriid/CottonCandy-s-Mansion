using UnityEngine;

public class MockPhysicsSettings : IPhysicsChecks
{
    public bool Grounded;
    public bool CheckIfGrounded(Transform checkTransform, LayerMask groundLayer, float checkRadius = 0.2F)
    {
        return Grounded;
    }
}
