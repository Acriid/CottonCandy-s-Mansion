using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "CottonCandy", menuName = "CottonCandy/CottonCandy")]
public class CottonCandySO : ScriptableObject
{
    //Gives deals which have an upside and downside (options in deals are scriptable objects where there are numbers and types. Upside and 10)
    //Type of deal (bad,not that bad, good,pretty good,holy shit) are based on if you have more upsides than downsides. (something like a number)
    //Changes chances based on the actions done during your playthrough
    //Changes can range from giving better/worse deals changing the chances of specific items appearing.

    //List of all deals yes.
    //List of current upsides
    //List of current downsides

    private Dictionary<ConditionSO.ConditionType,int> _dummyConditionDictionary = new();
    [SerializeField] private Dictionary<ConditionSO.ConditionType,List<ConditionSO>> _conditionDictionary = new();

    private static readonly ConditionSO.ConditionType[] _allTypes =
    (ConditionSO.ConditionType[])Enum.GetValues(typeof(ConditionSO.ConditionType));

    private static readonly ConditionWeightComparison WeightComparer = new();

    const string LOADLABLE = "Conditions";

    private async void OnEnable()
    {
        await LoadConditions();
    }
    private async Task LoadConditions()
    {
        var handle = Addressables.LoadAssetsAsync<ConditionSO>(LOADLABLE, null);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load conditions.");
            Addressables.Release(handle);
            return;
        }

        // Count sizes
        var counts = new Dictionary<ConditionSO.ConditionType, int>();
        foreach (var c in handle.Result)
        {
            if (!counts.TryAdd(c.Type, 1))
                counts[c.Type]++;
        }

        // Pre-size lists
        foreach (var t in _allTypes)
        {
            _conditionDictionary[t] = new List<ConditionSO>(counts.TryGetValue(t, out int n) ? n : 0);
        }

        // Add directly
        foreach (var c in handle.Result)
        {
            _conditionDictionary[c.Type].Add(c);
        }

        // Sort once
        foreach (var t in _allTypes)
            _conditionDictionary[t].Sort(WeightComparer);

        
        Addressables.Release(handle);
    }

    public Offer GetOffer()
    {
        int benefitRandom = UnityEngine.Random.Range(0,3);
        int drawbackRandom = UnityEngine.Random.Range(4,
         _conditionDictionary[ConditionSO.ConditionType.Movement].Count);


        ConditionSO benefit = _conditionDictionary[ConditionSO.ConditionType.Movement][benefitRandom];
        ConditionSO drawback = _conditionDictionary[ConditionSO.ConditionType.Movement][drawbackRandom];

        int OfferWeight =  benefit.Weight + drawback.Weight;
        OfferType offerType = OfferType.Neutral;

        if(OfferWeight >= 10)
        {
            offerType = OfferType.Amazing;
        }
        else if (OfferWeight >= 5)
        {
            offerType = OfferType.Great;
        }
        else if (OfferWeight > 0)
        {
            offerType = OfferType.Good;
        }
        else if (OfferWeight <= -10)
        {
            offerType = OfferType.Horrid;
        }
        else if (OfferWeight <= -5)
        {
            offerType = OfferType.Terrible;
        }
        else if (OfferWeight < 0)
        {
            offerType = OfferType.Bad;
        }

        Offer result = new(benefit,drawback,offerType);
        return result;
    }


}

public struct Offer
{
    public ConditionSO Benefit;
    public ConditionSO Drawback;
    public OfferType OfferType;
    public Offer(ConditionSO benefit, ConditionSO drawBack, OfferType offerType)
    {
        Benefit = benefit;
        Drawback = drawBack;
        OfferType = offerType;
    }
}


public enum OfferType
{
    Horrid,
    Terrible,
    Bad,
    Neutral,
    Good,
    Great,
    Amazing
}