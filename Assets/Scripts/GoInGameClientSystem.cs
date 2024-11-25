using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct GoInGameCommand: IRpcCommand
{

}


[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct GoInGameClientSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAny<NetworkId>();
        builder.WithNone<NetworkStreamInGame>();
        state.RequireAnyForUpdate(state.GetEntityQuery(builder));
    }

    public void OnUpdate(ref SystemState state)
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (id, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>().WithEntityAccess())
        {
            commandBuffer.AddComponent<NetworkStreamInGame>(entity);
            var request = commandBuffer.CreateEntity();
            commandBuffer.AddComponent<GoInGameCommand>(request);
            commandBuffer.AddComponent<SendRpcCommandRequest>(request);
        }


        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}