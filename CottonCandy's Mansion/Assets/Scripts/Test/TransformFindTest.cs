using UnityEngine;

public class TransformFindTest : MonoBehaviour
{
    public string TransformToFind;
    public Transform rootTransform;
    void Awake()
    {
        rootTransform.Find(TransformToFind).gameObject.SetActive(false);
    }
}
