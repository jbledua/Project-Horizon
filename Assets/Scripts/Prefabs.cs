using UnityEngine;
using Unity.Entities;

public class Prefabs : MonoBehaviour
{
    public GameObject prefab = null;

   
}

public struct PrefabsData: IComponentData
{
    public Entity prefab;
}

public class PrefabBacker : Baker<Prefabs>
{
    public override void Bake(Prefabs authoring)
    {
        if (authoring.prefab != null)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PrefabsData
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic)
            }) ;
        }
    }
}
