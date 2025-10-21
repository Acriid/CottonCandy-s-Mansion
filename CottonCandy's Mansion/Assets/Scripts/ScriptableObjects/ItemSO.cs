using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public String ItemName;
    public String ItemDescription;
    public Sprite ItemSprite;
    public int ItemCost;
    public GameObject ItemObject;
}
