using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMechanic : MonoBehaviour
{
    //Player can currently hold only 2 items at a time
    //Items being held should be dynamic just in case in the future
    [SerializeField] private int _maxItems = 2;
    [SerializeField] private List<GameObject> _heldItems = new();
    [SerializeField] private GameObject _currentHeldItem = null;
    public void AddToInventory(GameObject ItemToAdd)
    {
        if(_heldItems.Count == _maxItems){return;}
        _heldItems.Add(ItemToAdd);
        _currentHeldItem = ItemToAdd;
    }
    public void RemoveFromInventory(GameObject ItemToRemove)
    {
        if(_heldItems.Count == 0) {return;}
        if(!_heldItems.Contains(ItemToRemove)){return;}
        _heldItems.Remove(ItemToRemove);
    }
    public void RemoveFromInventory(Item ItemToRemove)
    {
        RemoveFromInventory(ItemToRemove.gameObject);
    }
    public GameObject GetCurrentHeldItemGameObject()
    {
        return _currentHeldItem;
    }
    public Item GetCurrentHeldItem()
    {
        if(_currentHeldItem == null)
            return null;
        if(_currentHeldItem.TryGetComponent<Item>(out Item item))
            return item;
        else
            return null;
    }
    public void ChangeItemGravityDirections(Vector3 newDirection)
    {
        foreach(GameObject inventoryObject in _heldItems)
        {
            inventoryObject.GetComponent<Item>().ChangeGravityDirection(newDirection);
        }
    }
    public void ChangeItemGravityModifier(float newModifier)
    {
        foreach(GameObject inventoryObject in _heldItems)
        {
            inventoryObject.GetComponent<Item>().ChangeGravityModifier(newModifier);
        }
    }
    
}
