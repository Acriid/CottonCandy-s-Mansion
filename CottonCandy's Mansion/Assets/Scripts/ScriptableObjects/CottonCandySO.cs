using System;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private List<OfferSO> _allDeals;
}