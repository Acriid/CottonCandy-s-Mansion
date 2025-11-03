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