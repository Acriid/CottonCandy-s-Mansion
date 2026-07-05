using UnityEngine;
public interface IPhysicsChecks 
{
    bool CheckIfGrounded(
        Transform checkTransform, 
        LayerMask groundLayer, 
        float checkRadius = 0.2f
    );
}
