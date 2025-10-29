using UnityEngine;

public class TransformFindTest : MonoBehaviour
{
    public string TransformToFind;
    public Transform rootTransform;
    void Awake()
    {
        Debug.Log(rootTransform.Find(TransformToFind).forward * -1);
    }
}
