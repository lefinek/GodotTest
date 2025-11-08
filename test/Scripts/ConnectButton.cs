using Godot;

public partial class ConnectButton : Button
{
    [Signal] public delegate void ConnectPressedEventHandler(string socketId, string remoteUserId);

    private LineEdit _remoteUserIdField;

    public override void _Ready()
    {
        _remoteUserIdField = GetNode<LineEdit>("../ConnectFields/RemoteUserId/RemoteUserIdField");
        Pressed += OnPressed;
    }

    private void OnPressed()
    {
        EmitSignal(SignalNameConnectPressed, "beef", _remoteUserIdField.Text);
    }
}
