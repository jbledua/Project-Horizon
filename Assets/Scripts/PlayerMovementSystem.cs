using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct PlayerMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerMovement, localTransform) in SystemAPI.Query<RefRO<PlayerMovement>, RefRW<LocalTransform>>())
        {
            float currentSpeed = playerMovement.ValueRO.IsBoosting
                ? playerMovement.ValueRO.BoostSpeed
                : playerMovement.ValueRO.MoveSpeed;

            float3 movement = new float3(
                playerMovement.ValueRO.MoveInput.x,
                0,
                playerMovement.ValueRO.MoveInput.y
            ) * currentSpeed * SystemAPI.Time.DeltaTime;

            // Update position
            localTransform.ValueRW.Position += movement;
        }
    }
}
