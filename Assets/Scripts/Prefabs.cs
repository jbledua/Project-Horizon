using UnityEngine;
using Unity.Entities;

public class Prefabs : MonoBehaviour
{
    public GameObject unit = null;
    public GameObject player = null;

   
}

public struct PrefabsData: IComponentData
{
    public Entity unit;
    public Entity player;

}

public class PrefabBacker : Baker<Prefabs>
{
    public override void Bake(Prefabs authoring)
    {
        Entity unitPrefab = default;
        Entity playerPrefab = default;

        if (authoring.unit != null)
        {
            unitPrefab = GetEntity(authoring.unit, TransformUsageFlags.Dynamic);
        }

        if (authoring.player != null)
        {
            playerPrefab = GetEntity(authoring.player, TransformUsageFlags.Dynamic);
        }

        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PrefabsData
        {
            unit = unitPrefab,
            player = playerPrefab
        });
    }
}
