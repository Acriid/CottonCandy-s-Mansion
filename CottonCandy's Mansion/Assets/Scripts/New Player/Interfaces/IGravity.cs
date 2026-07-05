using UnityEngine;

public interface IGravity
{
    public void Initialize();
    public void ChangeGravity(ref Vector3 originalDirection);
    public void SetGravityDirection(Vector3 newDirection);

}
