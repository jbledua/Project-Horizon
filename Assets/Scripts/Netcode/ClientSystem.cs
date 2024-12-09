using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

public struct ClientMessageRpcCommand : IRpcCommand
{
    public FixedString64Bytes message;
}

public struct ShootMissileRpcCommand : IRpcCommand
{
    public Entity PlayerEntity; // Reference to the shooting player
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class ClientSystem : SystemBase
{
   
    protected override void OnCreate()
    {
        RequireForUpdate<NetworkId>();

    }

    protected override void OnUpdate()
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (request, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ServerMessageRpcCommand>>().WithEntityAccess())
        {
            Debug.Log("Client: " + command.ValueRO.message);
;
            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();

        var localNetworkId = SystemAPI.GetSingleton<NetworkId>().Value; // Get local client's NetworkId

        Entities.ForEach((Entity entity, ref PlayerData playerData, in GhostOwner owner) =>
        {
            // Check if the player is owned by the local client
            if (owner.NetworkId == localNetworkId)
            {
                playerData.localPlayer = true; // Set as local player
            }
            else
            {
                playerData.localPlayer = false; // Ensure others are not marked as local
            }
        }).ScheduleParallel();
   
    }


     
    public static void SendMessageToServerRpc(string text, World world)
    {
        if (world == null || world.IsCreated == false)
        {
            return;
        }
        var entity = world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ClientMessageRpcCommand));
        world.EntityManager.SetComponentData(entity, new ClientMessageRpcCommand()
        {
            message = text
        });
    }

    public static void ShootMissileRPC(World world)
    {
        if (world == null || world.IsCreated == false)
        {
            return;
        }
        world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ShootMissileRpcCommand));
    }

    public static void RespawnPlayerRPC(World world)
    {
        if (world == null || world.IsCreated == false)
        {
            return;
        }
        world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(RespawnPlayerRpcCommand));
    }

}