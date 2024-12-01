using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

public struct ServerMessageRpcCommand : IRpcCommand
{
    public FixedString64Bytes message;
}

public struct InitializedClient : IComponentData
{
    
}

public struct RespawnPlayerRpcCommand : IRpcCommand
{
    
}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial class ServerSystem : SystemBase
{
    private ComponentLookup<NetworkId> _clients;

    protected override void OnCreate()
    {
        _clients = GetComponentLookup<NetworkId>(true);
    }
    protected override void OnUpdate()
    {
        _clients.Update(this);
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        // Handle Player Joining
        foreach (var (id, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClient>().WithEntityAccess())
        {
            commandBuffer.AddComponent<InitializedClient>(entity);
            PrefabsData prefabManager = SystemAPI.GetSingleton<PrefabsData>();
            if (prefabManager.player != null)
            {
                Entity player = commandBuffer.Instantiate(prefabManager.player);
                commandBuffer.SetComponent(player, new LocalTransform
                {
                    Position = new float3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10)),
                    Rotation = Quaternion.Euler(0f, 0f, 0f),
                    Scale = 0.1f
                });

                commandBuffer.SetComponent(player, new GhostOwner()
                {
                    NetworkId = id.ValueRO.Value
                });
                commandBuffer.AppendToBuffer(entity, new LinkedEntityGroup()
                {
                    Value = player
                });

                Debug.Log("Player " + id.ValueRO.Value + " joined");
            }

        } // End foreach Joining

        // Handle Player Respawn 
        foreach (var (request, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<RespawnPlayerRpcCommand>>().WithEntityAccess())
        {
           
            Debug.Log("Respaning player (Not Working Yet)");


            commandBuffer.DestroyEntity(entity);


            //commandBuffer.AddComponent<InitializedClient>(entity);
            //PrefabsData prefabManager = SystemAPI.GetSingleton<PrefabsData>();
            //if (prefabManager.player != null)
            //{
            //    commandBuffer.SetComponent(player, new LocalTransform
            //    {
            //        Position = new float3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10)),
            //        Rotation = Quaternion.Euler(0f, 0f, 0f),
            //        Scale = 0.1f
            //    });

            //    Debug.Log("Respaning player " + id.ValueRO.Value);
            //}

        } // End foreach Respawn

        // Handle Client Messages
        foreach (var (request, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ClientMessageRpcCommand>>().WithEntityAccess())
        {
            Debug.Log(command.ValueRO.message + " from client index " + request.ValueRO.SourceConnection.Index + " version " + request.ValueRO.SourceConnection.Version);
            commandBuffer.DestroyEntity(entity);
        } // End foreach Messages

        // Handle Shooting Requests
        foreach (var (request, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ShootMissileRpcCommand>>().WithEntityAccess())
        {
            PrefabsData prefabs;
            if (SystemAPI.TryGetSingleton<PrefabsData>(out prefabs) && prefabs.missile != null)
            {
                // Find the player entity and its transform
                foreach (var (playerTransform, playerData, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PlayerData>, RefRO<GhostOwnerIsLocal>>())
                {
                    // Instantiate the missile
                    Entity missile = commandBuffer.Instantiate(prefabs.missile);

                    // Calculate the spawn position
                    float3 forward = math.forward(playerTransform.ValueRO.Rotation);
                    float3 up = new float3(0, 1, 0); // World up vector
                    float3 right = math.normalize(math.cross(up, forward)); // Calculate right direction
                    float wingOffset = 2.0f; // Adjust for the wing position
                    float3 wingPositionOffset = playerData.ValueRW.activeWing ? right * wingOffset : -right * wingOffset;

                    float3 spawnPosition = playerTransform.ValueRO.Position +
                                           forward * 5f + // Adjust 5f for desired distance
                                           wingPositionOffset;

                    // Alternate the active wing
                    playerData.ValueRW.activeWing = !playerData.ValueRW.activeWing;

                    // Set the missile's transform
                    commandBuffer.SetComponent(missile, new LocalTransform()
                    {
                        Position = spawnPosition,
                        Rotation = playerTransform.ValueRO.Rotation, // Match player's rotation
                        Scale = 1f
                    });

                    // Assign ownership
                    var networkId = _clients[request.ValueRO.SourceConnection];
                    commandBuffer.SetComponent(missile, new GhostOwner()
                    {
                        NetworkId = networkId.Value
                    });

                    // Link the missile entity
                    commandBuffer.AppendToBuffer(request.ValueRO.SourceConnection, new LinkedEntityGroup()
                    {
                        Value = missile
                    });

                    // Destroy the request entity
                    commandBuffer.DestroyEntity(entity);

                    // Exit the loop after finding the player
                    break;
                }
            }
        } // End foreach Shooting


        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }

   

    public void SendMessageRpc(string text, World world, Entity target = default)
    {
        if (world == null || world.IsCreated == false)
        {
            return;
        }
        var entity = world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ServerMessageRpcCommand));
        world.EntityManager.SetComponentData(entity, new ServerMessageRpcCommand()
        {
            message = text
        });
        if (target != Entity.Null)
        {
            world.EntityManager.SetComponentData(entity, new SendRpcCommandRequest()
            {
                TargetConnection = target
            });
        }
    }

}