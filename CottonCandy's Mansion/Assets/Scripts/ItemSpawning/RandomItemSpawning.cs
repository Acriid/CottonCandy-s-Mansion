using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawning : MonoBehaviour
{
    public List<ItemSO> playerItemSO;
    public List<Bounds> itemBounds;
    public Transform parentTransform;
    public int Budget;
    public int CheckTimes;
    public float SpawnRadius;
    void Awake()
    {
        foreach(ItemSO itemSO in playerItemSO)
        {
            itemSO.InitializeSize();
        }
        SpawnItems();
    }
    private void SpawnItems()
    {
        while (Budget > 0 && playerItemSO.Count != 0)
        {
            int randomSpot = Random.Range(0, playerItemSO.Count);
            if (CheckIfEnoughBudget(Budget, playerItemSO[randomSpot].ItemCost))
            {
                bool validPoint = false;
                Vector3 randomPoint = Vector3.zero;
                Bounds testBounds = new Bounds(Vector3.zero, Vector3.zero);
                for (int i = 0; i < CheckTimes; i++)
                {
                    randomPoint = GetRandomPointinRange(SpawnRadius);
                    testBounds = new Bounds(randomPoint, playerItemSO[randomSpot].ItemSize);
                    if (CheckIfValidSpawn(testBounds)) { validPoint = true; break; }
                }

                if (validPoint)
                {
                    SpawnObject(randomPoint, playerItemSO[randomSpot].ItemObject);
                    Budget -= playerItemSO[randomSpot].ItemCost;
                    itemBounds.Add(testBounds);
                }
                else
                {
                    playerItemSO.RemoveAt(randomSpot);
                }
            }
            else
            {
                playerItemSO.RemoveAt(randomSpot);
            }

        }
    }
    private void SpawnObject(Vector3 point,GameObject spawnObject)
    {
        Instantiate(spawnObject, point, Quaternion.identity, parentTransform);
    }

    private bool CheckIfEnoughBudget(int currentBudget, int spawnCost)
    {
        return currentBudget >= spawnCost;
    }
    private Vector3 GetRandomPointinRange(float range)
    {
        float randomX = Random.Range(-range, range);
        float randomY = Random.Range(-range, range);
        float randomZ = Random.Range(-range, range);
        Vector3 randomSpot = new Vector3(randomX, randomY, randomZ);

        return randomSpot;
    }
    private bool CheckIfValidSpawn(Bounds spawnBounds)
    {
        bool result = true;
        foreach (Bounds bounds in itemBounds)
        {
            if (CompareBounds(bounds, spawnBounds))
            {
                result = false;
            }
        }
        return result;
    }
    private bool CompareBounds(Bounds staticBounds, Bounds compareBounds)
    {
        return staticBounds.Intersects(compareBounds);
    }
}
