using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition", menuName = "Conditions/Condition")]
public class ConditionSO : ScriptableObject
{
    public ConditionType Type;
    public int Weight;
    public string Description;
    [SerializeField] private ConditionCommand _conditionCommand;


    public void Execute()
    {
        Debug.Log("Executed");
        //_conditionCommand.Execute();
    }



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
