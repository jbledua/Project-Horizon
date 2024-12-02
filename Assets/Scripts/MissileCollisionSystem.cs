using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial struct MissileCollisionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<MissileData, LocalTransform>();
        state.RequireAnyForUpdate(state.GetEntityQuery(builder));
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;

        NativeArray<Entity> entities = entityManager.GetAllEntities(Allocator.Temp);

        foreach (Entity entity in entities)
        {
            // Get all entities will MissileData Component
            if (entityManager.HasComponent<MissileData>(entity))
            {

                // Get the Transform of the missile
                RefRW<LocalToWorld> missileTransform = SystemAPI.GetComponentRW<LocalToWorld>(entity);
                RefRO<MissileData> missileDataComponent = SystemAPI.GetComponentRO<MissileData>(entity);


                PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

                physicsWorld.SphereCastAll(missileTransform.ValueRO.Position, missileDataComponent.ValueRO.colliderSize/2,
                    float3.zero,1, ref hits, CollisionFilter.Default);

                foreach (ColliderCastHit hit in hits)
                {
                    Debug.Log("Enemy Hit" + hit.Entity);
                    //entityManager.DestroyEntity(hit.Entity);
                }
            }

        }

        entities.Dispose();
    }
}
