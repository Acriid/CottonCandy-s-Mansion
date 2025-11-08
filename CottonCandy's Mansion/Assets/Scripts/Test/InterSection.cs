using UnityEngine;

public class InterSection : MonoBehaviour
{
    public GameObject gameObject1;
    public GameObject gameObject2;
    void Awake()
    {
        Bounds bounds1 = gameObject1.GetComponent<MeshRenderer>().bounds;
        Bounds bounds2 = gameObject2.GetComponent<MeshRenderer>().bounds;
        Debug.Log(bounds1.Intersects(bounds2));
    }
}
