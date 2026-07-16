using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/PlayerSettings")]
public class PlayerSettingsSO : ScriptableObject
{
    [Header("Camera")]
    public float PlayerLookSensitivity = 20f;

    [Header("Player Movement")]
    public float PlayerSpeed = 12.5f;
    public float PlayerDrag = 8f;


    [Header("Player Gravity")]
    public Vector3 PlayerGravityDirection = new(0f,-1f,0f);
    public float PlayerGravityModifier = 9.8f;
    public LayerMask PlayerGroundLayerMask;
    public float PlayerTimeToRotate = 1f;
    public void Initialize()
    {
        PlayerGroundLayerMask = LayerMask.GetMask("Floor"); 
    }
}
