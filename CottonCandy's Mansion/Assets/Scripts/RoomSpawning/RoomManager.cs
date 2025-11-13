using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;

public class RoomManager : MonoBehaviour
{
    #region Global vars
    public static RoomManager instance;
    public Transform roomParent;
    private Dictionary<RoomSO.RoomType, List<RoomSO>> RoomDictionary = new();
    private List<RoomSO> SpecificRoomsList = new();
    private List<RoomSO> rooms = new();
    public RoomSO RoomtoSpawn;
    public RoomSO RoomtoSpawn2;
    #region String Constants
    //Doors
    const string DoorString = "Structure/Doors/Door_Next";
    //HeightOffsets
    const string UpperOffsetString = "HeightOffset/Next";
    const string LowerOffsetString = "HeightOffset/Prev";
    //Room Loading
    const string LoadLable = "Room";
    #endregion
    #endregion
    async void Awake()
    {
        if (instance == null) { instance = this; }
        else { return; }

        await LoadAllRooms();

        TestSpawn();
    }

    private async Task LoadAllRooms()
    {
        AsyncOperationHandle<IList<RoomSO>> handle = Addressables.LoadAssetsAsync<RoomSO>(LoadLable, null);

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            rooms.AddRange(handle.Result);
        }
        else
        {
            Debug.LogError("Failed to load rooms.");
        }

        Addressables.Release(handle);
        foreach (RoomSO.RoomType roomType in Enum.GetValues(typeof(RoomSO.RoomType)))
        {
            RoomDictionary.Add(roomType, new List<RoomSO>());
        }
        foreach (RoomSO room in rooms)
        {
            RoomDictionary[room.Type].Add(room);
        }
    }
    #region SpawnRoom
    private void TestSpawn()
    {
        List<RoomSO> RoomList = GetRoomList(RoomSO.RoomType.Hallway);
        List<GameObject> CurrentLevel = new();
        //new Quaternion(1f,0.5f,0f,0f)
        RoomSO randomRoom = GetRandomRoom(RoomList);
        GameObject CurrentRoom = Instantiate(RoomtoSpawn.MainRoom, Vector3.zero,new Quaternion(1f,0.5f,0f,0f) , roomParent);
        Vector3 RoomPosition = GetNewRoomPosition(RoomtoSpawn2, CurrentRoom);
        Quaternion RoomRotation = GetRoomRotation(CurrentRoom);
        Debug.Log(CurrentRoom.transform.position);
        Instantiate(RoomtoSpawn2.MainRoom, RoomPosition, RoomRotation, roomParent);

    }
    private void SpawnLevel(int levelSize)
    {
        List<RoomSO> RoomList = GetRoomList(RoomSO.RoomType.Hallway);
        List<Bounds> BoundsList = new();
        List<GameObject> CurrentLevel = new();
        for(int i = 0; i < levelSize; i++)
        {
            CurrentLevel.Add(SpawnRoom(RoomList, BoundsList, CurrentLevel));
        }
    }
    private GameObject SpawnRoom(List<RoomSO> RoomList, List<Bounds> BoundsList, List<GameObject> CurrentLevel)
    {
        List<RoomSO> roomList = new(RoomList);
        RoomSO randomRoom = GetRandomRoom(roomList);
        GameObject SpawnedRoom = null;
        GameObject CurrentRoom = null;
        Bounds RoomBounds;
        if (CurrentLevel.Count == 0)
        {
            SpawnedRoom = Instantiate(randomRoom.MainRoom, Vector3.zero, Quaternion.identity, roomParent);
            RoomBounds = new(SpawnedRoom.transform.position, randomRoom.RoomSize);
            BoundsList.Add(RoomBounds);
            return SpawnedRoom;
        }
        else
        {
            CurrentRoom = CurrentLevel[^1];
        }
        bool CanContinue = CheckIfCanContinue(randomRoom, CurrentRoom, roomList, BoundsList);

        while (!CanContinue && roomList.Count > 0)
        {
            RemoveFromList(randomRoom, roomList);
            if(roomList.Count > 0)
            {
                randomRoom = GetRandomRoom(roomList);
            }
            CanContinue = CheckIfCanContinue(randomRoom, CurrentRoom, roomList, BoundsList);
        }
        
        if (roomList.Count <= 0)
        {
            return null;
        }
        
        Vector3 RoomPosition = GetNewRoomPosition(randomRoom, CurrentRoom);
        Quaternion RoomRotation = GetRoomRotation(CurrentRoom);
        Bounds spawnBounds = new(RoomPosition, randomRoom.RoomSize - Vector3.one);
        SpawnedRoom = Instantiate(randomRoom.MainRoom, RoomPosition, RoomRotation, roomParent);
        BoundsList.Add(spawnBounds);

        return SpawnedRoom;
    }
    #region RoomPositions

    private Vector3 GetNewRoomPosition(RoomSO roomToSpawn, GameObject currentRoom)
    {
        // --- Step 1: Get references ---
        Transform spawnDoor = currentRoom.transform.Find(DoorString);
        Transform upperOffset = currentRoom.transform.Find(UpperOffsetString);
        Transform lowerOffset = roomToSpawn.MainRoom.transform.Find(LowerOffsetString);

        if (spawnDoor == null)
        {
            Debug.LogError($"Door '{DoorString}' not found in {currentRoom.name}");
            return currentRoom.transform.position;
        }

        // --- Step 2: Get direction and rotation ---
        Vector3 newForward = -spawnDoor.forward.normalized; // opposite of current door’s forward
        Quaternion roomRotation = GetRoomRotation(currentRoom);

        // --- Step 3: Height offset ---
        Vector3 heightOffset = Vector3.zero;
        if (upperOffset != null) heightOffset.y += upperOffset.localPosition.y;
        if (lowerOffset != null) heightOffset.y += Mathf.Abs(lowerOffset.localPosition.y);
        heightOffset = roomRotation * heightOffset;

        // --- Step 4: Room offset along spawn direction ---
        Vector3 halfRoomSize = roomToSpawn.RoomSize / 2f;
        Vector3 roomOffset = roomRotation * halfRoomSize;
        float forwardDot = Vector3.Dot(roomOffset, newForward);
        Vector3 roomScaleOffset = forwardDot * newForward;
        Debug.Log(spawnDoor.position);
        // --- Step 5: Compute final position ---
        Vector3 newRoomPosition = spawnDoor.position + roomScaleOffset + heightOffset + newForward /2f;

        return newRoomPosition;
    }

    private Vector3 GetNewRoomPosition(RoomSO roomToSpawn, RoomSO CurrentRoom, Vector3 RoomPosition, Quaternion RoomRotation)
    {
        Vector3 CurrentRoomDimensions = RoomPosition;
        Transform UpperOffset = CurrentRoom.MainRoom.transform.Find(UpperOffsetString);
        Transform LowerOffset = roomToSpawn.MainRoom.transform.Find(LowerOffsetString);
        Vector3 HeightOffset = Vector3.zero;

        if (UpperOffset != null)
        {
            HeightOffset.y += UpperOffset.localPosition.y;

        }
        if (LowerOffset != null)
        {
            HeightOffset.y += Mathf.Abs(LowerOffset.localPosition.y);
        }

        Transform spawnDoor = CurrentRoom.MainRoom.transform.Find(DoorString);
        Vector3 doorForward = spawnDoor.forward * -1;
        doorForward = RoomRotation * doorForward;


        Vector3 ResultVector = CompareVectors(CurrentRoomDimensions, doorForward, 0.001f);
        Vector3 newDoorPosition = Vector3.Scale(Vector3.Scale(spawnDoor.position + CurrentRoomDimensions, doorForward), doorForward);
        Vector3 newSpawn = newDoorPosition + doorForward / 2f + Vector3.Scale(RoomRotation * roomToSpawn.RoomSize / 2f, doorForward) +
        Vector3.Scale(CurrentRoomDimensions, ResultVector);


        return newSpawn;
    }
    private Quaternion GetRoomRotation(GameObject CurrentRoom)
    {
        //Gets room and door
        Transform spawnDoor = CurrentRoom.transform.Find(DoorString);
        //Mirrors forward
        Vector3 newForward = -spawnDoor.forward;
        Vector3 newUp = spawnDoor.up;

        Quaternion result = Quaternion.LookRotation(newForward, newUp);

        return result;
    }
