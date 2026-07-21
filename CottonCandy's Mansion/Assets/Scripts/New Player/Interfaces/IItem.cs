using UnityEngine;

public interface IItem 
{
    public void PickUpItem(GameObject objectPickingUp);
    public void PutDownItem(GameObject objectPuttingDown);
    public void UseItem(GameObject objectUsingItem);
    public string GetItemName();
    public string GetItemDescription();
    public void SetItemName(string newName);
    public void SetItemDescription(string newDescription);
}
