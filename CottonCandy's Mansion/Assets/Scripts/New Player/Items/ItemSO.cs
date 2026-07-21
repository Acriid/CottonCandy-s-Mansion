using UnityEngine;

public class ItemSO : ScriptableObject
{
    public bool GravityAffected = true;
    public Vector3 ItemGravityDirection = new(0f,-1f,0f);
    public string ItemName = "CandyCotton";
    public string ItemDescription = "Fairy a you that not are to supposed see.";
}
