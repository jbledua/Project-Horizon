using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 5f;

    public Vector3 cameraOffset = new Vector3(0f, 30f, -10f); // Default offset
    public float cameraLagSpeed = 2f; // Default lag speed
    public float flightHeight = 10f; // Default flight height for top-down jet
    public bool activeWing;    // Tracks the currently active wing (true = right, false = left)

}

public struct PlayerData : IComponentData
{
    public float speed;
    public float rotationSpeed;

    public float3 cameraOffset;
    public float cameraLagSpeed;
    public float flightHeight; // Player's flight height
    public bool activeWing;    // Tracks the currently active wing (true = right, false = left)
}

public class PlayerBaker : Baker<Player>
{
    public override void Bake(Player authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PlayerData
        {
            speed = authoring.speed,
            rotationSpeed = authoring.rotationSpeed,
            cameraOffset = authoring.cameraOffset,
            cameraLagSpeed = authoring.cameraLagSpeed,
            flightHeight = authoring.flightHeight,
            activeWing = authoring.activeWing,
        }); 
        AddComponent<PlayerInputData>(entity);
    }
}
