using Unity.Entities;
using Unity.Mathematics;

public struct PlayerMovement : IComponentData
{
    public float MoveSpeed;    // Normal movement speed
    public float BoostSpeed;   // Boosted movement speed
    public bool IsBoosting;    // Whether boosting is active
    public float2 MoveInput;   // Input for movement
    public int PlayerId; // Unique ID for each player
}

