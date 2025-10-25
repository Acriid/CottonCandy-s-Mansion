using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "ScriptableObjects/RoomSO")]
public class RoomSO : ScriptableObject
{
    public string RoomName;
    public string RoomDescription;
    public int RoomCost;
    public Vector3 RoomSize;
    public GameObject MainRoom;
    private List<GameObject> RoomObjects;
    private List<GameObject> SpawnZones;

    public void InitializeLists()
    {
        MainRoom.GetComponentInChildren<Transform>();
    }
    
    /*Adding on rooms equal to room sizes (y if adding forward/back and x if adding to the sides) 
      added together and divided by 2. This equals the room spawn spot.*/
}
