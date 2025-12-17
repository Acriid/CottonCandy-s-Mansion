using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "CottonCandy", menuName = "ScriptableObjects/CottonCandySO")]
public class CottonCandySO : ScriptableObject
{
    //Gives deals which have an upside and downside (options in deals are scriptable objects where there are numbers and types. Upside and 10)
    //Type of deal (bad,not that bad, good,pretty good,holy shit) are based on if you have more upsides than downsides. (something like a number)
    //Changes chances based on the actions done during your playthrough
    //Changes can range from giving better/worse deals changing the chances of specific items appearing.

    //List of all deals yes.
    //List of current upsides
    //List of current downsides

    [SerializeField] private List<ConditionSO> _allConditions;
    [SerializeField] private Dictionary<ConditionSO.ConditionType,List<ConditionSO>> _conditionDictionary = new();


    const string LOADLABLE = "Offers";

    async void OnEnable()
    {
        await LoadConditions();
    }
    private async Task LoadConditions()
    {
        AsyncOperationHandle<IList<ConditionSO>> _handle = Addressables.LoadAssetsAsync<ConditionSO>(LOADLABLE, null);

        await _handle.Task;

        if (_handle.Status == AsyncOperationStatus.Succeeded)
        {
            _allConditions.AddRange(_handle.Result);
        }
        else
        {
            Debug.LogError("Failed to load rooms.");
        }

        Addressables.Release(_handle);

        foreach(ConditionSO.ConditionType offerType in Enum.GetValues(typeof(ConditionSO.ConditionType)))
        {
            _conditionDictionary.Add(offerType,new());
        }

        foreach (ConditionSO offerSO in _allConditions)
        {
            _conditionDictionary[offerSO.Type].Add(offerSO);
        }

        foreach(ConditionSO.ConditionType offerType in Enum.GetValues(typeof(ConditionSO.ConditionType)))
        {
            _conditionDictionary[offerType].Sort(new ConditionWeightComparison());
        }


    }
}