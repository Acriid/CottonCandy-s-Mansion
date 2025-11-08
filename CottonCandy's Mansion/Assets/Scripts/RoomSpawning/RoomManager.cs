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
    public string LoadLable = "Room";
    private Dictionary<RoomSO.RoomType, List<RoomSO>> RoomDictionary = new Dictionary<RoomSO.RoomType, List<RoomSO>>();
    private List<RoomSO> SpecificRoomsList = new List<RoomSO>();
    private List<RoomSO> rooms = new List<RoomSO>();
    private List<GameObject> CurrentLevel = new List<GameObject>();
    private List<Bounds> CurrentBounds = new List<Bounds>();
    public Transform roomParent;
    async void Awake()
    {
        if (instance == null) { instance = this; }
        else { return; }

        await LoadAllRooms();

        for(int i = 0; i < 10; i ++)
        {
            SpawnLevel();
        }
    }

    private async Task LoadAllRooms()
    {
        //
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

        RoomSO roomToSpawn = GetRandomRoom();
        GameObject LevelObject;


        if (CurrentLevel.Count == 0)
        {
            LevelObject = Instantiate(roomToSpawn.MainRoom, Vector3.zero, Quaternion.identity, roomParent);
            CurrentLevel.Add(LevelObject);
            CurrentBounds.Add(new Bounds(roomToSpawn.MainRoom.transform.position, roomToSpawn.RoomSize));
            return;
        }

        GameObject CurrentRoom = CurrentLevel[CurrentLevel.Count - 1];
        Vector3 RoomPosition;
        Bounds spawnBounds;
        Quaternion RoomRotation;

        do
        {
            runs++;
            roomToSpawn = GetRandomRoom();
            RoomPosition = GetNewRoomPosition(roomToSpawn, CurrentRoom);
            spawnBounds = new Bounds(RoomPosition, roomToSpawn.RoomSize - Vector3.one);
            RoomRotation = GetRoomRotation(CurrentRoom);
            if(!TestIfCanSpawn(spawnBounds, CurrentBounds))
            {
                RemoveFromList(roomToSpawn);
            }
        } while (!TestIfCanSpawn(spawnBounds, CurrentBounds) && runs < 99);



        LevelObject = Instantiate(roomToSpawn.MainRoom, RoomPosition, RoomRotation, roomParent);
        CurrentLevel.Add(LevelObject);
        CurrentBounds.Add(spawnBounds);

    }

    private Vector3 GetNewRoomPosition(RoomSO roomToSpawn,GameObject CurrentRoom)
    {
        //Gets room and door
        Vector3 CurrentRoomDimensions = CurrentRoom.transform.position;
        Transform spawnDoor = CurrentRoom.transform.Find("Structure/Doors/Door_Next");
        //Mirrors forward
        Vector3 newForward = spawnDoor.forward * -1;
        //Gets new spawn
        Vector3 ResultVector = CompareVectors(CurrentRoomDimensions, newForward,0.0001f);
        Vector3 newDoorPosition = Vector3.Scale(Vector3.Scale(spawnDoor.position, newForward), newForward);
        Vector3 newSpawn = newDoorPosition + newForward / 2f + Vector3.Scale(roomToSpawn.RoomSize / 2f, newForward) +
        Vector3.Scale(CurrentRoomDimensions,ResultVector);
        return newSpawn;
        
    }
    private Quaternion GetRoomRotation(GameObject CurrentRoom)
    {
        //Gets room and door
        Transform spawnDoor = CurrentRoom.transform.Find("Structure/Doors/Door_Next");
        //Mirrors forward
        Vector3 newForward = spawnDoor.forward * -1;
        //Gets new Rotation
        Quaternion result = Quaternion.LookRotation(newForward);
        return result;
    }
    #endregion
    private RoomSO GetRandomRoom()
    {
        if(SpecificRoomsList.Count == 0)
        {
            SpecificRoomsList = new List<RoomSO>(GetRandomRoomList());
        }
        int UpperBoundry = SpecificRoomsList.Count - 1;
        if (UpperBoundry == -1)
        {
            SpecificRoomsList = new List<RoomSO>(GetRandomRoomList());
        }
        UpperBoundry = SpecificRoomsList.Count - 1;
        int RandomRoom = UnityEngine.Random.Range(0, UpperBoundry);
        RoomSO result = SpecificRoomsList[RandomRoom];
        return result;
    }
    private List<RoomSO> GetRandomRoomList()
    {
        return RoomDictionary[RoomSO.RoomType.Hallway];
    }
    private void RemoveFromList(RoomSO roomToRemove)
    {
        if (SpecificRoomsList.Count == 0) { return; }
        SpecificRoomsList.Remove(roomToRemove);
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