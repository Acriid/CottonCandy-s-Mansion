using UnityEngine;

public class PlayerAttributes : ScriptableObject
{
    #region Movement
    public float PlayerSpeed {get; private set;}
    public float GroundDrag {get; private set;}
    public float MaxSlopeAngle {get; private set;}
    #endregion
    #region Jump
    public int MaxJumps {get; private set;}
    public float JumpForce {get; private set;}
    #endregion
    #region Pickup
    public float DetectionRadius {get; private set;}
    #endregion
    #region Gravity
    public float GravityModifier {get; private set;}
    #endregion
}
