using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Unity.Mathematics;

public struct ClientMessageRpcCommand : IRpcCommand
{
    public FixedString64Bytes message;
}

public struct ShootMissileRpcCommand : IRpcCommand
{

}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class ClientSystem : SystemBase
{
    private PlayerMovementActions playerInput;



    protected override void OnCreate()
    {
        RequireForUpdate<NetworkId>();
        playerInput = new PlayerMovementActions();
        playerInput.Enable();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (request, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ServerMessageRpcCommand>>().WithEntityAccess())
        {
            Debug.Log(command.ValueRO.message);
            commandBuffer.DestroyEntity(entity);
        }

        if (playerInput.Player.Taunt.WasPressedThisFrame())
        {
            // Retrieve the player's position
            float3 playerPosition = float3.zero;
            //foreach (var (transform, player, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerData>, RefRO<GhostOwnerIsLocal>>())
            //{
            //    //float3 targetPosition = transform.ValueRO.Position;
            //    break; // Assuming only one local player entity
            //}

            // Format the message with the player's position
            string message = $"I am at {playerPosition.x:F2}, {playerPosition.y:F2}, {playerPosition.z:F2} come and get me!";

            // Send the message via RPC
            SendMessageRpc(message, ConnectionManager.clientWorld);
        }


        if (playerInput.Player.Shoot.WasPressedThisFrame())
        {
             
            ShootMissileRPC(ConnectionManager.clientWorld);
        }

        if (playerInput.Player.Respawn.WasPressedThisFrame())
        {
            RespawnPlayerRPC(ConnectionManager.clientWorld);
        }

        if (playerInput.Player.Pause.WasPressedThisFrame())
        {
            MainMenuUI.ShowPauseMenu();
        }


        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }

    public void SendMessageRpc(string text, World world)
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

    public void ShootMissileRPC(World world)
    {
        if (world == null || world.IsCreated == false)
        {
            return;
        }
        world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ShootMissileRpcCommand));
    }

    public void RespawnPlayerRPC(World world)
    {
        if (world == null || world.IsCreated == false)
        {
            return;
        }
        world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(RespawnPlayerRpcCommand));
    }

}