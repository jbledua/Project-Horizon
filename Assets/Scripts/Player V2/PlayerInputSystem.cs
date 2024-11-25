using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public partial struct PlayerInputSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // Process all PlayerMovement components
        foreach (var playerMovement in SystemAPI.Query<RefRW<PlayerMovement>>())
        {
            var keyboard = Keyboard.current;

            // Check for movement input (WASD or Arrow keys)
            float2 moveInput = new float2
            (
                (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
                (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0)
            );

            if (math.lengthsq(moveInput) > 0)
            {
                // Normalize input to maintain consistent speed
                playerMovement.ValueRW.MoveInput = math.normalize(moveInput);
            }
            else
            {
                // Reset movement input when no keys are pressed
                playerMovement.ValueRW.MoveInput = float2.zero;
            }

            // Capture boost input (Shift key)
            playerMovement.ValueRW.IsBoosting = keyboard.shiftKey.isPressed;
        }
    }
}
