using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct NoiseParameters : IComponentData
{
    public int MapWidth;
    public int MapHeight;
    public int Seed;
    public float Scale;
    public int Octaves;
    public float Persistence;
    public float Lacunarity;
    public float2 Offset;
    public int NormalizeMode; // 0 = Local, 1 = Global
}

public struct NoiseTexture : IComponentData
{
    public Entity TextureEntity; // Reference to the texture entity
}
