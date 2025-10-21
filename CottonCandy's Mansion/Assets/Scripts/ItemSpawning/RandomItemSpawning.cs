using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawning : MonoBehaviour
{
    public List<ItemSO> playerItemSO;
    public Transform parentTransform;
    public int Budget;
    void Awake()
    {
        while(Budget > 0 && playerItemSO.Count != 0)
        {
            int randomSpot = Random.Range(0, playerItemSO.Count);
            if (CheckIfEnoughBudget(Budget, playerItemSO[randomSpot].ItemCost))
            {
                SpawnInSphere(5f, playerItemSO[randomSpot].ItemObject);
                Budget -= playerItemSO[randomSpot].ItemCost;
            }
            else
            {
                playerItemSO.RemoveAt(randomSpot);
            }

        }
    }

    private void SpawnInSphere(float radius,GameObject spawnObject)
    {
        float randomX = Random.Range(-radius, radius);
        float randomY = Random.Range(-radius, radius);
        float randomZ = Random.Range(-radius, radius);
        Vector3 randomSpot = new Vector3(randomX, randomY, randomZ);

        Instantiate(spawnObject, randomSpot, Quaternion.identity, parentTransform);
    }

    private bool CheckIfEnoughBudget(int currentBudget, int spawnCost)
    {
        return currentBudget >= spawnCost;
    }
}
