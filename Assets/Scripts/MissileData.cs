using Unity.Entities;

public struct MissileData : IComponentData
{
    public float speed;      // Speed of the missile
    public float lifeTime;   // Remaining lifetime of the missile

    public float colliderSize; // The size of the collidor
}
