using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string ItemName;
    public string ItemDescription;
    public Sprite ItemSprite;
    public int ItemCost;
    public GameObject ItemObject;
    public Vector3 ItemSize;
    public Bounds GetItemBounds()
    {
        return ItemObject.GetComponent<MeshRenderer>().bounds;
    }
    public void InitializeSize()
    {
        ItemSize = ItemObject.GetComponent<MeshRenderer>().bounds.size;
    }
}
