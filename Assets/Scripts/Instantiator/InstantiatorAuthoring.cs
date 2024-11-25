using UnityEngine;
using Unity.Entities;
public class InstantiatorAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int instanceCount;

    private class Baker : Baker<InstantiatorAuthoring>
    {
        public override void Bake(InstantiatorAuthoring authoring)
        {
            Entity entityPrefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);
            Instantiator instantiator = new(entityPrefab, authoring.instanceCount);
            AddComponent(GetEntity(TransformUsageFlags.None), instantiator);
        }
    }
}