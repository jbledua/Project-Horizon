using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct PlayerInputData : IInputComponentData
{
    public float2 move;
    public InputEvent taunt;
    public InputEvent shoot;
}
