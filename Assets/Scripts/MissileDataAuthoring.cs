using Unity.Entities;
using UnityEngine;

public class MissileDataAuthoring : MonoBehaviour
{
    public float Speed = 10f;      // Default speed
    public float LifeTime = 5f;    // Default lifetime

    class Baker : Baker<MissileDataAuthoring>
    {
        public override void Bake(MissileDataAuthoring authoring)
        {
            AddComponent(new MissileData
            {
                Speed = authoring.Speed,
                LifeTime = authoring.LifeTime
            });
        }
    }
}
