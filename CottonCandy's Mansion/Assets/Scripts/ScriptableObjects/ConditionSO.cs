using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CottonCandy", menuName = "ScriptableObjects/CottonCandySO")]
public class ConditionSO : ScriptableObject
{
    public ConditionType Type;
    public float Weight;
    public string Description;
    public enum ConditionType
    {
        Movement,
        Vision,
        Item,
        Damage,
        Room,
        Offer,
        Enemy,
        CottonCandy,
        Luck,

    }
}

public class ConditionWeightComparison : IComparer<ConditionSO>
{
    public int Compare(ConditionSO x, ConditionSO y)
    {
        if(x == null || y == null) return 0;

        int result = x.Weight.CompareTo(y.Weight);
        return -result;
    }
}
