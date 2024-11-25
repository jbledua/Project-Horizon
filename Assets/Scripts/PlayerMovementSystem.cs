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
public partial struct PlayerMovementJob: IJobEntity
{
    public float deltaTime;

    public void Execute(PlayerData player, PlayerInputData input, ref LocalTransform transform)
    {
        float3 movement = new float3(input.move.x, 0, input.move.y) * player.speed * deltaTime;
        transform.Position = transform.Translate(movement).Position;
    }
}