using System.Collections.Generic;
using UnityEngine;

public class InventoryMechanic : MonoBehaviour
{
    //Player can currently hold only 2 items at a time
    //Items being held should be dynamic just in case in the future
    [SerializeField] private int _maxItems = 2;
    [SerializeField] private List<GameObject> _heldItems = new();
    public void AddToInventory(GameObject ItemToAdd)
    {
        if(_heldItems.Count == _maxItems){return;}
        _heldItems.Add(ItemToAdd);
    }
    public void RemoveFromInventory(GameObject ItemToRemove)
    {
        if(_heldItems.Count == 0) {return;}
        if(!_heldItems.Contains(ItemToRemove)){return;}
        _heldItems.Remove(ItemToRemove);
    }
    
}
