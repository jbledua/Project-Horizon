using Unity.Entities;

public struct MissileData : IComponentData
{
    public float Speed;      // Speed of the missile
    public float LifeTime;   // Remaining lifetime of the missile
}
