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
        foreach (var (request, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ClientMessageRpcCommand>>().WithEntityAccess())
        {
            Debug.Log(command.ValueRO.message + " from client index " + request.ValueRO.SourceConnection.Index + " version " + request.ValueRO.SourceConnection.Version);
            commandBuffer.DestroyEntity(entity);
        }

        // Handle Shooting Requests
        foreach (var (request, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<SpawnUnitRpcCommand>>().WithEntityAccess())
        {
            PrefabsData prefabs;
            if (SystemAPI.TryGetSingleton<PrefabsData>(out prefabs) && prefabs.missile != null)
            {
                Entity missile = commandBuffer.Instantiate(prefabs.missile);
                commandBuffer.SetComponent(missile, new LocalTransform()
                {
                    Position = new float3(UnityEngine.Random.Range(-10f, 10f), 0, UnityEngine.Random.Range(-10f, 10f)),
                    Rotation = quaternion.identity,
                    Scale = 0.25f
                });

                var networkId = _clients[request.ValueRO.SourceConnection];
                commandBuffer.SetComponent(missile, new GhostOwner()
                {
                    NetworkId = networkId.Value
                });

                // Link 
                commandBuffer.AppendToBuffer(request.ValueRO.SourceConnection, new LinkedEntityGroup()
                {
                    Value = missile
                });
                commandBuffer.DestroyEntity(entity);
            }
        }

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
                    Position = new float3(UnityEngine.Random.Range(-10,10),0, UnityEngine.Random.Range(-10, 10)),
                    //Position = new float3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10)),
                    //Rotation = quaternion.identity,
                    Rotation = Quaternion.Euler(-90f, 0f, 0f),
                Scale = 0.01f
                });

                commandBuffer.SetComponent(player, new GhostOwner()
                {
                    NetworkId = id.ValueRO.Value
                });
                commandBuffer.AppendToBuffer(entity, new LinkedEntityGroup() 
                { 
                    Value = player
                });

                // 
                Debug.Log("New Player joined");
            }
        }
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