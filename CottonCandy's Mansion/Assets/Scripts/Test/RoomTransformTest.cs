using UnityEngine;

public class RoomTransformTest : MonoBehaviour
{
    public RoomSO roomSO;
    public GameObject roomToRotateTo;
    public GameObject roomToRotateFrom;
    //
    void Awake()
    {
        Transform doorTransform = roomSO.MainRoom.transform.Find("Structure/Doors/Door_Next");
//        Debug.Log(doorTransform.rotation);
        doorTransform.rotation = Quaternion.RotateTowards(doorTransform.rotation, roomSO.MainRoom.transform.rotation,360f);
   //     Debug.Log(doorTransform.rotation);
        roomToRotateFrom.transform.rotation = Quaternion.RotateTowards(roomToRotateFrom.transform.rotation, roomToRotateTo.transform.rotation,360f);
    }
}
