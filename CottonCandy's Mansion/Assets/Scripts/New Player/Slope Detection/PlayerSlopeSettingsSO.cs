using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/PlayerSlopeSettings")]
public class PlayerSlopeSettingsSO : ScriptableObject
{
    public float MaxSlopeAngle = 45f;
    public float CheckDistance = 0.3f;
    public float CheckRadius = 0.25f;
    public LayerMask PlayerGroundLayerMask;
    public void Initialize()
    {
        PlayerGroundLayerMask = LayerMask.GetMask("Floor"); 
    }
}
