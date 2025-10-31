using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemRarityTest : MonoBehaviour
{
    //x = base, y = min , z = max
    //0 = commonluck , 1 = rareluck, 2 = legendaryluck
    public Vector3 CommonAttributes;
    public Vector3 RareAttributes;
    public Vector3 LegendaryAttributes;
    public List<bool> TypeOfLuck;
    private float specialChance;
    public int LuckToMax;
    private double CommonGrowth;
    private double RareGrowth;
    private double LegendaryGrowth;
    void OnEnable()
    {
        for (int i = 0; i < 1000; i++)
        {
            RandomChoice();
        }
        
    }
    private void RandomChoice()
    {
        string ItemRarity = "None";
        float FirstThreshold = CommonAttributes.x;
        float SecondThreshold = FirstThreshold + RareAttributes.x;
        float ThirdThreshold = SecondThreshold + LegendaryAttributes.x;
        float randomResult = UnityEngine.Random.Range(0f, 1f);
        if (randomResult >= 0 && randomResult <= FirstThreshold) { ItemRarity = "Common"; }
        else if (randomResult > FirstThreshold && randomResult <= SecondThreshold) { ItemRarity = "Rare"; }
        else if (randomResult > SecondThreshold && randomResult <= ThirdThreshold) { ItemRarity = "Legendary"; }
        Debug.Log(ItemRarity);
    }

    private void ChangeChance(float luck)
    {
        //1.1^n * a0 = 0.33
        //1.1^n = 0.33/0.1
        //n = log(1.1)(0.33/0.1) 
    }
    private void InitializeGrowth(List<bool> lucktype)
    {
        if (lucktype.Count != 3) { Debug.LogError("Luck is not good"); return; }
        
        if(lucktype[2])
        {
            LegendaryGrowth = Math.Pow(LegendaryAttributes.z / LegendaryAttributes.x, 1f / LuckToMax);
            RareGrowth = Math.Pow(RareAttributes.y / RareAttributes.x, 1f / LuckToMax);
            
        }
    }
}
