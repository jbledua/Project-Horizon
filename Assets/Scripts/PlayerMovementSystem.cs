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
        // Calculate movement vector (X, Z plane only)
        float3 movement = new float3(input.move.x, 0f, input.move.y) * player.speed * deltaTime;

        // Update position (maintaining flight height on Y-axis)
        transform.Position = new float3(
            transform.Position.x + movement.x,
            player.flightHeight, // Always set to flight height
            transform.Position.z + movement.z
        );

        // If there's input, calculate the forward direction and update rotation
        if (!math.all(movement == float3.zero))
        {
            float3 forward = math.normalize(movement); // Normalize the movement vector

            // Adjust the forward direction by rotating -90 degrees on the X-axis
            quaternion baseRotation = quaternion.Euler(math.radians(0f), 0f, 0f);
            quaternion targetRotation = quaternion.LookRotationSafe(forward, math.up()); // Calculate target rotation

            // Combine the target rotation with the base adjustment
            quaternion finalRotation = math.mul(targetRotation, baseRotation);

            // Smoothly rotate towards the target
            transform.Rotation = math.slerp(transform.Rotation, finalRotation, deltaTime * 10f);
        }
    }
}
