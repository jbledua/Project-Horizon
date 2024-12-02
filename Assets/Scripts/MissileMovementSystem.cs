using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.NetCode;
using UnityEngine;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class MissileMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Create an EntityCommandBuffer to handle structural changes
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (missileData, transform, entity) in SystemAPI.Query<RefRW<MissileData>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            // Move the missile forward
            float3 forward = math.forward(transform.ValueRO.Rotation);
            transform.ValueRW.Position += forward * missileData.ValueRO.speed * deltaTime;

            // Decrease lifetime
            missileData.ValueRW.lifeTime -= deltaTime;

            // Queue the missile for destruction if its lifetime expires
            if (missileData.ValueRO.lifeTime <= 0)
            {
                commandBuffer.DestroyEntity(entity);
            }
        }

        // Play back the command buffer to execute the structural changes
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
