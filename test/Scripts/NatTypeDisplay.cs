using Godot;

public partial class NatTypeDisplay : HBoxContainer
{
    private RichTextLabel _natTypeLabel;

    public override void _Ready()
    {
        _natTypeLabel = GetNode<RichTextLabel>("NATTypeLabel");
        EOS.GetInstance().P2PInterfaceQueryNatTypeCallback += OnQueryNatTypeComplete;
        EOS.GetInstance().ConnectInterfaceLoginCallback += OnConnectInterfaceLogin;
    }

    private void OnConnectInterfaceLogin(Godot.Collections.Dictionary data)
    {
        EOS.P2P.P2PInterface.QueryNatType();
    }

    private void OnQueryNatTypeComplete(EOS.P2P.NATType natType)
    {
        switch (natType)
        {
            case EOS.P2P.NATType.Open:
                _natTypeLabel.Text = "[color=green]Open[/color]";
                break;
            case EOS.P2P.NATType.Moderate:
                _natTypeLabel.Text = "[color=yellow]Moderate[/color]";
                break;
            case EOS.P2P.NATType.Strict:
                _natTypeLabel.Text = "[color=red]Strict[/color]";
                break;
            default:
                _natTypeLabel.Text = "[color=black]Unknown[/color]";
                break;
        }
    }
}
