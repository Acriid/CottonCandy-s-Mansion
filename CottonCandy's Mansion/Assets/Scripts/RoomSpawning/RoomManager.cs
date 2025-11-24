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
    public static RoomManager instance;
    public Transform roomParent;
    private Dictionary<RoomSO.RoomType, List<RoomSO>> RoomDictionary = new();
    private List<RoomSO> SpecificRoomsList = new();
    private List<RoomSO> rooms = new();
    public List<Bounds> LevelBounds = new();
    public List<GameObject> Level = new();
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

        SpawnLevel(10);
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
        //Initial room
        Quaternion testRotation = Quaternion.Normalize(new Quaternion(1f, 0.5f, 0f, 0f));
        GameObject CurrentRoom = Instantiate(RoomtoSpawn.MainRoom, Vector3.zero, testRotation, roomParent);

        Quaternion RoomRotation = CurrentRoom.transform.rotation;
        Quaternion RoomRotation2 = GetRoomRotation(RoomtoSpawn,testRotation);

        // Simulate next room
        Vector3 ActualRoomPosition = GetNewRoomPosition(RoomtoSpawn2,CurrentRoom);
        Vector3 RoomPosition2 = GetNewRoomPosition(RoomtoSpawn2, RoomtoSpawn, Vector3.zero, testRotation);

        Instantiate(RoomtoSpawn2.MainRoom, RoomPosition2, RoomRotation2, roomParent);
    }
    private void TestBounds()
    {
        GameObject CurrentRoom = Instantiate(RoomtoSpawn.MainRoom, Vector3.zero, Quaternion.identity, roomParent);
        Vector3 RoomSize = GetNewRoomSize(RoomtoSpawn,Quaternion.identity);
        Bounds bounds = new(Vector3.zero,RoomSize);
        LevelBounds.Add(bounds);
        
        Quaternion RoomRotation = GetRoomRotation(RoomtoSpawn,Quaternion.identity);
        Vector3 RoomPosition = GetNewRoomPosition(RoomtoSpawn2,CurrentRoom);
        RoomSize = GetNewRoomSize(RoomtoSpawn2,RoomRotation);
        bounds = new(RoomPosition,RoomSize);
        LevelBounds.Add(bounds);

        SpawnRoom(RoomtoSpawn2,RoomPosition,RoomRotation);

        foreach(Bounds boundss in LevelBounds)
        {
            GameObject BoundsObject= Instantiate(TestBoundsObj,boundss.center,Quaternion.identity);
            BoundsObject.transform.localScale = boundss.size;
            BoundsObject.GetComponent<MeshRenderer>().material = TestObjMat;
        }
    }
    private void SpawnLevel(int LevelSize)
    {
        List<RoomSO> RoomList = new(GetRoomList(RoomSO.RoomType.Hallway));
        int InitialCount = RoomList.Count;

        RoomSO RandomRoom = GetRandomRoom(RoomList);
        Vector3 RoomPosition = Vector3.zero;
        Quaternion RoomRotation = new(1f, 0.5f, 0f, 0f);

        GameObject CurrentRoom = SpawnRoom(RandomRoom,RoomPosition,RoomRotation);

        Vector3 NewRoomSize = GetNewRoomSize(RandomRoom,RoomRotation);
        Bounds RoomBounds = new(RoomPosition,NewRoomSize);

        LevelBounds.Add(RoomBounds);
        Level.Add(CurrentRoom);

        int Count = 0;
        int FailSafe = 0;

        while(Count < LevelSize && FailSafe < 100 && RoomList.Count > 0)
        {
            RandomRoom = GetRandomRoom(RoomList);
            RoomPosition = GetNewRoomPosition(RandomRoom,CurrentRoom);
            RoomRotation = GetRoomRotation(CurrentRoom);
            NewRoomSize = GetNewRoomSize(RandomRoom,RoomRotation);
            RoomBounds = new(RoomPosition,NewRoomSize);
            
            if(TestIfCanSpawn(RoomBounds,LevelBounds))
            {
                if(RoomList.Count != InitialCount){RoomList = new(GetRoomList(RoomSO.RoomType.Hallway));}
                CurrentRoom = SpawnRoom(RandomRoom,RoomPosition,RoomRotation);

                LevelBounds.Add(RoomBounds);
                Level.Add(CurrentRoom);
                Count++;
            }
            else
            {
                RoomList.Remove(RandomRoom);
            }
            FailSafe++;
        }

        foreach(Bounds bounds in LevelBounds)
        {
            GameObject BoundsObject= Instantiate(TestBoundsObj,bounds.center,Quaternion.identity);
            BoundsObject.transform.localScale = bounds.size;
            BoundsObject.GetComponent<MeshRenderer>().material = TestObjMat;
        }
    }
    private GameObject SpawnRoom(RoomSO roomToSpawn, Vector3 Position, Quaternion Rotation)
    {
        return Instantiate(roomToSpawn.MainRoom,Position,Rotation,roomParent);
    }
    private bool TestIfRoomCanSpawn(RoomSO roomToCheck, Vector3 RoomPosition, Quaternion RoomRotation)
    {
        //Get RoomList
        List<RoomSO> TestList = new(GetRoomList(RoomSO.RoomType.Hallway));
        RoomSO TestRoom = GetRandomRoom(TestList);
        
        //Get Room Position/Rotation
        Quaternion NewRoomRotation = GetRoomRotation(roomToCheck,RoomRotation);
        Vector3 NewRoomPosition = GetNewRoomPosition(TestRoom,roomToCheck,RoomPosition,NewRoomRotation);
        Vector3 NewSize = GetNewRoomSize(TestRoom,NewRoomRotation);
        Bounds TestBounds = new(NewRoomPosition,NewSize);

        bool CanSpawn = TestIfCanSpawn(TestBounds,LevelBounds);

        while(!CanSpawn && TestList.Count > 0)
        {
            if(!TestList.Remove(TestRoom)) {Debug.Log("Oh Fuck");}
            TestRoom = GetRandomRoom(TestList);
            if(TestRoom == null){break;}
            //New Room Stuff
            NewRoomPosition = GetNewRoomPosition(TestRoom,roomToCheck,RoomPosition,NewRoomRotation);
            NewSize = GetNewRoomSize(TestRoom,NewRoomRotation);
            TestBounds = new(NewRoomPosition,NewSize);

            CanSpawn = TestIfCanSpawn(TestBounds,LevelBounds);
        }

        return CanSpawn;
    }
    private RoomSO GetNextRoom()
    {
        return null;
    }
    #endregion
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
       // if (upperOffset != null) heightOffset.y += upperOffset.localPosition.y;
        if (lowerOffset != null) heightOffset.y += Mathf.Abs(lowerOffset.localPosition.y);
        heightOffset = roomRotation * heightOffset;

        // --- Step 4: Room offset along spawn direction ---
        Vector3 halfRoomSize = roomToSpawn.RoomSize / 2f;
        Vector3 roomOffset = roomRotation * halfRoomSize;
        float forwardDot = Vector3.Dot(roomOffset, newForward);
        Vector3 roomScaleOffset = forwardDot * newForward;
        // --- Step 5: Compute final position ---
        Vector3 newRoomPosition = spawnDoor.position + roomScaleOffset + heightOffset + newForward /2f;

        return newRoomPosition;
    }
    private Vector3 GetNewRoomPosition(RoomSO roomToSpawn, RoomSO currentRoom, Vector3 roomPosition, Quaternion roomRotation)
    {
        // --- Step 1: Find key transforms in the prefab ---
        Transform spawnDoor = currentRoom.MainRoom.transform.Find(DoorString);
        Transform upperOffset = currentRoom.MainRoom.transform.Find(UpperOffsetString);
        Transform lowerOffset = roomToSpawn.MainRoom.transform.Find(LowerOffsetString);

        if (spawnDoor == null)
        {
            Debug.LogError($"Door '{DoorString}' not found in {currentRoom.MainRoom.name}");
            return roomPosition;
        }

        // --- Step 2: Normalize and apply rotation ---
        Quaternion normalizedRot = Quaternion.Normalize(roomRotation);

        // Door’s forward and up in world space
        Vector3 doorForward = normalizedRot * spawnDoor.forward;
        // --- Step 3: Mirror door direction (new room faces opposite) ---
        Vector3 newForward = -doorForward.normalized;

        // --- Step 4: Compute the door’s world-space position ---
        Vector3 worldSpawnDoorPos = roomPosition + (normalizedRot * spawnDoor.position);

        // --- Step 5: Handle height offset ---
        Vector3 heightOffset = Vector3.zero;
       // if (upperOffset != null)
        //    heightOffset.y += upperOffset.localPosition.y;
        if (lowerOffset != null)
            heightOffset.y += Mathf.Abs(lowerOffset.localPosition.y);

        // Rotate into world orientation
        heightOffset = normalizedRot * heightOffset;

        Quaternion NewRoomRotaion = Quaternion.Normalize(GetRoomRotation(currentRoom,normalizedRot));
        // --- Step 6: Offset by half the next room size along the new direction ---
        Vector3 halfRoomSize = roomToSpawn.RoomSize / 2f;
        Vector3 roomOffset = NewRoomRotaion * halfRoomSize;
        float forwardDot = Vector3.Dot(roomOffset, newForward);
        Vector3 roomScaleOffset = forwardDot * newForward;
        // --- Step 7: Combine everything ---
        Vector3 newRoomPosition = worldSpawnDoorPos + roomScaleOffset + heightOffset + newForward * 0.5f;
        return newRoomPosition;
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
    private Quaternion GetRoomRotation(RoomSO currentRoom, Quaternion roomRotation)
    {
        // --- Step 1: Find the door in the prefab ---
        Transform spawnDoor = currentRoom.MainRoom.transform.Find(DoorString);

        if (spawnDoor == null)
        {
            Debug.LogError($"Door '{DoorString}' not found in {currentRoom.MainRoom.name}");
            return roomRotation; // Fallback to current rotation
        }

        // --- Step 2: Convert door orientation from local to world space ---
        Vector3 worldForward = roomRotation * -spawnDoor.forward;
        Vector3 worldUp = roomRotation * spawnDoor.up;

        // --- Step 3: Mirror the forward direction (so next room faces opposite) ---

        // --- Step 4: Create new rotation using both forward and up directions ---
        Quaternion result = Quaternion.LookRotation(worldForward, worldUp);
        return result;
    }
    private Vector3 GetNewRoomSize(RoomSO room, Quaternion RoomRotation)
    {
        Vector3 SizeOffset = RoomRotation * Vector3.one;
        SizeOffset = new(Mathf.Abs(SizeOffset.x),Mathf.Abs(SizeOffset.y),Mathf.Abs(SizeOffset.z));

        Vector3 RotationResult = RoomRotation * room.RoomSize;
        RotationResult = new(Mathf.Abs(RotationResult.x),Mathf.Abs(RotationResult.y),Mathf.Abs(RotationResult.z));

        return RotationResult - SizeOffset;
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