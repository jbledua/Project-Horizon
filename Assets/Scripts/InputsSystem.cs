using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class InputsSystem : SystemBase
{
    private PlayerMovementActions _controls;

    protected override void OnCreate()
    {
       
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<PlayerInputData>();
        RequireForUpdate(GetEntityQuery(builder));

        _controls = new PlayerMovementActions();
        _controls.Enable();
        _controls.Player.Shoot.performed += OnPlayerShoot;
        _controls.Player.Taunt.performed += OnTauntShoot;
        _controls.Player.Pause.performed += OnPause;


    }

    protected override void OnDestroy()
    {
        _controls.Disable();
        _controls.Player.Shoot.performed -= OnPlayerShoot;
        _controls.Player.Taunt.performed -= OnTauntShoot;
        _controls.Player.Pause.performed -= OnPause;
    }

    private void OnPlayerShoot(InputAction.CallbackContext obj)
    { 
   
        ClientSystem.SendMessageRpc("Shoot event triggered", ConnectionManager.clientWorld);
    }


    private void OnTauntShoot(InputAction.CallbackContext obj)
    {
        // Query for the local player's entity
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        foreach (var playerEntity in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<GhostOwnerIsLocal>())
        {
            // Get the LocalTransform component for the local player
            var localTransform = playerEntity.ValueRO;

            // Extract the position
            Vector3 _location = new Vector3(localTransform.Position.x, localTransform.Position.y, localTransform.Position.z);

            // Send the message with the player's position
            ClientSystem.SendMessageRpc($"I am at {_location.x}, {_location.z} come and get me.", ConnectionManager.clientWorld);

            break; // Break after finding the first local player
        }
    }

    private void OnPause(InputAction.CallbackContext obj)
    {
        MainMenuUI.ShowPauseMenu();
    }

    protected override void OnUpdate()
    {
        Vector2 playerMove = _controls.Player.Move.ReadValue<Vector2>();

        foreach (RefRW<PlayerInputData> input in SystemAPI.Query<RefRW<PlayerInputData>>().WithAll<GhostOwnerIsLocal>())
        {
            input.ValueRW.move = playerMove;

        }

       
    }

}
