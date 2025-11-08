using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;
    public string LoadLable = "Room";
    private Dictionary<RoomSO.RoomType, List<RoomSO>> RoomDictionary = new Dictionary<RoomSO.RoomType, List<RoomSO>>();
    private List<RoomSO> rooms = new List<RoomSO>();
    private List<GameObject> CurrentLevel = new List<GameObject>();
    public Transform roomParent;
    async void Awake()
    {
        if (instance == null) { instance = this; }
        else { return; }

        await LoadAllRooms();

        for(int i = 0; i < 3; i ++)
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
            Debug.Log("Loaded Rooms.");
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
        SpawnRoom(GetRandomRoom());
    }
    private void SpawnRoom(RoomSO roomToSpawn)
    {
        GameObject LevelObject;
        if (CurrentLevel.Count == 0)
        {
            LevelObject = Instantiate(roomToSpawn.MainRoom, Vector3.zero, Quaternion.identity, roomParent);
            CurrentLevel.Add(LevelObject);
            return;
        }

        GameObject CurrentRoom = CurrentLevel[CurrentLevel.Count - 1];
        Debug.Log(CurrentRoom);
        Vector3 RoomPosition = GetNewRoomPosition(roomToSpawn,CurrentRoom);
        Quaternion RoomRotation = GetRoomRotation(CurrentRoom);
        LevelObject = Instantiate(roomToSpawn.MainRoom, RoomPosition, RoomRotation, roomParent);
        CurrentLevel.Add(LevelObject);

    }

    private Vector3 GetNewRoomPosition(RoomSO roomToSpawn,GameObject CurrentRoom)
    {
        //Gets room and door
        Transform spawnDoor = CurrentRoom.transform.Find("Structure/Doors/Door_Next");
        //Mirrors forward
        Vector3 newForward = spawnDoor.forward * -1;
        //Gets new spawn
        Vector3 newDoorPosition = Vector3.Scale(Vector3.Scale(spawnDoor.position, newForward),newForward);
        Vector3 newSpawn = newDoorPosition + newForward / 2f + Vector3.Scale(roomToSpawn.RoomSize / 2f, newForward);        
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
        int UpperBoundry = RoomDictionary[RoomSO.RoomType.Hallway].Count;
        int RandomRoom = UnityEngine.Random.Range(0, UpperBoundry);
        RoomSO result = RoomDictionary[RoomSO.RoomType.Hallway][RandomRoom];
        return result;
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