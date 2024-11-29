using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    private UIDocument _document;

    private Button hostButton;
    private Button joinButton;
    private Button privateButton;
    private Button exitButton;

    private ConnectionManager connectionManager; // Reference to the ConnectionManager

    [System.Obsolete]
    private void OnEnable()
    {
        // Get the UI Document
        _document = GetComponent<UIDocument>();
        if (_document == null)
        {
            Debug.LogError("UIDocument not found. Ensure a UIDocument is attached to this GameObject.");
            return;
        }

        // Find buttons by their name
        hostButton = _document.rootVisualElement.Q<Button>("hostButton");
        joinButton = _document.rootVisualElement.Q<Button>("joinButton");
        privateButton = _document.rootVisualElement.Q<Button>("privateButton");
        exitButton = _document.rootVisualElement.Q<Button>("exitButton");

        if (hostButton == null || joinButton == null || privateButton == null || exitButton == null)
        {
            Debug.LogError("One or more buttons are missing in the UI. Check their names in the UI Builder.");
            return;
        }

        // Attach callbacks to buttons
        hostButton.clicked += OnHostPressed;
        joinButton.clicked += OnJoinPressed;
        privateButton.clicked += OnPrivatePressed;
        exitButton.clicked += OnExitPressed;

        // Get the ConnectionManager reference (assuming it's in the same scene)
        connectionManager = FindObjectOfType<ConnectionManager>();
        if (connectionManager == null)
        {
            Debug.LogError("ConnectionManager not found in the scene. Ensure it's added to the scene.");
        }
    }

    private void OnDisable()
    {
        // Unregister callbacks only if the buttons are not null
        if (hostButton != null) hostButton.clicked -= OnHostPressed;
        if (joinButton != null) joinButton.clicked -= OnJoinPressed;
        if (privateButton != null) privateButton.clicked -= OnPrivatePressed;
        if (exitButton != null) exitButton.clicked -= OnExitPressed;
    }

    private void OnHostPressed()
    {
        Debug.Log("Host Button Pressed");
        // Your logic for hosting a game
    }

    private void OnJoinPressed()
    {
        Debug.Log("Join Button Pressed");
        // Your logic for joining a game
    }

    private void OnPrivatePressed()
    {
        Debug.Log("Private Button Pressed");

        if (connectionManager == null)
        {
            Debug.LogError("ConnectionManager reference is null. Cannot start private session.");
            return;
        }

        // Call StartPrivate on the ConnectionManager
        connectionManager.StartPrivate();

        // Hide the UIDocument by disabling it
        _document.gameObject.SetActive(false);
    }

    private void OnExitPressed()
    {
        Debug.Log("Exit Button Pressed");
#if UNITY_EDITOR
        // Stop playing the scene in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the application in a build
        Application.Quit();
#endif
    }
}
