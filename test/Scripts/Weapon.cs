using Godot;

public partial class Weapon : Node2D
{
    [Export] public int Damage { get; set; } = 1;
    [Export] public PackedScene Projectile { get; set; }
    [Export] public float RateOfFire { get; set; } = 120f;

    private Node2D _firePos;
    private float _cooldown = 0f;

    public override void _Ready()
    {
        _firePos = GetNode<Node2D>("FirePos");
    }

    public override void _Process(double delta)
    {
        _cooldown -= (float)delta;
    }

    public void Fire()
    {
        if (_cooldown > 0f) return;
        if (Projectile == null) return;
        var newProjectile = Projectile.Instantiate() as Bullet;
        if (newProjectile == null) return;
        newProjectile.Position = _firePos.GlobalPosition;
        newProjectile.Damage = Damage;
        newProjectile.Direction = Rotation;
        var player = GetParent() as PlayerCharacter;
        newProjectile.OwnerId = player?.OwnerId ?? 0;
        var level = GetNode<Node2D>("/root/Main/Game/Level");
        level.AddChild(newProjectile, true);
        _cooldown = 60f / RateOfFire;
    }
}
