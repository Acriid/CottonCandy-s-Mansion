using UnityEngine;
using System;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;
    private Dictionary<RoomSO.RoomType, List<RoomSO>> itemDictionary = new Dictionary<RoomSO.RoomType, List<RoomSO>>();
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