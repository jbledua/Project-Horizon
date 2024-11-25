using UnityEngine;
using Unity.Entities;
public struct Instantiator : IComponentData
{
    public readonly Entity entityPrefab;
    public readonly int instanceCount;
    public bool instantiated;

    public Instantiator(Entity entityPrefab, int instanceCount)
    {
        this.entityPrefab = entityPrefab;
        this.instanceCount = instanceCount;
        this.instantiated = false;
    }
}