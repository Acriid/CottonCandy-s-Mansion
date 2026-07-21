using System;
using UnityEngine;

public class Item : MonoBehaviour, IItem
{
    [SerializeField] protected ItemSO _itemSettings;
    public virtual string GetItemDescription()
    {
        return _itemSettings.ItemDescription;
    }

    public virtual string GetItemName()
    {
        return _itemSettings.ItemName;
    }

    public virtual void PickUpItem(GameObject objectPickingUp)
    {
        Debug.Log($"Picked up {_itemSettings.name}");
    }

    public virtual void PutDownItem(GameObject objectPuttingDown)
    {
        Debug.Log($"Put down {_itemSettings.name}");
    }

    public virtual void SetItemDescription(string newDescription)
    {
        _itemSettings.ItemDescription = newDescription;
    }

    public virtual void SetItemName(string newName)
    {
        _itemSettings.ItemName = newName;
    }

    public virtual void UseItem(GameObject objectUsingItem)
    {
        Debug.Log($"Used {_itemSettings.name}");
    }
}
