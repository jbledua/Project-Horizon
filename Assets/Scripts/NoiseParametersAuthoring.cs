using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class NoiseParametersAuthoring : MonoBehaviour
{
    public int MapWidth = 256;
    public int MapHeight = 256;
    public int Seed = 42;
    public float Scale = 50f;
    public int Octaves = 4;
    public float Persistence = 0.5f;
    public float Lacunarity = 2f;
    public Vector2 Offset;
    public Noise.NormalizeMode NormalizeMode;

    class Baker : Baker<NoiseParametersAuthoring>
    {
        public override void Bake(NoiseParametersAuthoring authoring)
        {

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NoiseParameters
            {
                MapWidth = authoring.MapWidth,
                MapHeight = authoring.MapHeight,
                Seed = authoring.Seed,
                Scale = authoring.Scale,
                Octaves = authoring.Octaves,
                Persistence = authoring.Persistence,
                Lacunarity = authoring.Lacunarity,
                Offset = new float2(authoring.Offset.x, authoring.Offset.y),
                NormalizeMode = (int)authoring.NormalizeMode
            });

            /*
            AddComponent(new NoiseParameters
            {
                MapWidth = authoring.MapWidth,
                MapHeight = authoring.MapHeight,
                Seed = authoring.Seed,
                Scale = authoring.Scale,
                Octaves = authoring.Octaves,
                Persistence = authoring.Persistence,
                Lacunarity = authoring.Lacunarity,
                Offset = new float2(authoring.Offset.x, authoring.Offset.y),
                NormalizeMode = (int)authoring.NormalizeMode
            });
            //*/
        }
    }
}
