using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Rendering;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    [Header("Lists")]
    public List<ItemSO> items;
    private Dictionary<ItemSO.ItemRarity, List<ItemSO>> itemDictionary = new Dictionary<ItemSO.ItemRarity, List<ItemSO>>();
    [Header("RarityAttributes")]
    [SerializeField] RarityDictionary rarityDictionary;
    private Dictionary<ItemSO.ItemRarity, float> RarityChances;
    [SerializeField] public int ItemCost;
    void Awake()
    {
        foreach(ItemSO item in items)
        {
            item.InitializeSize();
        }
        if (instance == null) { instance = this; InitializeLists(); }
        RarityChances = rarityDictionary.toDictionary();
        UpdateRarityChances();
        Debug.Log(GetRandomItem().name);
        
    }

    private void InitializeLists()
    {
        itemDictionary = new Dictionary<ItemSO.ItemRarity, List<ItemSO>>();
        InitializeRarity();
        foreach (ItemSO itemSO in items)
        {
            itemDictionary[itemSO.rarity].Add(itemSO);
        }
    }
    private void InitializeRarity()
    {
        foreach(ItemSO.ItemRarity rarity in Enum.GetValues(typeof(ItemSO.ItemRarity)))
        {
            itemDictionary.Add(rarity, new List<ItemSO>());
        }
    }
    private void ShowDictionary()
    {
        foreach(var item in itemDictionary)
        {
            foreach (ItemSO listItem in item.Value)
            {
                Debug.Log(listItem);
            }
        }
    }
    private void UpdateRarityChances()
    {
        
        float CurrentPercentage = 0f;
        foreach (var item in RarityChances)
        {
            if (!(item.Value < 0))
            {
               CurrentPercentage += item.Value;
            }          
        }

        if (!Mathf.Approximately(CurrentPercentage, 1f))
        {
            Dictionary<ItemSO.ItemRarity, float> newRarityChances = new Dictionary<ItemSO.ItemRarity, float>();
            foreach (var item in RarityChances)
            {
                float percentage = item.Value / CurrentPercentage;
                if(percentage < 0) { percentage = 0f; }
                newRarityChances.Add(item.Key, percentage);
            }
            RarityChances = newRarityChances;
        }
    }
    private ItemSO.ItemRarity GetRandomRarity()
    {
        Dictionary<float, ItemSO.ItemRarity> thresholds = new Dictionary<float, ItemSO.ItemRarity>();
        float cumulativeThreshold = 0f;
        foreach (var item in RarityChances)
        {
            if (!Mathf.Approximately(item.Value, 0f))
            {
                cumulativeThreshold += item.Value;
                thresholds.Add(cumulativeThreshold,item.Key);
            }
        }
        float random = UnityEngine.Random.Range(0f, 1f);
        foreach (var item in thresholds)
        {
            if (random < item.Key)
            {
                return item.Value;
            }
        }
        return ItemSO.ItemRarity.None;
    }
    public void ChangeRarity(ItemSO.ItemRarity key, float newValue)
    {
        if (RarityChances.ContainsKey(key)) { RarityChances[key] = newValue; }
        UpdateRarityChances();
    }

    public ItemSO GetRandomItem()
    {
        ItemSO.ItemRarity rarity = GetRandomRarity();
        int randomBoundry = itemDictionary[rarity].Count;
        int randomItem = UnityEngine.Random.Range(0, randomBoundry);
        return itemDictionary[rarity][randomItem];
    }
    public bool CheckIfCanSpawn()
    {
        bool result = false;
        foreach (var item in RarityChances)
        {
            if (item.Value > 0)
            {
                result = true;
            }
        }
        return result;
    }
}


[Serializable]
public class RarityDictionary
{
    [SerializeField] RarityDictionaryItem[] newDictionaryItems;
    public Dictionary<ItemSO.ItemRarity, float> toDictionary()
    {
        Dictionary<ItemSO.ItemRarity, float> newDictionary = new Dictionary<ItemSO.ItemRarity, float>();
        foreach (var item in newDictionaryItems)
        {
            newDictionary.Add(item.rarity, item.chance);
        }
        return newDictionary;
    }
}
[Serializable]
public class RarityDictionaryItem
{
    [SerializeField] public ItemSO.ItemRarity rarity;
    [SerializeField] public float chance;
}
