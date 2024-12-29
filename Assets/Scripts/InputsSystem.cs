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
        //ClientSystem.SendMessageToServerRpc("Respawn event triggered", ConnectionManager.clientWorld);
        ClientSystem.RespawnPlayerRPC( ConnectionManager.clientWorld);

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
        Vector2 playerMove = Vector2.zero;

        // Read movement from keyboard/controller
        Vector2 controllerMove = _controls.Player.Move.ReadValue<Vector2>();

        // Read touch input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            // Normalize touch position to range [-1, 1] assuming the touch input represents the movement
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // Convert touch position to normalized coordinates (0 to 1)
            Vector2 normalizedTouch = new Vector2(touchPosition.x / screenWidth, touchPosition.y / screenHeight);

            // Adjust to match the range expected for movement (-1 to 1)
            normalizedTouch = (normalizedTouch - new Vector2(0.5f, 0.5f)) * 2;

            playerMove = normalizedTouch; // Assign normalized touch as the movement vector
        }

        // Combine inputs (use the controller/keyboard input if touch is not active)
        if (playerMove == Vector2.zero)
        {
            playerMove = controllerMove;
        }

        bool isBoosting = _controls.Player.Boost.IsPressed(); // Check if boost is pressed

        foreach (RefRW<PlayerInputData> input in SystemAPI.Query<RefRW<PlayerInputData>>().WithAll<GhostOwnerIsLocal>())
        {
            input.ValueRW.move = playerMove;
            input.ValueRW.boost = isBoosting; // Set boost input state
        }
    }


}
