using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string ItemName;
    public string ItemDescription;
    public Sprite ItemSprite;
    public ItemRarity rarity;
    public GameObject ItemObject;
    public Vector3 ItemSize;
    public float ItemGravity;
    public void InitializeSize()
    {
        ItemSize = ItemObject.GetComponent<MeshRenderer>().bounds.size;
    }
    void OnEnable()
    {
        InitializeSize();
    }

    public enum ItemRarity
    {
        Common,
        Rare,
        Legendary,
        None
    }

}
