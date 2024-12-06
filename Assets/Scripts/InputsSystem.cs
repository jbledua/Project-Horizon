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

        _controls.Player.Pause.performed += OnPause;
        _controls.Player.Respawn.performed += OnRespawn;

        _controls.Player.Taunt.performed += OnTaunt;
        _controls.Player.Shoot.performed += OnShoot;
        _controls.Player.Boost.performed += OnBoost;


    }

    protected override void OnDestroy()
    {
        _controls.Player.Pause.performed -= OnPause;
        _controls.Player.Respawn.performed -= OnRespawn;

        _controls.Player.Taunt.performed -= OnTaunt;
        _controls.Player.Shoot.performed -= OnShoot;
        _controls.Player.Boost.performed -= OnBoost;
    }

    private void OnShoot(InputAction.CallbackContext obj)
    {
        ClientSystem.SendMessageToServerRpc("Shoot event triggered", ConnectionManager.clientWorld);
    }

    private void OnBoost(InputAction.CallbackContext obj)
    {
        //ClientSystem.SendMessageToServerRpc("Boost event triggered", ConnectionManager.clientWorld);
    }

    private void OnRespawn(InputAction.CallbackContext obj)
    {
        ClientSystem.SendMessageToServerRpc("Respawn event triggered", ConnectionManager.clientWorld);
    }

    private void OnTaunt(InputAction.CallbackContext obj)
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
            ClientSystem.SendMessageToServerRpc($"I am at {_location.x:F2}, {_location.z:F2} come and get me.", ConnectionManager.clientWorld);

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
        bool isBoosting = _controls.Player.Boost.IsPressed(); // Check if boost is pressed

        foreach (RefRW<PlayerInputData> input in SystemAPI.Query<RefRW<PlayerInputData>>().WithAll<GhostOwnerIsLocal>())
        {
            input.ValueRW.move = playerMove;
            input.ValueRW.boost = isBoosting; // Set boost input state
        }



    }

}
