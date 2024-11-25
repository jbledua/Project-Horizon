using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.NetCode;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class CameraFollowSystem : SystemBase
{
    private Transform _mainCamera;
    private float3 _cameraOffset = new float3(0f, 10f, 0f); // Fixed height and no rotation
    private float _lagSpeed = 2f; // Camera lag speed

    protected override void OnCreate()
    {
        _mainCamera = Camera.main?.transform;

        if (_mainCamera == null)
        {
            Debug.LogWarning("No camera tagged as 'MainCamera' found. Please tag your camera correctly.");
        }
    }

    protected override void OnUpdate()
    {
        // Query entities with PlayerData and GhostOwnerIsLocal
        foreach (var (transform, player,_) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerData>, RefRO<GhostOwnerIsLocal>>())
        {
            // Get the player's position
            float3 targetPosition = transform.ValueRO.Position;

            // Calculate the desired camera position with lag
            float3 desiredPosition = targetPosition + _cameraOffset;

            // Smoothly interpolate the camera's position for the lag effect
            float3 smoothedPosition = math.lerp(_mainCamera.position, desiredPosition, SystemAPI.Time.DeltaTime * _lagSpeed);

            // Apply the smoothed position to the camera
            _mainCamera.position = new Vector3(smoothedPosition.x, _cameraOffset.y, smoothedPosition.z);
        }
    }
}
