using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PlayerMovementAuthoring : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float boostSpeed = 10f;

    public class Baker : Baker<PlayerMovementAuthoring>
    {
        public override void Bake(PlayerMovementAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerMovement
            {
                MoveSpeed = authoring.moveSpeed,
                BoostSpeed = authoring.boostSpeed,
                IsBoosting = false,
                MoveInput = float2.zero
            });
        }
    }
}