private Vector3 GPTGetNewRoomPosition(RoomSO roomToSpawn, RoomSO currentRoom, Vector3 roomPosition, Quaternion roomRotation)
{
    // --- Step 1: Get reference points from ScriptableObjects ---
    Transform spawnDoor = currentRoom.MainRoom.transform.Find(DoorString);
    Transform upperOffset = currentRoom.MainRoom.transform.Find(UpperOffsetString);
    Transform lowerOffset = roomToSpawn.MainRoom.transform.Find(LowerOffsetString);

    if (spawnDoor == null)
    {
        Debug.LogError($"Door '{DoorString}' not found in {currentRoom.MainRoom.name}");
        return roomPosition;
    }

    // --- Step 2: Get direction and rotation ---
    // Door forward in world space (rotated by current room's rotation)
    Vector3 doorForward = roomRotation * spawnDoor.forward;
    Vector3 newForward = -doorForward.normalized; // opposite of door direction

    // --- Step 3: Height offset (in world space) ---
    Vector3 heightOffset = Vector3.zero;
    if (upperOffset != null)
        heightOffset.y += upperOffset.localPosition.y;
    if (lowerOffset != null)
        heightOffset.y += Mathf.Abs(lowerOffset.localPosition.y);

    // Rotate height offset into world orientation
    heightOffset = roomRotation * heightOffset;

    // --- Step 4: Room offset along spawn direction ---
    Vector3 halfRoomSize = roomToSpawn.RoomSize / 2f;
    Vector3 roomOffset = roomRotation * halfRoomSize;
    float forwardDot = Vector3.Dot(roomOffset, newForward);
    Vector3 roomScaleOffset = forwardDot * newForward;

    // --- Step 5: Compute world position of the door (in current room space) ---
    Vector3 worldSpawnDoorPos = roomPosition + (roomRotation * (spawnDoor.localPosition));

    // --- Step 6: Compute final new room position ---
    Vector3 newRoomPosition = worldSpawnDoorPos + roomScaleOffset + heightOffset + newForward / 2f;

    return newRoomPosition;
}

    #endregion
    private bool CheckIfCanContinue(RoomSO roomToCheck, GameObject CurrentRoom, List<RoomSO> RoomList, List<Bounds> BoundsList)
    {
        //Checks if the dungeon can continue if this room spawns.
        //If not do not spawn this room.
        //If can spawn this room
        //Checks by getting the current room spawn for this room (GetNewRoomPosition) 
        //Then gets a calculated room spawn for where the next room will spawn
        //If no next room can spawn then return false.
        //If a next room can spawn return true/


        List<RoomSO> rooms = new(RoomList);
        List<Bounds> bounds = new(BoundsList);
        
        Vector3 RoomPosition = GetNewRoomPosition(roomToCheck, CurrentRoom);
        Quaternion RoomRotation = GetRoomRotation(CurrentRoom);
        Bounds spawnBounds = new(RoomPosition, roomToCheck.RoomSize - Vector3.one);
        Instantiate(roomToCheck.MainRoom, RoomPosition, RoomRotation, roomParent);
        
        if(!TestIfCanSpawn(spawnBounds,bounds))
        {
            return false;
        }
        bounds.Add(spawnBounds);

        int Failsafe = 0;
        bool CanSpawn;
        do
        {
            Failsafe++;
            RoomSO roomAfterThis = GetRandomRoom(rooms);
            Debug.Log(roomAfterThis);
            Vector3 RoomPosition2 = GetNewRoomPosition(roomAfterThis, roomToCheck, RoomPosition, RoomRotation);
            Quaternion RoomRotation2 = GetRoomRotation(roomToCheck.MainRoom);
            Bounds spawnBounds2 = new(RoomPosition2, roomAfterThis.RoomSize - Vector3.one);
            CanSpawn = TestIfCanSpawn(spawnBounds2, bounds);
            if (!CanSpawn)
            {
                RemoveFromList(roomAfterThis, rooms);
            }
            Debug.Log(CanSpawn);
        } while (!CanSpawn && rooms.Count > 0 && Failsafe < 1000);      
        
        return CanSpawn;
    }
    #endregion
    #region GetRooms
    private RoomSO GetRandomRoom(List<RoomSO> RoomList)
    {
        //ListCheck
        if (RoomList.Count == 0)
        {
            return null;
        }
        //Gets Random Room From list
        int UpperBoundry = RoomList.Count - 1;
        int RandomRoom = UnityEngine.Random.Range(0, UpperBoundry);
        RoomSO result = RoomList[RandomRoom];

        return result;
    }

    private List<RoomSO> GetRoomList(RoomSO.RoomType roomType)
    {
        List<RoomSO> RoomList;
        if (RoomDictionary.TryGetValue(roomType, out RoomList))
            return RoomList;
        else
            return null;
    }
    #endregion

    private void RemoveFromList<T>(T removeValue, List<T> RemoveList)
    {
        if (RemoveList.Count == 0) { return; }
        RemoveList.Remove(removeValue);
    }


    private Vector3 CompareVectors(Vector3 a, Vector3 b, float tolerance = 0.0001f)
    {
        return new Vector3(
            (Mathf.Abs(a.x) > tolerance && Mathf.Abs(b.x) <= tolerance) ? 1 : 0,
            (Mathf.Abs(a.y) > tolerance && Mathf.Abs(b.y) <= tolerance) ? 1 : 0,
            (Mathf.Abs(a.z) > tolerance && Mathf.Abs(b.z) <= tolerance) ? 1 : 0
        );
    }

    
    private bool TestIfCanSpawn(Bounds SpawnBounds, List<Bounds> CompareBounds)
    {
        foreach(Bounds bounds in CompareBounds)
        {
            if(bounds.Intersects(SpawnBounds))
            {
                return false;
            }
        }
        return true;
    }

}


[Serializable]
public class RoomDictionary
{
    [SerializeField] RoomDictionaryRoom[] newRoomDictionary;
    public Dictionary<RoomSO.RoomType,float> toDictionary()
    {
        Dictionary<RoomSO.RoomType, float> newDictionary = new Dictionary<RoomSO.RoomType, float>();
        foreach (var item in newRoomDictionary)
        {
            newDictionary.Add(item.type, item.chance);
        }
        return newDictionary;
    }
}
[Serializable]
public class RoomDictionaryRoom
{
    [SerializeField] public RoomSO.RoomType type;
    [SerializeField] public float chance;
}