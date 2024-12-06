using UnityEngine;
using UnityEngine.UIElements;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField]
    private UIDocument _uiDocument;
    private Label _xLabel;
    private Label _zLabel;

    private EntityManager _entityManager;
    private EntityQuery _playerQuery;


    void Start()
    {
        // Get the UIDocument and labels
        if (_uiDocument != null)
        {
            var root = _uiDocument.rootVisualElement;
            _xLabel = root.Q<Label>("XCoordinateLabel");
            _zLabel = root.Q<Label>("ZCoordinateLabel");
        }

        // Initialize EntityManager and create a query for the player entity
        _entityManager = ConnectionManager.clientWorld.EntityManager;
        _playerQuery = _entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<PlayerData>(),
            ComponentType.ReadOnly<LocalTransform>());
    }

    void Update()
    {
        if (_xLabel != null && _zLabel != null && _playerQuery.CalculateEntityCount() > 0)
        {
            // Get the player entity
            var playerEntity = _playerQuery.GetSingletonEntity();

            // Retrieve the LocalTransform component
            var localTransform = _entityManager.GetComponentData<LocalTransform>(playerEntity);

            // Update the UI labels with the player's position
            _xLabel.text = $"X: {localTransform.Position.x:F2}";
            _zLabel.text = $"Z: {localTransform.Position.z:F2}";
        }
    }
}
