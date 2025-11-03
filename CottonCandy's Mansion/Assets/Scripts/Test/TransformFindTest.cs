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
            newSpawn = new Vector3(0f, flooar.position.y, door.position.z) + -0.5f * door.forward + new Vector3(0f, 0f, roomSO.RoomSize.z / 2);
            Debug.Log(newSpawn);
        }
        Instantiate(roomSO.MainRoom, newSpawn, Quaternion.identity, rootTransform);
    }
}
