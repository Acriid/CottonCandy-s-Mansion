using UnityEngine;

public interface IHotbar
{
    public void PickUpItem(GameObject itemToPickUp);
    public void PutDownItem(GameObject itemToPutDown);
    public GameObject GetCurrentlyHeldItem();
    public GameObject GetOffHandItem();
    public void SwitchMainHandAndOffHand();
}
