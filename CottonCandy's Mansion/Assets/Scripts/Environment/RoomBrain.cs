using System.Collections.Generic;
using UnityEngine;

public class RoomBrain : MonoBehaviour
{
    [SerializeField] private List<GameObject> _roomScriptObjects;
    void OnEnable()
    {
        foreach(var spawnListner in GetComponentsInChildren<IRooms>())
        {
            spawnListner.Initialize();
        }
    }
}
