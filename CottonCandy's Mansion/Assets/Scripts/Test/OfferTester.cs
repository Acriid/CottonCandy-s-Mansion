using UnityEngine;

public class OfferTester : MonoBehaviour
{
    public CottonCandySO cottonCandySO;
    void OnEnable()
    {
        Offer offer = cottonCandySO.GetOffer();
        Debug.Log(offer.Benefit.name);
        Debug.Log(offer.Drawback.name);
        Debug.Log(offer.OfferType);
    }
}
