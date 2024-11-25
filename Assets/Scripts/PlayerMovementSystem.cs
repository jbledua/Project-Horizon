using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;

//[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<PlayerData, PlayerInputData, LocalTransform>();
        state.RequireAnyForUpdate(state.GetEntityQuery(builder));
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new PlayerMovementJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}

//[BurstCompile]
public partial struct PlayerMovementJob : IJobEntity
{
    public float deltaTime;

    public void Execute(PlayerData player, PlayerInputData input, ref LocalTransform transform)
    {
        // Calculate movement vector
        float3 movement = new float3(input.move.x, 0, input.move.y) * player.speed * deltaTime;

        // Update position
        transform.Position = transform.Translate(movement).Position;

        // If there's input, calculate the forward direction and update rotation
        if (!math.all(movement == float3.zero))
        {
            float3 forward = math.normalize(movement); // Normalize the movement vector

            // Adjust the forward direction by rotating -90 degrees on the X-axis
            quaternion baseRotation = quaternion.Euler(math.radians(-90f), 0f, 0f); // -90 degrees on X-axis
            quaternion targetRotation = quaternion.LookRotationSafe(forward, math.up()); // Calculate target rotation

            // Combine the target rotation with the base adjustment
            quaternion finalRotation = math.mul(targetRotation, baseRotation);

            // Smoothly rotate towards the target
            transform.Rotation = math.slerp(transform.Rotation, finalRotation, deltaTime * 10f);
        }
    }
}

