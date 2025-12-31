using System.Threading.Tasks;
using UnityEngine;

public class OfferTester : MonoBehaviour
{
    public CottonCandySO cottonCandySO;
    async void OnEnable()
    {
        await cottonCandySO.ReloadConditions();
        Offer offer = cottonCandySO.GetOffer();
        Debug.Log(offer.Benefit.name);
        Debug.Log(offer.Benefit.Type);
        Debug.Log(offer.Drawback.name);
        Debug.Log(offer.Drawback.Type);
        Debug.Log(offer.OfferType);
    }
}
