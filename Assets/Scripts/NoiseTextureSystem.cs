using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class NoiseTextureSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (noiseParams, entity) in SystemAPI.Query<RefRO<NoiseParameters>>().WithEntityAccess())
        {
            Debug.Log($"Processing entity: {entity} with noise parameters:");
            Debug.Log($"MapWidth: {noiseParams.ValueRO.MapWidth}, MapHeight: {noiseParams.ValueRO.MapHeight}");
            Debug.Log($"Seed: {noiseParams.ValueRO.Seed}, Scale: {noiseParams.ValueRO.Scale}");
            Debug.Log($"Octaves: {noiseParams.ValueRO.Octaves}, Persistence: {noiseParams.ValueRO.Persistence}, Lacunarity: {noiseParams.ValueRO.Lacunarity}");
            Debug.Log($"Offset: {noiseParams.ValueRO.Offset}, NormalizeMode: {noiseParams.ValueRO.NormalizeMode}");

            // Generate Noise Map
            float[,] noiseMap = Noise.GenerateNoiseMap(
                noiseParams.ValueRO.MapWidth,
                noiseParams.ValueRO.MapHeight,
                noiseParams.ValueRO.Seed,
                noiseParams.ValueRO.Scale,
                noiseParams.ValueRO.Octaves,
                noiseParams.ValueRO.Persistence,
                noiseParams.ValueRO.Lacunarity,
                new Vector2(noiseParams.ValueRO.Offset.x, noiseParams.ValueRO.Offset.y),
                (Noise.NormalizeMode)noiseParams.ValueRO.NormalizeMode
            );

            Debug.Log("Noise map generated successfully.");

            // Convert Noise Map to Texture
            Texture2D texture = new Texture2D(noiseParams.ValueRO.MapWidth, noiseParams.ValueRO.MapHeight);
            for (int y = 0; y < noiseParams.ValueRO.MapHeight; y++)
            {
                for (int x = 0; x < noiseParams.ValueRO.MapWidth; x++)
                {
                    float value = noiseMap[x, y];
                    Color color = new Color(value, value, value);
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            Debug.Log("Texture generated successfully.");

            // Create a new entity to hold the texture
            var textureEntity = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(textureEntity, new NoiseTexture
            {
                TextureEntity = textureEntity
            });
            Debug.Log($"Created texture entity: {textureEntity}");

            // Optionally: Assign the texture to a renderer
            if (EntityManager.HasComponent<Renderer>(entity))
            {
                var renderer = EntityManager.GetComponentObject<Renderer>(entity);
                if (renderer != null)
                {
                    Debug.Log("Renderer found. Applying texture to material.");
                    renderer.material.mainTexture = texture;
                }
                else
                {
                    Debug.LogWarning("Renderer component exists but is null.");
                }
            }
            else
            {
                Debug.LogWarning("No Renderer component found on entity.");
            }
        }

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
