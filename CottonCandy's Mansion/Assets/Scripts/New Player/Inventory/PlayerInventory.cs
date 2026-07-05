using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IInventory
{
    private GameObject _currentlyHeldItem;
    private List<GameObject> _currentInventory;

    public void PickUpItem(GameObject itemToPickUp)
    {
        throw new System.NotImplementedException();
    }

    public void PickUpItem(GameObject objectPickingUp, GameObject itemToPickUp)
    {
        throw new System.NotImplementedException();
    }

    public void PutDownItem(GameObject itemToPutDown)
    {
        throw new System.NotImplementedException();
    }

    public void SwitchCurrentlyHeldItem(GameObject newHeldItem)
    {
        _currentlyHeldItem = newHeldItem;
    }
    public GameObject GetCurrentlyHeldItem()
    {
        return _currentlyHeldItem;
    }

    public void AddItemToInventory(GameObject itemToAdd)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveItemToInventory(GameObject itemToRemove)
    {
        throw new System.NotImplementedException();
    }
}
