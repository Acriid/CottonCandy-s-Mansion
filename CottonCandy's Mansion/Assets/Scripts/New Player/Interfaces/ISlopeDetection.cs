using UnityEngine;

public interface ISlopeDetection
{
    void FixedUpdateLogic();
    bool IsOnWalkableSlope();
    Vector3 GetSlopeMoveDirection(Vector3 inputDirection);
    void OnCollisionStayLogic(Collision collision);
    void OnCollisionExitLogic(Collision collision);

}
