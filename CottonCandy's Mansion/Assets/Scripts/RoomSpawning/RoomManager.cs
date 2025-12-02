using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using Unity.VisualScripting;

public class RoomManager : MonoBehaviour
{
    #region Global vars

    public GameObject TestBoundsObj;
    public Material TestObjMat;

    public static RoomManager Instance;

    public Transform RoomParent;

    private Dictionary<RoomSO.RoomType, List<RoomSO>> _roomDictionary = new();//Holds all the rooms sorted by type
    private List<RoomSO> _rooms = new();
    public List<GameObject> _level = new();

    public RoomSO RoomtoSpawn;
    public RoomSO RoomtoSpawn2;

    #region String Constants
    const string DOORSTRING = "Structure/Doors/Door_Next";//Temp to find next spawn door
    const string UPPEROFFSETSTRING = "HeightOffset/Next";
    const string LOWEROFFSETSTRING = "HeightOffset/Prev";
    const string LOADLABLE = "Room";
    #endregion
    
    #endregion
    async void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { return; }

        await LoadAllRooms();

        SpawnLevel(10);
    }

    private async Task LoadAllRooms()
    {
        //Loads all rooms into dictionary
        AsyncOperationHandle<IList<RoomSO>> _handle = Addressables.LoadAssetsAsync<RoomSO>(LOADLABLE, null);

        await _handle.Task;

        if (_handle.Status == AsyncOperationStatus.Succeeded)
        {
            _rooms.AddRange(_handle.Result);
        }
        else
        {
            Debug.LogError("Failed to load rooms.");
        }

        Addressables.Release(_handle);

        foreach (RoomSO.RoomType roomType in Enum.GetValues(typeof(RoomSO.RoomType)))
        {
            _roomDictionary.Add(roomType, new List<RoomSO>());
        }
        foreach (RoomSO room in _rooms)
        {
            _roomDictionary[room.Type].Add(room);
        }
    }
    #region SpawnRoom
    private void SpawnLevel(int levelSize)
    {
        List<RoomSO> roomList = new(GetRoomList(RoomSO.RoomType.Hallway));

        RoomSO randomRoom = GetRandomRoom(roomList);
        Vector3 roomPosition = RoomParent.transform.position;
        Quaternion roomRotation = RoomParent.transform.rotation;

        GameObject currentRoom = SpawnRoom(randomRoom,roomPosition,roomRotation);

        _level.Add(currentRoom);

        int failSafe = 0;

        while(_level.Count < levelSize && failSafe < 100 && roomList.Count > 0)
        {
            randomRoom = GetRandomRoom(roomList);
            roomList.Remove(randomRoom);
            roomPosition = GetNewRoomPosition(randomRoom,currentRoom);
            roomRotation = GetRoomRotation(currentRoom);

            if(!OverLapRoom(randomRoom.RoomSize,roomPosition,roomRotation) && TestIfRoomCanSpawn(randomRoom,roomPosition,roomRotation))
            {
                roomList = new(GetRoomList(RoomSO.RoomType.Hallway));
                currentRoom = SpawnRoom(randomRoom,roomPosition,roomRotation);

                SpawnBox(randomRoom.RoomSize,roomPosition,roomRotation);
                _level.Add(currentRoom);
            }

            failSafe++;
        }
    }

    private GameObject SpawnRoom(RoomSO roomToSpawn, Vector3 position, Quaternion rotation)
    {
        return Instantiate(roomToSpawn.MainRoom,position,rotation,RoomParent);
    }

    private bool OverLapRoom(Vector3 roomSize, Vector3 roomPos, Quaternion roomRotation)
    {

        bool Check = Physics.CheckBox(
            roomPos,
            (roomSize - Vector3.one)/2f,
            roomRotation
        );

        return Check;
    }
    private void SpawnBox(Vector3 roomSize, Vector3 roomPos, Quaternion roomRotation)
    {
        GameObject currentObj = Instantiate(TestBoundsObj,roomPos,roomRotation);
        currentObj.GetComponent<MeshRenderer>().material = TestObjMat;
        currentObj.transform.localScale = roomSize - Vector3.one;
    }
    private bool TestIfRoomCanSpawn(RoomSO roomToCheck, Vector3 roomPosition, Quaternion roomRotation)
    {
        //Gets room from roomlist
        //TODO: Change hardcode to a paramater in function
        List<RoomSO> roomList = new(GetRoomList(RoomSO.RoomType.Hallway));
        RoomSO testRoom = GetRandomRoom(roomList);
        roomList.Remove(testRoom);

        Vector3 testRoomPosition = GetNewRoomPosition(testRoom,roomToCheck,roomPosition,roomRotation);
        Quaternion testRoomRotation = GetRoomRotation(roomToCheck,roomRotation);

        bool result = OverLapRoom(testRoom.RoomSize,testRoomPosition,testRoomRotation);

        //loops through list to see if any room can spawn
        while(result && roomList.Count > 0)
        {
            testRoom = GetRandomRoom(roomList);
            roomList.Remove(testRoom);

            testRoomPosition = GetNewRoomPosition(testRoom,roomToCheck,roomPosition,roomRotation);

            result = OverLapRoom(testRoom.RoomSize,testRoomPosition,testRoomRotation); 
        }

        return !result;
    }
    private RoomSO GetNextRoom()
    {
        //TODO: When room order is figured out it gets the next room
        return null;
    }
    #endregion
    #region RoomPositions
    private Vector3 GetNewRoomPosition(RoomSO roomToSpawn, GameObject currentRoom)
    {
        Transform spawnDoor = currentRoom.transform.Find(DOORSTRING);
        Transform lowerOffset = roomToSpawn.MainRoom.transform.Find(LOWEROFFSETSTRING);

        if (spawnDoor == null)
        {
            Debug.LogError($"{currentRoom} has no Next Door");
            return currentRoom.transform.position;
        }


        Vector3 newForward = -spawnDoor.forward.normalized; // opposite of current door’s forward
        Quaternion roomRotation = GetRoomRotation(currentRoom);

        Vector3 heightOffset = Vector3.zero;
        if (lowerOffset != null) heightOffset.y += Mathf.Abs(lowerOffset.localPosition.y);
        heightOffset = roomRotation * heightOffset;

        Vector3 halfRoomSize = roomToSpawn.RoomSize / 2f;
        Vector3 roomOffset = roomRotation * halfRoomSize;
        float forwardDot = Vector3.Dot(roomOffset, newForward);
        Vector3 roomScaleOffset = forwardDot * newForward;

        Vector3 newRoomPosition = spawnDoor.position + roomScaleOffset + heightOffset + newForward /2f;

        return newRoomPosition;
    }

    private Vector3 GetNewRoomPosition(RoomSO roomToSpawn, RoomSO currentRoom, Vector3 roomPosition, Quaternion roomRotation)
    {
        
        Transform spawnDoor = currentRoom.MainRoom.transform.Find(DOORSTRING);
        Transform lowerOffset = roomToSpawn.MainRoom.transform.Find(LOWEROFFSETSTRING);

        if (spawnDoor == null)
        {
            Debug.LogError($"{currentRoom} has no Next Door");
            return roomPosition;
        }

        Quaternion normalizedRot = Quaternion.Normalize(roomRotation);

        Vector3 doorForward = normalizedRot * spawnDoor.forward;
        Vector3 newForward = -doorForward.normalized; //Mirror because doors face inward to rooms
        Vector3 worldSpawnDoorPos = roomPosition + (normalizedRot * spawnDoor.localPosition);

        Vector3 heightOffset = Vector3.zero;
        if (lowerOffset != null) heightOffset.y += Mathf.Abs(lowerOffset.localPosition.y);
        heightOffset = normalizedRot * heightOffset;

        Quaternion NewRoomRotaion = Quaternion.Normalize(GetRoomRotation(currentRoom,normalizedRot));

        Vector3 halfRoomSize = roomToSpawn.RoomSize / 2f;
        Vector3 roomOffset = NewRoomRotaion * halfRoomSize;
        float forwardDot = Vector3.Dot(roomOffset, newForward);
        Vector3 roomScaleOffset = forwardDot * newForward;

        Vector3 newRoomPosition = worldSpawnDoorPos + roomScaleOffset + heightOffset + newForward * 0.5f;//TODO get rid of 0.5f magic number
        return newRoomPosition;
    }
    private Quaternion GetRoomRotation(GameObject currentRoom)
    {
        //Gets room and door
        Transform spawnDoor = currentRoom.transform.Find(DOORSTRING);
        //Mirrors forward
        Vector3 newForward = -spawnDoor.forward;
        Vector3 newUp = spawnDoor.up;

        Quaternion result = Quaternion.LookRotation(newForward, newUp);

        return result;
    }
    private Quaternion GetRoomRotation(RoomSO currentRoom, Quaternion roomRotation)
    {
        // Find Door Prefab
        Transform spawnDoor = currentRoom.MainRoom.transform.Find(DOORSTRING);

        if (spawnDoor == null)
        {
            Debug.LogError($"Door '{DOORSTRING}' not found in {currentRoom.MainRoom.name}");
            return roomRotation; 
        }

        // Get new forward
        Vector3 worldForward = roomRotation * -spawnDoor.forward;
        Vector3 worldUp = roomRotation * spawnDoor.up;

        // Return rotation from that forward
        Quaternion result = Quaternion.LookRotation(worldForward, worldUp);
        return result;
    }
    private Vector3 GetNewRoomSize(RoomSO room, Quaternion roomRotation)
    {
        Vector3 sizeOffset = roomRotation * Vector3.one;
        sizeOffset = new(Mathf.Abs(sizeOffset.x),Mathf.Abs(sizeOffset.y),Mathf.Abs(sizeOffset.z));

        Vector3 rotationResult = roomRotation * room.RoomSize;
        rotationResult = new(Mathf.Abs(rotationResult.x),Mathf.Abs(rotationResult.y),Mathf.Abs(rotationResult.z));

        return rotationResult - sizeOffset;
    }
    #endregion
    #region GetRooms
    private RoomSO GetRandomRoom(List<RoomSO> roomList)
    {
        //ListCheck
        if (roomList.Count == 0)
        {
            return null;
        }
        //Gets Random Room From list
        int upperBoundry = roomList.Count;
        int randomRoomIndex = UnityEngine.Random.Range(0, upperBoundry);
        RoomSO result = roomList[randomRoomIndex];

        return result;
    }

    private List<RoomSO> GetRoomList(RoomSO.RoomType roomType)
    {
        List<RoomSO> roomList;
        if (_roomDictionary.TryGetValue(roomType, out roomList))
            return roomList;
        else
            return null;
    }
    #endregion

}


[Serializable]
public class _roomDictionary
{
    [SerializeField] _roomDictionaryRoom[] new_roomDictionary;
    public Dictionary<RoomSO.RoomType,float> toDictionary()
    {
        Dictionary<RoomSO.RoomType, float> newDictionary = new Dictionary<RoomSO.RoomType, float>();
        foreach (var item in new_roomDictionary)
        {
            newDictionary.Add(item.type, item.chance);
        }
        return newDictionary;
    }
}
[Serializable]
public class _roomDictionaryRoom
{
    [SerializeField] public RoomSO.RoomType type;
    [SerializeField] public float chance;
}