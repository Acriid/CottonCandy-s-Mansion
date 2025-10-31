using System.Collections.Generic;
using UnityEngine;

public class VariableTest : MonoBehaviour
{
    private List<int> ints = new List<int>();
    void Start()
    {
        AddtoInts(ints);
        foreach(int i in ints)
        {
            print(i);
        }
    }

    private void AddtoInts(List<int> intList)
    {
        for(int i = 0; i < 10; i ++)
        {
            intList.Add(i);
        }
    }
}
