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
    private float3 _cameraRoation = new float3(90f, 0f, 0f); // Fixed height and no rotation
    private float _lagSpeed = 2f; // Camera lag speed

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
            // Get the player's position
            float3 targetPosition = transform.ValueRO.Position;

            // Calculate the desired camera position with lag
            float3 desiredPosition = targetPosition + _cameraOffset;

            // Smoothly interpolate the camera's position for the lag effect
            float3 smoothedPosition = math.lerp(_mainCamera.position, desiredPosition, SystemAPI.Time.DeltaTime * _lagSpeed);

            // Apply the smoothed position to the camera
            _mainCamera.position = new Vector3(smoothedPosition.x, _cameraOffset.y, smoothedPosition.z);
            _mainCamera.transform.eulerAngles = new Vector3(_cameraRoation.x, _cameraRoation.y, _cameraRoation.z);
        }
    }
}
