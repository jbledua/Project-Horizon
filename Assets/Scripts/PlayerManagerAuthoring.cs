using UnityEngine;
using Unity.Entities;

public class PlayerManagerAuthoring : MonoBehaviour
{
    public GameObject playerPrefab;

    public class Baker : Baker<PlayerManagerAuthoring>
    {
        public override void Bake(PlayerManagerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PlayerManager
            {
                PlayerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}
