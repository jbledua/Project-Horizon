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
        float3 newPosition = new float3(
            transform.Position.x + movement.x,
            player.flightHeight,
            transform.Position.z + movement.z
        );

        transform.Position = newPosition;
        //player.Position = newPosition; // Update position in PlayerData

        //Debug.Log($"PlayerData.Position: {player.Position}");


        //Debug.Log("Position: "+ player.Position);

        // If there's input, calculate the forward direction and update rotation
        if (!math.all(movement == float3.zero))
        {
            float3 forward = math.normalize(movement);
            quaternion targetRotation = quaternion.LookRotationSafe(forward, math.up());
            transform.Rotation = math.slerp(transform.Rotation, targetRotation, deltaTime * 10f);
        }
    }
}

