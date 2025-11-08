using Godot;

public partial class Bullet : CharacterBody2D
{
    [Export] public int Speed { get; set; } = 300;
    public int OwnerId { get; set; }
    public int Damage { get; set; }
    public float Direction { get; set; }

    public override void _Process(double delta)
    {
        var rect = GetViewport().GetVisibleRect().Size;
        if (Position.X < 0 || Position.X > rect.X || Position.Y < 0 || Position.Y > rect.Y)
        {
            QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var vecDir = Vector2.Right.Rotated(Direction);
        var velocity = vecDir * Speed;
        var collision = MoveAndCollide(velocity * (float)delta);
        if (collision != null)
        {
            if (Multiplayer.IsServer())
            {
                var collider = collision.GetCollider();
                if (collider is PlayerCharacter pc)
                {
                    pc.Rpc(nameof(PlayerCharacter.TakeDamage), Damage, OwnerId);
                }
            }
            QueueFree();
        }
    }
}
