using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Rooms/Room")]
public class RoomSO : ScriptableObject
{
    public string RoomName;
    public string RoomDescription;
    public GameObject MainRoom;
    public Vector3 RoomSize;
    public RoomType Type;
    public enum RoomType
    {
        Hallway,
        Boss,
        Special,
        Encounter,
        Treasure,
        End,
        None
    }
}


