using UnityEngine;
using Unity.Entities;

public class ChunckDataAuthoring : MonoBehaviour
{
    public float Width;
    public float Length;

    // Baker class to convert this MonoBehaviour to an ECS component
    public class Baker : Baker<ChunckDataAuthoring>
    {
        public override void Bake(ChunckDataAuthoring authoring)
        {
 //           AddComponent(new ChunckData
 //           {
 //              Width = authoring.Width,
 //              Length = authoring.Length
 //           });
        }
    }
}
