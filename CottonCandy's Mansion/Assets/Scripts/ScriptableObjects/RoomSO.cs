using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "ScriptableObjects/RoomSO")]
public class RoomSO : ScriptableObject
{
    public string RoomName;
    public string RoomDescription;
    public Vector3 RoomSize;
    public GameObject MainRoom;

    public void InitializeLists()
    {
        MainRoom.GetComponentInChildren<Transform>();
    }

    /*Adding on rooms equal to room sizes (y if adding forward/back and x if adding to the sides) 
      added together and divided by 2. This equals the room spawn spot.*/

    public enum RoomType
    {
        
    }
}
