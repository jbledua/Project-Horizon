using UnityEngine;
using Unity.Entities;

public class PlayerPrefabAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; // Assign the prefab in the Editor

    public class Baker : Baker<PlayerPrefabAuthoring>
    {
        public override void Bake(PlayerPrefabAuthoring authoring)
        {
            // Create an Entity from the prefab
            Entity prefabEntity = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic);

            // Add the PlayerPrefabHolder component to the entity
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerPrefabHolder
            {
                PlayerPrefab = prefabEntity
            });
        }
    }
}
