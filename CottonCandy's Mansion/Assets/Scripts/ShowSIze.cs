using UnityEngine;

public class ShowSIze : MonoBehaviour
{
    public MeshRenderer gameObject1;
    public MeshRenderer gameObject2;
    void OnEnable()
    {
        if (gameObject1 != null && gameObject2 != null)
        {
            Debug.Log(gameObject1.bounds.Intersects(gameObject2.bounds));
        }
    }
}
