using Unity.VisualScripting;
using UnityEngine;

public class TransformFindTest : MonoBehaviour
{
    public string TransformToFind;
    public Transform rootTransform;
    public RoomSO roomSO;
    void Awake()
    {
        GameObject room = Instantiate(roomSO.MainRoom, Vector3.zero, Quaternion.identity, rootTransform);
        Transform flooar = room.transform.Find("Structure/Floor");
        Transform doors = room.transform.Find("Structure/Doors/Next");
        Vector3 newSpawn = Vector3.zero;
        foreach (Transform door in doors)
        {
            Vector3 newForward = door.forward * -1;
            newSpawn = Vector3.Scale(door.position,newForward) + newForward/2 + Vector3.Scale(roomSO.RoomSize/2,newForward);
            Debug.Log(newSpawn);
        }
        Instantiate(roomSO.MainRoom, newSpawn, Quaternion.identity, rootTransform);
        
    }
}
