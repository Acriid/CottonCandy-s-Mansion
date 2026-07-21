using System.Collections.Generic;
using UnityEngine;

public class HotBar : IHotbar
{
    private GameObject _currentlyHeldItem;
    private GameObject _offHandItem;

    public void PickUpItem(GameObject itemToPickUp)
    {
        if(_currentlyHeldItem == null)
        {
            _currentlyHeldItem = itemToPickUp;
            return;
        }
        else if(_offHandItem == null)
        {
            _offHandItem = itemToPickUp;
            return;
        }

        PutDownItem(_currentlyHeldItem);
        _currentlyHeldItem = itemToPickUp;

    }
    public void PutDownItem(GameObject itemToPutDown)
    {
        if(_currentlyHeldItem == itemToPutDown)
        {
            _currentlyHeldItem = null;
        }
        else if(_offHandItem == itemToPutDown)
        {
            _offHandItem = null;
        }
    }
    public GameObject GetCurrentlyHeldItem()
    {
        return _currentlyHeldItem;
    }

    public GameObject GetOffHandItem()
    {
        return _offHandItem;
    }

    public void SwitchMainHandAndOffHand()
    {
        (_offHandItem, _currentlyHeldItem) = (_currentlyHeldItem, _offHandItem);
    }
}
