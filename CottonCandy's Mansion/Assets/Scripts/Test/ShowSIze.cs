using UnityEngine;

public class ShowSIze : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log(GetComponent<MeshRenderer>().bounds.center);
        Debug.Log(GetComponent<MeshRenderer>().bounds.size);
    }
}
