using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CottonCandy", menuName = "ScriptableObjects/CottonCandySO")]
public class CottonCandySO : ScriptableObject
{
    [Header("Rarity Chances")]
    public float CommonChance;
    public float RareChance;
    public float LegendaryChance;
}