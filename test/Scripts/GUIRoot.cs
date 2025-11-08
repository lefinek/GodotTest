using Godot;

public partial class GUIRoot : Control
{
    private Control _mainMenu;
    private Control _mainMenuOptions;
    private Control _connectToServerMenu;
    private HBoxContainer _localUserIdField;
    public Control ConnectionStatus { get; private set; }
    private Control _login;
    private Control _loginStatus;
    private Control _playerHud;

    public override void _Ready()
    {
        _mainMenu = GetNode<Control>("MainMenu");
        _mainMenuOptions = GetNode<Control>("MainMenu/MainMenuOptions");
        _connectToServerMenu = GetNode<Control>("MainMenu/ConnectToServerMenu");
        _localUserIdField = GetNode<HBoxContainer>("../LocalUserId");
        ConnectionStatus = GetNode<Control>("ConnectionStatus");
        _login = GetNode<Control>("Login");
        _loginStatus = GetNode<Control>("LoginStatus");
        _playerHud = GetNode<Control>("PlayerHUD");
    }

    public void SetLoginStatusLabel(string message)
    {
        _loginStatus.GetNode<Label>("Label").Text = message;
    }

    public void SetConnectionStatusLabel(string message)
    {
        ConnectionStatus.GetNode<Label>("Status").Text = message;
    }

    public void OnConnectToServerMenuBackButtonPressed()
    {
        _mainMenuOptions.Visible = true;
        _connectToServerMenu.Visible = false;
    }

    public void OnDeviceIdLoginButtonPressed()
    {
        _login.Visible = false;
        _loginStatus.Visible = true;
    }

    public void OnLoginButtonPressed()
    {
        _login.Visible = false;
        _loginStatus.Visible = true;
    }

    public void OnLoginBackButtonPressed()
    {
        SetLoginStatusLabel("Loggin in...");
        _loginStatus.Visible = false;
        _login.Visible = true;
    }

    public void OnServerConnectButtonPressed()
    {
        _mainMenu.Visible = false;
        ConnectionStatus.Visible = true;
    }

    public void OnCreateClientButtonPressed()
    {
        _mainMenuOptions.Visible = false;
        _connectToServerMenu.Visible = true;
    }

    public void OnConnectionStatusBackButtonPressed()
    {
        SetConnectionStatusLabel("Connecting...");
        ConnectionStatus.GetNode<Button>("BackButton").Visible = false;
        ConnectionStatus.Visible = false;
        _mainMenu.Visible = true;
    }

    public void OnCreateServerButtonPressed()
    {
        _mainMenu.Visible = false;
    }
}
