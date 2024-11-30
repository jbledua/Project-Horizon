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
    private float3 _cameraRotation = new float3(90f, 0f, 0f); // Fixed rotation

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        // Check for an existing camera
        _mainCamera = Camera.main?.transform;

        if (_mainCamera == null)
        {
            // Instantiate a new camera if none exists
            GameObject cameraGO = new GameObject("MainCamera");
            Camera camera = cameraGO.AddComponent<Camera>();
            camera.tag = "MainCamera"; // Tag as MainCamera for reference if needed
            _mainCamera = camera.transform;

            // Set camera properties (e.g., clear flags, perspective)
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.orthographic = false;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 1000f;

            Debug.Log("No camera found. Instantiated a new MainCamera.");
        }
    }

    protected override void OnUpdate()
    {
        // Ensure the camera is instantiated
        if (_mainCamera == null)
        {
            Debug.LogError("MainCamera is missing. Unable to update camera position.");
            return;
        }

        // Query entities with PlayerData and GhostOwnerIsLocal
        foreach (var (transform, player, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerData>, RefRO<GhostOwnerIsLocal>>())
        {
            // Get the player's position and camera settings
            float3 targetPosition = transform.ValueRO.Position;
            float3 cameraOffset = player.ValueRO.cameraOffset;
            float lagSpeed = player.ValueRO.cameraLagSpeed;

            // Calculate the desired camera position with offset
            float3 desiredPosition = targetPosition + cameraOffset;

            // Smoothly interpolate the camera's position for the lag effect
            float3 smoothedPosition = math.lerp(_mainCamera.position, desiredPosition, SystemAPI.Time.DeltaTime * lagSpeed);

            // Apply the smoothed position to the camera
            _mainCamera.position = new Vector3(smoothedPosition.x, smoothedPosition.y, smoothedPosition.z);
            _mainCamera.transform.eulerAngles = new Vector3(_cameraRotation.x, _cameraRotation.y, _cameraRotation.z);
        }
    }
}