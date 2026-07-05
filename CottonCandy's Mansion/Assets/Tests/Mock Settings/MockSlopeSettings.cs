using UnityEngine;

public class MockSlopeSettings : ISlopeDetection
{
    public bool OnWalkableSlope;
    public void FixedUpdateLogic()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 GetSlopeMoveDirection(Vector3 inputDirection)
    {
        return Vector3.up;
    }

    public bool IsOnWalkableSlope()
    {
        return OnWalkableSlope;
    }

    public void OnCollisionExitLogic(Collision collision)
    {

    }

    public void OnCollisionStayLogic(Collision collision)
    {

    }
}
