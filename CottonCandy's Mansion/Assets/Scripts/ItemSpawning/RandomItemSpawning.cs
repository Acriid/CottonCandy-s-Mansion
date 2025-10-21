using UnityEngine;

public class RandomItemSpawning : MonoBehaviour
{
    public ItemSO playerItemSO;

    void Awake()
    {
        Instantiate(playerItemSO.ItemObject);
    }
}
