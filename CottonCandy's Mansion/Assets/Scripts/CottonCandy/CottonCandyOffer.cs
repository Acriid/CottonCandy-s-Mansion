using UnityEngine;
using TMPro;

public class CottonCandyOffer : MonoBehaviour
{
    [SerializeField] private TMP_Text _benifitText;
    [SerializeField] private TMP_Text _drawbackText;
    private Offer offer;
    public void ChangeBenefitText(string newText)
    {
        _benifitText.text = newText;
    }
    public void ChangeDrawbackText(string newText)
    {
        _drawbackText.text = newText;
    }
    public void InitializeOffer(Offer offerToShow)
    {
        _benifitText.text = offerToShow.Benefit.Description;
        _drawbackText.text = offerToShow.Drawback.Description;
        offer = offerToShow;
    }
    public void ChangeBenefit(ConditionSO newBenefit)
    {
        offer.Benefit = newBenefit;
        ChangeBenefitText(offer.Benefit.Description);
    }
    public void ChangeDrawback(ConditionSO newDrawback)
    {
        offer.Drawback = newDrawback;
        ChangeBenefitText(offer.Drawback.Description);
    }
    public void AcceptCondition()
    {
        offer.Benefit.Execute();
        offer.Drawback.Execute();
        CottonCandyManager.Instance.HideOffer();
    }
    public void DeclineCondition()
    {
        CottonCandyManager.Instance.HideOffer();
    }
}
