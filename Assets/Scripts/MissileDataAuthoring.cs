using Unity.Entities;
using UnityEngine;

public class MissileDataAuthoring : MonoBehaviour
{
    public float speed = 80f;      // Default speed
    public float lifeTime = 5f;    // Default lifetime
    public float collidorSize = 1;

    public class MissileDataBaker : Baker<MissileDataAuthoring>
    {
        public override void Bake(MissileDataAuthoring authoring)
        {
            Entity missileAuthoring = GetEntity(TransformUsageFlags.None);

            AddComponent(missileAuthoring, new MissileData
            {
                speed = authoring.speed,
                lifeTime = authoring.lifeTime,
                colliderSize = authoring.collidorSize
            });
        }
    }
}
