using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "ScriptableObjects/RoomSO")]
public class RoomSO : ScriptableObject
{
    public string RoomName;
    public string RoomDescription;
    public List<GameObject> RoomObjects;
    public int RoomCost;
    
    
}
