using Godot;

public partial class PlayerCharacter : CharacterBody2D
{
    [Signal] public delegate void CurrentHealthChangedEventHandler(int currentHealth);
    [Signal] public delegate void DiedEventHandler(PlayerCharacter player);

    [Export] public float Speed { get; set; } = 300f;
    [Export] public int MaxHealth { get; set; } = 100;
    [Export] public int OwnerId { get; set; }

    private int _currentHealth;
    [Export]
    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;
            EmitSignal(SignalNameCurrentHealthChanged, _currentHealth);
        }
    }

    private Weapon _gun;

    public override void _Ready()
    {
        _gun = GetNode<Weapon>("Gun");
        if (OwnerId == Multiplayer.GetUniqueId())
        {
            var global = GetNode<GlobalData>("/root/GlobalData");
            global.LocalPlayer = this;
        }
        if (_currentHealth == 0) _currentHealth = MaxHealth;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (OwnerId != Multiplayer.GetUniqueId()) return;

        var input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = input * Speed;
        MoveAndSlide();

        var mousePos = GetViewport().GetMousePosition();
        _gun.LookAt(mousePos);

        Rpc(nameof(MoveRemote), GlobalPosition);
        Rpc(nameof(GunLookAtRemote), mousePos);

        if (Input.IsActionPressed("fire"))
        {
            Rpc(nameof(Fire));
        }
    }

    [Rpc(MultiplayerPeer.RpcMode.Authority)]
    public void TakeDamage(int amount, int peerId)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            EmitSignal(SignalNameDied, this);
            var global = GetNode<GlobalData>("/root/GlobalData");
            global.RpcId(peerId, nameof(GlobalData.Scored));
        }
    }

    [Rpc(MultiplayerPeer.RpcMode.AnyPeer)]
    public void Fire()
    {
        _gun.Fire();
    }

    [Rpc(MultiplayerPeer.RpcMode.AnyPeer)]
    public void MoveRemote(Vector2 position)
    {
        GlobalPosition = position;
    }

    [Rpc(MultiplayerPeer.RpcMode.AnyPeer)]
    public void GunLookAtRemote(Vector2 position)
    {
        _gun.LookAt(position);
    }
}
