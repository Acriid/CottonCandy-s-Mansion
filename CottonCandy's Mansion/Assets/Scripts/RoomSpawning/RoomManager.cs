using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;
    public Transform roomParent;
    private Dictionary<RoomSO.RoomType, List<RoomSO>> RoomDictionary = new Dictionary<RoomSO.RoomType, List<RoomSO>>();
    private List<RoomSO> SpecificRoomsList = new List<RoomSO>();
    private List<RoomSO> rooms = new List<RoomSO>();
    private List<GameObject> CurrentLevel = new List<GameObject>();
    private List<Bounds> CurrentBounds = new List<Bounds>();
    #region String Constants
    //Doors
    const string DoorString = "Structure/Doors/Door_Next";
    //HeightOffsets
    const string HeightString = "Structure/HeightOffset/Upper";
    //Room Loading
    const string LoadLable = "Room";
    #endregion
    async void Awake()
    {
        if (instance == null) { instance = this; }
        else { return; }

        await LoadAllRooms();

        for (int i = 0; i < 10; i++)
        {
            SpawnLevel();
        }
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
    private void SpawnLevel()
    {
        SpawnRoom();
    }
    private void SpawnRoom()
    {
        int runs = 0;

        RoomSO roomToSpawn = GetRandomRoom(SpecificRoomsList);
        GameObject LevelObject;

        //First Levels
        if (CurrentLevel.Count == 0)
        {
            LevelObject = Instantiate(roomToSpawn.MainRoom, Vector3.zero, Quaternion.identity, roomParent);
            CurrentLevel.Add(LevelObject);
            CurrentBounds.Add(new Bounds(roomToSpawn.MainRoom.transform.position, roomToSpawn.RoomSize));
            return;
        }
        //Sequencial levels
        GameObject CurrentRoom = CurrentLevel[CurrentLevel.Count - 1];
        Vector3 RoomPosition;
        Bounds spawnBounds;
        Quaternion RoomRotation;

        //Test if room does not spawn in another room if no room can spawn just spawn a room
        do
        {
            runs++;
            roomToSpawn = GetRandomRoom(SpecificRoomsList);
            RoomPosition = GetNewRoomPosition(roomToSpawn, CurrentRoom);
            spawnBounds = new Bounds(RoomPosition, roomToSpawn.RoomSize - Vector3.one);
            RoomRotation = GetRoomRotation(CurrentRoom);
            if (!TestIfCanSpawn(spawnBounds, CurrentBounds))
            {
                RemoveFromList(roomToSpawn,SpecificRoomsList);
            }

        } while (!TestIfCanSpawn(spawnBounds, CurrentBounds) && runs < 99);



        LevelObject = Instantiate(roomToSpawn.MainRoom, RoomPosition, RoomRotation, roomParent);
        CurrentLevel.Add(LevelObject);
        CurrentBounds.Add(spawnBounds);

    }

    private Vector3 GetNewRoomPosition(RoomSO roomToSpawn, GameObject CurrentRoom)
    {
        //Gets room and door
        Vector3 CurrentRoomDimensions = CurrentRoom.transform.position;
        Transform UpperOffset = CurrentRoom.transform.Find(HeightString);

        if (UpperOffset != null)
            CurrentRoomDimensions.y += UpperOffset.position.y;

        Transform spawnDoor = CurrentRoom.transform.Find(DoorString);

        //Mirrors forward
        Vector3 newForward = spawnDoor.forward * -1;

        //Gets new spawn
        Vector3 ResultVector = CompareVectors(CurrentRoomDimensions, newForward, 0.0001f);
        Vector3 newDoorPosition = Vector3.Scale(Vector3.Scale(spawnDoor.position, newForward), newForward);
        Vector3 newSpawn = newDoorPosition + newForward / 2f + Vector3.Scale(roomToSpawn.RoomSize / 2f, newForward) +
        Vector3.Scale(CurrentRoomDimensions, ResultVector);

        return newSpawn;

    }
    private Vector3 GetNewRoomPosition(RoomSO roomToSpawn, RoomSO CurrentRoom,Vector3 RoomPosition,Quaternion RoomRotation)
    {
        Transform spawnDoor = CurrentRoom.MainRoom.transform.Find(DoorString);
        spawnDoor.position += RoomPosition;
        Vector3 doorForward = spawnDoor.forward * -1;
        doorForward = RoomRotation * doorForward;

        Vector3 CurrentRoomDimensions = RoomPosition;
        Transform UpperOffset = CurrentRoom.MainRoom.transform.Find(HeightString);
        
        if (UpperOffset != null)
            CurrentRoomDimensions.y += UpperOffset.position.y;

        Vector3 ResultVector = CompareVectors(CurrentRoomDimensions, doorForward, 0.001f);
        Vector3 newDoorPosition = Vector3.Scale(Vector3.Scale(spawnDoor.position, doorForward), doorForward);
        Vector3 newSpawn = newDoorPosition + doorForward / 2f + Vector3.Scale(roomToSpawn.RoomSize / 2f, doorForward) +
        Vector3.Scale(CurrentRoomDimensions, ResultVector);

        return Vector3.zero;
    }
    private Quaternion GetRoomRotation(GameObject CurrentRoom)
    {
        //Gets room and door
        Transform spawnDoor = CurrentRoom.transform.Find(DoorString);
        //Mirrors forward
        Vector3 newForward = spawnDoor.forward * -1;
        //Gets new Rotation
        Quaternion result = Quaternion.LookRotation(newForward);
        return result;
    }
    private bool CheckIfCanContinue(RoomSO roomToCheck, GameObject CurrentRoom, List<RoomSO> RoomList, List<Bounds> BoundsList)
    {
        //Checks if the dungeon can continue if this room spawns.
        //If not do not spawn this room.
        //If can spawn this room
        //Checks by getting the current room spawn for this room (GetNewRoomPosition) 
        //Then gets a calculated room spawn for where the next room will spawn
        //If no next room can spawn then return false.
        //If a next room can spawn return true/
        Vector3 RoomPosition = GetNewRoomPosition(roomToCheck, CurrentRoom);
        Quaternion RoomRotation = GetRoomRotation(CurrentRoom);
        Bounds spawnBounds = new Bounds(RoomPosition, roomToCheck.RoomSize - Vector3.one);


        return true;
    }
    
    private Vector3 CalculatedRoomSpawn(RoomSO CurrentRoom , Vector3 RoomPosition , Quaternion RoomRotation , List<RoomSO> RoomList)
    {
        //Simulate a what if the currentRoom is here
        RoomSO randomRoom = GetRandomRoom(RoomList);
        if (randomRoom == null)
        {
            return Vector3.zero;
        }


        return Vector3.one;
    }
    #endregion

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