using UnityEngine;
using Unity.Entities;

public class Prefabs : MonoBehaviour
{
    public GameObject missile = null;
    public GameObject player = null;

   
}

public struct PrefabsData: IComponentData
{
    public Entity missile;
    public Entity player;

}

public class PrefabBacker : Baker<Prefabs>
{
    public override void Bake(Prefabs authoring)
    {
        Entity missilePrefab = default;
        Entity playerPrefab = default;

        if (authoring.missile != null)
        {
            missilePrefab = GetEntity(authoring.missile, TransformUsageFlags.Dynamic);
        }

        if (authoring.player != null)
        {
            playerPrefab = GetEntity(authoring.player, TransformUsageFlags.Dynamic);
        }

        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PrefabsData
        {
            missile = missilePrefab,
            player = playerPrefab
        });
    }
}
