using Godot;

public partial class MainRoot : Node2D
{
    [Export] public Node2D Game { get; set; }
    [Export] public LineEdit LocalUserId { get; set; }

    private LineEdit _devCredentialField;
    private GUIRoot _gui;

    public override void _Ready()
    {
        _devCredentialField = GetNode<LineEdit>("GUI/Login/DevCredential/Field");
        _gui = GetNode<GUIRoot>("GUI");
        InitializePlatform();
    }

    private void InitializePlatform()
    {
        var initOptions = new EOS.Platform.InitializeOptions
        {
            ProductName = "P2P Sample Game",
            ProductVersion = "1.0"
        };
        var initResult = EOS.Platform.PlatformInterface.Initialize(initOptions);
        if (initResult != EOS.Result.Success)
        {
            GD.Print("Failed to initialize EOS SDK: ", EOS.ResultStr(initResult));
            return;
        }
        GD.Print("Initialized EOS Platform");

        var productDetails = GetNode<ProductDetails>("/root/ProductDetails");
        var createOptions = new EOS.Platform.CreateOptions
        {
            ProductId = productDetails.product_id,
            SandboxId = productDetails.sandbox_id,
            DeploymentId = productDetails.deployment_id,
            ClientId = productDetails.client_id,
            ClientSecret = productDetails.client_secret,
            EncryptionKey = productDetails.encryption_key
        };
        var createResult = EOS.Platform.PlatformInterface.Create(createOptions);
        GD.Print("EOS Platform Created");

        EOS.GetInstance().LoggingInterfaceCallback += OnLoggingInterfaceCallback;
        var res = EOS.Logging.SetLogLevel(EOS.Logging.LogCategory.AllCategories, EOS.Logging.LogLevel.Info);
        if (res != EOS.Result.Success)
        {
            GD.Print("Failed to set log level: ", EOS.ResultStr(res));
        }
        EOS.GetInstance().ConnectInterfaceLoginCallback += OnConnectLoginCallback;
        EOS.GetInstance().AuthInterfaceLoginCallback += OnAuthLoginCallback;
    }

    private void OnLoggingInterfaceCallback(object msg)
    {
        var logMsg = EOS.Logging.LogMessage.From(msg) as EOS.Logging.LogMessage;
        GD.Print("SDK ", logMsg.Category, " | ", logMsg.Message);
    }

    private async void AnonymousLogin()
    {
        var opts = new EOS.Connect.CreateDeviceIdOptions
        {
            DeviceModel = OS.GetName() + " " + OS.GetModelName()
        };
        EOS.Connect.ConnectInterface.CreateDeviceId(opts);
        await ToSignal(EOS.GetInstance(), "connect_interface_create_device_id_callback");

        var credentials = new EOS.Connect.Credentials
        {
            Token = null,
            Type = EOS.ExternalCredentialType.DeviceidAccessToken
        };
        var loginOptions = new EOS.Connect.LoginOptions
        {
            Credentials = credentials,
            UserLoginInfo = new EOS.Connect.UserLoginInfo { DisplayName = "User" }
        };
        EOS.Connect.ConnectInterface.Login(loginOptions);
    }

    private void OnConnectLoginCallback(Godot.Collections.Dictionary data)
    {
        if (!(bool)data["success"])
        {
            GD.Print("Login failed");
            EOS.PrintResult(data);
            _gui.SetLoginStatusLabel("Login Failed");
            _gui.GetNode<Control>("LoginStatus").GetNode<Button>("Button").Visible = true;
            return;
        }
        LocalUserId.Text = (string)data["local_user_id"];
        GD.PrintRich("[b]Login successful[/b]: local_user_id=", data["local_user_id"]);
        _gui.GetNode<Control>("LoginStatus").Visible = false;
        _gui.GetNode<Control>("MainMenu").Visible = true;
    }

    private void OnAuthLoginCallback(Godot.Collections.Dictionary data)
    {
        if (!(bool)data["success"])
        {
            GD.Print("Login failed");
            EOS.PrintResult(data);
            _gui.SetLoginStatusLabel("Login Failed");
            _gui.GetNode<Control>("LoginStatus").GetNode<Button>("Button").Visible = true;
            return;
        }
        if ((string)data["local_user_id"] != "")
        {
            var epicAccountId = (string)data["local_user_id"];
            GD.Print("Epic Account Id: ", epicAccountId);
            var copyUserAuthToken = EOS.Auth.AuthInterface.CopyUserAuthToken(new EOS.Auth.CopyUserAuthTokenOptions(), epicAccountId);
            var token = copyUserAuthToken.Token;
            var options = new EOS.UserInfo.QueryUserInfoOptions
            {
                LocalUserId = epicAccountId,
                TargetUserId = epicAccountId
            };
            EOS.UserInfo.UserInfoInterface.QueryUserInfo(options);
            var credentials = new EOS.Connect.Credentials
            {
                Token = token.AccessToken,
                Type = EOS.ExternalCredentialType.Epic
            };
            var loginOptions = new EOS.Connect.LoginOptions
            {
                Credentials = credentials
            };
            EOS.Connect.ConnectInterface.Login(loginOptions);
        }
    }

    public void OnLoginButtonPressed()
    {
        if (_devCredentialField.Text.IsEmpty()) return;
        var credentials = new EOS.Auth.Credentials
        {
            Token = _devCredentialField.Text,
            Type = EOS.Auth.LoginCredentialType.Developer,
            Id = "localhost:7878"
        };
        var loginOptions = new EOS.Auth.LoginOptions
        {
            Credentials = credentials,
            ScopeFlags = EOS.Auth.ScopeFlags.BasicProfile
        };
        EOS.Auth.AuthInterface.Login(loginOptions);
    }

    public void OnDeviceIdButtonPressed()
    {
        AnonymousLogin();
    }
}
