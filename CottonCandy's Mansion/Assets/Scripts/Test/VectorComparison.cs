using UnityEngine;

public class VectorComparison : MonoBehaviour
{
    public Vector3 vector1;
    public Vector3 vector2;
    void Awake()
    {
        Debug.Log(CompareVectors(vector1, vector2));
    }
    private Vector3 CompareVectors(Vector3 a, Vector3 b)
    {
        return new Vector3(
            (!Mathf.Approximately(a.x, 0f) && Mathf.Approximately(b.x, 0f)) ? 1 : 0,
            (!Mathf.Approximately(a.y, 0f) && Mathf.Approximately(b.y, 0f)) ? 1 : 0,
            (!Mathf.Approximately(a.z, 0f) && Mathf.Approximately(b.z, 0f)) ? 1 : 0
        );
    }
}
