using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;
    public string LoadLable = "Rooms";
    private Dictionary<RoomSO.RoomType, List<RoomSO>> itemDictionary = new Dictionary<RoomSO.RoomType, List<RoomSO>>();
    private List<RoomSO> rooms = new List<RoomSO>();
    private List<RoomSO> CurrentLevel = new List<RoomSO>();
    async void Awake()
    {
        if (instance == null) { instance = this; }
        else { return; }

        await LoadAllRooms();
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
            itemDictionary.Add(roomType, new List<RoomSO>());
        }
        foreach (RoomSO room in rooms)
        {
            itemDictionary[room.Type].Add(room);
        }

    }

    private void SpawnRoom(RoomSO roomToSpawn)
    {
        if (CurrentLevel.Count == 0)
        {

        }
    }

    private Vector3 GetNewRoomPosition(RoomSO roomToSpawn)
    {
        RoomSO CurrentRoom = CurrentLevel[CurrentLevel.Count - 1];
        Transform spawnDoor = CurrentRoom.MainRoom.transform.Find("Doors/Door_Next");
        Vector3 newForward = spawnDoor.forward * -1;
        Vector3 newSpawn = Vector3.Scale(spawnDoor.position, newForward) + newForward / 2f + Vector3.Scale(roomToSpawn.RoomSize / 2f, newForward);
        return newSpawn;
    }
    private Vector3 CorrectRotation(RoomSO roomToSpawn)
    {
        
        return Vector3.zero;
    }
    private RoomSO GetRandomRoom()
    {
        return null;
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