using UnityEngine;

public class ShowSIze : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log(GetComponent<MeshRenderer>().bounds);
    }
}
