using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawning : MonoBehaviour
{
    public List<GameObject> restrictedBoundsObjects;
    public List<Bounds> restrictedBounds;
    public Transform spawnTransform;
    public int totalBudget;
    public int TotalCheckTimes;
    public float SpawnRadius;
    public GameObject SpawnBoundsObject;
    private Bounds SpawnBounds;
    void OnEnable()
    {
        SpawnBounds = SpawnBoundsObject.GetComponent<MeshRenderer>().bounds;

        foreach (GameObject boundsObject in restrictedBoundsObjects)
        {
            restrictedBounds.Add(boundsObject.GetComponent<MeshRenderer>().bounds);
        }

        SpawnItems(totalBudget, TotalCheckTimes, spawnTransform, restrictedBounds);
    }
    private void SpawnItems(int Budget, int CheckTimes, Transform parentTransform, List<Bounds> bounds)
    {
        int RunTimes = 0;
        while (Budget > 0 && RunTimes <= CheckTimes)
        {
            RunTimes++;
            ItemSO itemToSpawn = ItemManager.instance.GetRandomItem();
            int CheckRunTimes = 0;
            Vector3 spawnPoint = Vector3.zero;
            Bounds itemBounds;
            do
            {
                CheckRunTimes++;
                spawnPoint = GetRandomPointInBound(SpawnBounds);
                itemBounds = new Bounds(spawnPoint, itemToSpawn.ItemSize);
            } while (!CheckIfValidSpawn(itemBounds, bounds) && CheckRunTimes <= CheckTimes);

            if(CheckIfValidSpawn(itemBounds, bounds))
            {
                SpawnObject(spawnPoint,itemToSpawn.ItemObject, parentTransform);
                Budget -= ItemManager.instance.ItemCost;
                bounds.Add(itemBounds);
                RunTimes = 0;
            }
            

        }
    }
    private void SpawnObject(Vector3 point, GameObject spawnObject, Transform parentTransform)
    {
        Instantiate(spawnObject, point, Quaternion.identity, parentTransform);
    }

    private bool CheckIfEnoughBudget(int currentBudget, int spawnCost)
    {
        return currentBudget >= spawnCost;
    }

    private Vector3 GetRandomPointinRange(float range, Transform parentTransform)
    {
        float randomX = parentTransform.localPosition.x + Random.Range(-range, range);
        float randomY = parentTransform.localPosition.y + Random.Range(-range, range);
        float randomZ = parentTransform.localPosition.z + Random.Range(-range, range);
        Vector3 randomSpot = new Vector3(randomX, randomY, randomZ);

        return randomSpot;
    }

    private Vector3 GetRandomPointInBound(Bounds bounds)
    {

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        Vector3 randomSpot = new Vector3(randomX, randomY, randomZ);

        return randomSpot;
    }

    private bool CheckIfValidSpawn(Bounds spawnBounds, List<Bounds> staticBounds)
    {
        bool result = true;
        foreach (Bounds bounds in staticBounds)
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
