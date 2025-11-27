using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance {get; private set;}

    void Awake()
    {
        if(instance != null) return;
        instance = this;
    }
}
