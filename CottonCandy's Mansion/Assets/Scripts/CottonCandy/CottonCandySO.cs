using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private readonly Dictionary<ConditionSO.ConditionType,int> _dummyConditionIndexDictionary = new();

    [SerializeField] private Dictionary<ConditionSO.ConditionType,List<ConditionSO>> _conditionDictionary = new();

    private static readonly ConditionSO.ConditionType[] _allTypes =
    (ConditionSO.ConditionType[])Enum.GetValues(typeof(ConditionSO.ConditionType));

    private static readonly ConditionWeightComparison WeightComparer = new();

    const string LOADLABLE = "Conditions";

    private async void OnEnable()
    {
        await LoadConditions();
    }
    public async Task ReloadConditions()
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
        {
            _conditionDictionary[t].Sort(WeightComparer);
            

            var list = _conditionDictionary[t];
            int pivot = list.FindIndex(c => c.Weight == 0);
            _dummyConditionIndexDictionary[t] = pivot;
        }

        
        Addressables.Release(handle);
    }
    public void ShowOffer(Offer offerToShow)
    {   
        
    }
    public Offer GetOffer()
    {
        ConditionSO.ConditionType conditionType = GetRandomType();

        int pivit = _dummyConditionIndexDictionary[conditionType];
        int benefitRandom = UnityEngine.Random.Range(0,pivit - 1);

        ConditionSO benefit = _conditionDictionary[conditionType][benefitRandom];

        conditionType = GetRandomType();
        pivit = _dummyConditionIndexDictionary[conditionType];

        int drawbackRandom = UnityEngine.Random.Range(pivit + 1,_conditionDictionary[conditionType].Count);

        ConditionSO drawback = _conditionDictionary[conditionType][drawbackRandom];

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

    //Gets a random type of conditiontype
    private ConditionSO.ConditionType GetRandomType()
    {
        int maxRandom = _allTypes.GetLength(0) - 1;

        int randomIndex = UnityEngine.Random.Range(0,maxRandom);

        return _allTypes[randomIndex];
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