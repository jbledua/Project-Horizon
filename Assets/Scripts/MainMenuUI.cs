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

    [SerializeField]
    private GameObject mapGenerator;

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

    private void OnJoinPressed()
    {
        Debug.Log("Join Button Pressed");

        

        // Hide the main menu container
        var mainMenu = _document.rootVisualElement.Q<VisualElement>("ModeMenu");
        if (mainMenu != null)
        {
            mainMenu.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("MainMenu VisualElement not found. Ensure it has the correct name.");
            return;
        }


        // Locate or create the parent for the new menu
        var parent = _document.rootVisualElement.Q<VisualElement>("MenuContainer");
        if (parent == null)
        {
            Debug.LogWarning("MenuContainer not found. Creating a new container.");

            // Dynamically create a new parent container
            parent = new VisualElement { name = "MenuContainer" };
            parent.style.flexDirection = FlexDirection.Column;
            _document.rootVisualElement.Add(parent);
        }

        // Create a new menu container
        var joinMenu = new VisualElement();
        joinMenu.name = "JoinMenu";
        joinMenu.AddToClassList("centerMenu");

        // Create input fields
        var ipField = new TextField("IP Address:");
        ipField.name = "ipField";
        ipField.AddToClassList("inputField");
        joinMenu.Add(ipField);

        var portField = new TextField("Port Number:");
        portField.name = "portField";
        portField.AddToClassList("inputField");
        joinMenu.Add(portField);

        // Create buttons
        var connectButton = new Button(() => OnConnectPressed(ipField.text, portField.text))
        {
            text = "Connect"
        };
        connectButton.AddToClassList("button");
        joinMenu.Add(connectButton);

        var backButton = new Button(() => OnBackPressed(mainMenu, joinMenu))
        {
            text = "Back"
        };
        backButton.AddToClassList("button");
        joinMenu.Add(backButton);

        // Add the new menu to locator parent
        parent.Add(joinMenu);
    }

    private void OnHostPressed()
    {
        Debug.Log("Host Button Pressed");

        // Hide the main menu container
        var mainMenu = _document.rootVisualElement.Q<VisualElement>("ModeMenu");
        if (mainMenu != null)
        {
            mainMenu.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("MainMenu VisualElement not found. Ensure it has the correct name.");
            return;
        }

        // Locate or create the parent for the new menu
        var parent = _document.rootVisualElement.Q<VisualElement>("MenuContainer");
        if (parent == null)
        {
            Debug.LogWarning("MenuContainer not found. Creating a new container.");

            // Dynamically create a new parent container
            parent = new VisualElement { name = "MenuContainer" };
            parent.style.flexDirection = FlexDirection.Column;
            _document.rootVisualElement.Add(parent);
        }

        // Create a new menu container for hosting
        var hostMenu = new VisualElement();
        hostMenu.name = "HostMenu";
        hostMenu.AddToClassList("centerMenu");

        // Create an input field for the port number
        var portField = new TextField("Listening Port:");
        portField.name = "hostPortField";
        portField.AddToClassList("inputField");
        hostMenu.Add(portField);

        // Create buttons
        var startButton = new Button(() => OnStartHostPressed(portField.text))
        {
            text = "Start"
        };
        startButton.AddToClassList("button");
        hostMenu.Add(startButton);

        var backButton = new Button(() => OnBackPressed(mainMenu, hostMenu))
        {
            text = "Back"
        };
        backButton.AddToClassList("button");
        hostMenu.Add(backButton);

        // Add the new menu to the parent container
        parent.Add(hostMenu);
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

        if (mapGenerator != null) mapGenerator.SetActive(true);

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

    private void OnConnectPressed(string ip, string port)
    {
        Debug.Log("Connect Pressed");

        if (connectionManager == null)
        {
            Debug.LogError("ConnectionManager reference is null. Cannot start connection.");
            return;
        }

        // Try to convert the port to ushort
        if (!ushort.TryParse(port, out ushort parsedPort))
        {
            Debug.LogError($"Invalid port number: {port}. Please enter a valid number between 0 and 65535.");
            return;
        }

        // Call StartConnection on the ConnectionManager
        connectionManager.StartConnection(ip, parsedPort);

        if (mapGenerator != null) mapGenerator.SetActive(true);

        // Hide the UIDocument by disabling it
        _document.gameObject.SetActive(false);
    }


    private void OnStartHostPressed(string port)
    {
        // Add connection logic here
        Debug.Log("Connect Start Host");

        if (connectionManager == null)
        {
            Debug.LogError("ConnectionManager reference is null. Cannot start private session.");
            return;
        }

        // Try to convert the port to ushort
        if (!ushort.TryParse(port, out ushort parsedPort))
        {
            Debug.LogError($"Invalid port number: {port}. Please enter a valid number between 0 and 65535.");
            return;
        }

        // Call StartPrivate on the ConnectionManager
        connectionManager.StartHosting(parsedPort);

        if (mapGenerator != null) mapGenerator.SetActive(true);

        // Hide the UIDocument by disabling it
        _document.gameObject.SetActive(false);
    }


    private void OnBackPressed(VisualElement mainMenu, VisualElement joinMenu)
    {
        Debug.Log("Returning to Main Menu");

        // Show the main menu
        mainMenu.style.display = DisplayStyle.Flex;

        // Remove the join menu
        joinMenu.RemoveFromHierarchy();
    }
}
