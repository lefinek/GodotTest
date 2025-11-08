using Godot;

public partial class HealthBar : HBoxContainer
{
    private TextureProgressBar _progressBar;

    public override void _Ready()
    {
        _progressBar = GetNode<TextureProgressBar>("TexturedProgressBar");
        var global = GetNode<GlobalData>("/root/GlobalData");
        global.Connect(GlobalData.SignalNameLocalPlayerSpawned, new Callable(this, nameof(OnLocalPlayerSpawned)));
    }

    private void OnLocalPlayerSpawned(PlayerCharacter player)
    {
        _progressBar.MaxValue = player.MaxHealth;
        _progressBar.Value = player.MaxHealth;
        player.Connect(PlayerCharacter.SignalNameCurrentHealthChanged, new Callable(this, nameof(OnLocalPlayerHealthChanged)));
        player.Connect(PlayerCharacter.SignalNameDied, new Callable(this, nameof(OnLocalPlayerDied)));
    }

    private void OnLocalPlayerHealthChanged(int currentHealth)
    {
        _progressBar.Value = currentHealth;
    }

    private void OnLocalPlayerDied(PlayerCharacter player)
    {
        _progressBar.Value = 0;
        player.Disconnect(PlayerCharacter.SignalNameCurrentHealthChanged, new Callable(this, nameof(OnLocalPlayerHealthChanged)));
        player.Disconnect(PlayerCharacter.SignalNameDied, new Callable(this, nameof(OnLocalPlayerDied)));
    }
}
