using UnityEngine;

public interface IInventory 
{
    public void PickUpItem(GameObject itemToPickUp);
    public void PickUpItem(GameObject objectPickingUp, GameObject itemToPickUp);
    public void PutDownItem(GameObject itemToPutDown);
    public void AddItemToInventory(GameObject itemToAdd);
    public void RemoveItemToInventory(GameObject itemToRemove);
    public void SwitchCurrentlyHeldItem(GameObject newHeldItem);
    public GameObject GetCurrentlyHeldItem();
}
