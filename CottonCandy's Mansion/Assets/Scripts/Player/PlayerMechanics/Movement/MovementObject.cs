using UnityEngine;

public class MovementObject
{
    public Vector2 MovementDirection;
    public Vector3 GravityDirection;
    public Camera MovementCamera;
    public bool OnSlope;
    public Vector3 SlopeMoveDirection;
    public Rigidbody ObjectRigidbody;
    public float SpeedOffset;
    public float GravityOffset;
    public bool Grounded;
}
