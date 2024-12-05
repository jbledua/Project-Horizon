using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    private UIDocument _document;
    private VisualElement parent;

    private VisualElement mainMenu;
    private VisualElement hostMenu;
    private VisualElement joinMenu;
    private VisualElement pauseMenu;
    private VisualElement gameLogo;

    private Button exitButton;

    private ConnectionManager connectionManager;

    [SerializeField]
    private GameObject mapGenerator;

    // Static instance for external access
    private static MainMenuUI instance;

    [System.Obsolete]
    private void OnEnable()
    {
       

        _document = GetComponent<UIDocument>();
        if (_document == null)
        {
            Debug.LogError("UIDocument not found. Ensure a UIDocument is attached to this GameObject.");
            return;
        }

        parent = _document.rootVisualElement.Q<VisualElement>("MenuContainer");
        if (parent == null)
        {
            parent = new VisualElement { name = "MenuContainer" };
            parent.style.flexDirection = FlexDirection.Column;
            _document.rootVisualElement.Add(parent);
        }

        connectionManager = FindObjectOfType<ConnectionManager>();
        if (connectionManager == null)
        {
            Debug.LogError("ConnectionManager not found in the scene.");
        }

        // Initialize menus and logo
        gameLogo = CreateGameLogo();
        mainMenu = CreateMainMenu();
        hostMenu = CreateHostMenu();
        joinMenu = CreateJoinMenu();
        pauseMenu = CreatePauseMenu();

        ShowGameLogo();

        instance = this;

    }

    private VisualElement CreateGameLogo()
    {
        //var logo = new VisualElement { name = "GameLogo" };
        //logo.AddToClassList("centerMenu");

        var logoLabel = new Label("Game Logo");
        logoLabel.AddToClassList("logoLabel");

        //logo.Add(logoLabel);
        parent.Add(logoLabel);

        logoLabel.style.display = DisplayStyle.None;
        return logoLabel;
    }

    private VisualElement CreateMainMenu()
    {
        var menu = new VisualElement { name = "MainMenu" };
        menu.AddToClassList("centerMenu");

        var hostButton = new Button(OnHostPressed) { text = "Host" };
        var joinButton = new Button(OnJoinPressed) { text = "Join" };
        var privateButton = new Button(OnPrivatePressed) { text = "Private" };
        exitButton = new Button(OnExitPressed) { text = "Exit" };

        hostButton.AddToClassList("button");
        joinButton.AddToClassList("button");
        privateButton.AddToClassList("button");
        exitButton.AddToClassList("button");

        menu.Add(hostButton);
        menu.Add(joinButton);
        menu.Add(privateButton);
        menu.Add(exitButton);

        parent.Add(menu);
        menu.style.display = DisplayStyle.None;
        return menu;
    }

    private VisualElement CreateHostMenu()
    {
        var menu = new VisualElement { name = "HostMenu" };
        menu.AddToClassList("centerMenu");

        var portField = new TextField("Listening Port:") { name = "hostPortField" };
        portField.AddToClassList("input");
        portField.labelElement.AddToClassList("inputLabel");

        var startButton = new Button(() => OnStartHostPressed(portField.text)) { text = "Start" };
        var backButton = new Button(() => OnBackPressed(menu, mainMenu)) { text = "Back" };

        startButton.AddToClassList("button");
        backButton.AddToClassList("button");

        menu.Add(portField);
        menu.Add(startButton);
        menu.Add(backButton);

        parent.Add(menu);
        menu.style.display = DisplayStyle.None;
        return menu;
    }

    private VisualElement CreateJoinMenu()
    {
        var menu = new VisualElement { name = "JoinMenu" };
        menu.AddToClassList("centerMenu");

        var ipField = new TextField("IP Address:") { name = "ipField" };
        var portField = new TextField("Port Number:") { name = "portField" };

        var connectButton = new Button(() => OnConnectPressed(ipField.text, portField.text)) { text = "Connect" };
        var backButton = new Button(() => OnBackPressed(menu, mainMenu)) { text = "Back" };

        ipField.AddToClassList("input");
        ipField.labelElement.AddToClassList("inputLabel");

        portField.AddToClassList("input");
        portField.labelElement.AddToClassList("inputLabel");

        connectButton.AddToClassList("button");
        backButton.AddToClassList("button");

        menu.Add(ipField);
        menu.Add(portField);
        menu.Add(connectButton);
        menu.Add(backButton);

        parent.Add(menu);
        menu.style.display = DisplayStyle.None;
        return menu;
    }

    private VisualElement CreatePauseMenu()
    {
        var menu = new VisualElement { name = "PauseMenu" };
        menu.AddToClassList("centerMenu");

        var resumeButton = new Button(OnResumePressed) { text = "Resume" };
        var quitButton = new Button(OnExitPressed) { text = "Quit" };

        resumeButton.AddToClassList("button");
        quitButton.AddToClassList("button");

        menu.Add(resumeButton);
        menu.Add(quitButton);

        parent.Add(menu);
        menu.style.display = DisplayStyle.None;
        return menu;
    }

    private void ShowMenu(VisualElement menu)
    {
        menu.style.display = DisplayStyle.Flex;
    }

    private void HideMenu(VisualElement menu)
    {
        menu.style.display = DisplayStyle.None;
    }

    private void ShowGameLogo()
    {
        if (instance == null)
        {
            ShowMenu(gameLogo);

            // Schedule hiding the logo and showing the main menu
            Invoke(nameof(ShowMainMenuAfterLogo), 3f); // Display logo for 3 seconds
        }

    }

    private void ShowMainMenuAfterLogo()
    {
        HideMenu(gameLogo);
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        HideMenu(hostMenu);
        HideMenu(joinMenu);
        HideMenu(pauseMenu);
        ShowMenu(mainMenu);
    }

    private void OnHostPressed()
    {
        HideMenu(mainMenu);
        ShowMenu(hostMenu);
    }

    private void OnJoinPressed()
    {
        HideMenu(mainMenu);
        ShowMenu(joinMenu);
    }

    private void OnPrivatePressed()
    {
        Debug.Log("Private Button Pressed");
        connectionManager?.StartPrivate();
        mapGenerator?.SetActive(true);
        _document.gameObject.SetActive(false);
    }

    private void OnExitPressed()
    {
        Debug.Log("Exit Button Pressed");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnStartHostPressed(string port)
    {
        if (ushort.TryParse(port, out ushort parsedPort))
        {
            connectionManager?.StartHosting(parsedPort);
            mapGenerator?.SetActive(true);
            _document.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"Invalid port number: {port}");
        }
    }

    private void OnConnectPressed(string ip, string port)
    {
        if (ushort.TryParse(port, out ushort parsedPort))
        {
            connectionManager?.StartConnection(ip, parsedPort);
            mapGenerator?.SetActive(true);
            _document.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"Invalid port number: {port}");
        }
    }

    private void OnBackPressed(VisualElement currentMenu, VisualElement targetMenu)
    {
        HideMenu(currentMenu);
        ShowMenu(targetMenu);
    }

    private void OnResumePressed()
    {
        HideMenu(pauseMenu);
        _document.gameObject.SetActive(false);
    }

    public static void ShowPauseMenu()
    {
        if (instance != null)
        {
            instance._document.gameObject.SetActive(true);
            instance.HideMenu(instance.mainMenu);
            instance.HideMenu(instance.hostMenu);
            instance.HideMenu(instance.joinMenu);
            instance.ShowMenu(instance.pauseMenu);
        }
        else
        {
            Debug.LogError("MainMenuUI instance not initialized. Ensure the script is attached to a GameObject in the scene.");
        }
    }
}
