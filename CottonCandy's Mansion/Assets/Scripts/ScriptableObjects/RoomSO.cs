using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "ScriptableObjects/RoomSO")]
public class RoomSO : ScriptableObject
{
    public string RoomName;
    public string RoomDescription;
    public GameObject MainRoom;
    public Vector3 RoomSize;
    public enum RoomType
    {
        Hallway,
        Boss,
        Special,
        Encounter,
        Treasure,
        None
    }
}


