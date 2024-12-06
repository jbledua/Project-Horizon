using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;

public partial struct PlayerMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<PlayerData, PlayerInputData, LocalTransform>();
        state.RequireAnyForUpdate(state.GetEntityQuery(builder));
    }

    public void OnUpdate(ref SystemState state)
    {
        var job = new PlayerMovementJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}
public partial struct PlayerMovementJob : IJobEntity
{
    public float deltaTime;

    public void Execute(ref PlayerData player, in PlayerInputData input, ref LocalTransform transform)
    {
        // Always move forward
        float3 forward = math.forward(transform.Rotation);
        float3 movement = forward * player.speed * deltaTime;

        // Apply boost if active
        if (input.boost)
        {
            movement = forward * player.boostSpeed * deltaTime;
        }

        // Update position while maintaining flight height
        float3 newPosition = new float3(
            transform.Position.x + movement.x,
            player.flightHeight,
            transform.Position.z + movement.z
        );

        transform.Position = newPosition;

        // Handle rotation based on input
        if (!math.all(input.move == float2.zero))
        {
            // Calculate target rotation
            float3 targetDirection = math.normalize(new float3(input.move.x, 0f, input.move.y));
            quaternion targetRotation = quaternion.LookRotationSafe(targetDirection, math.up());

            // Smoothly rotate toward the target using rotationSpeed
            transform.Rotation = math.slerp(transform.Rotation, targetRotation, deltaTime * player.rotationSpeed);
        }
    }
}
