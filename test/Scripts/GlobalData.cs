using Godot;
using Godot.Collections;

public partial class GlobalData : Node
{
    // Signals (snake_case to match existing GDScript usage)
    [Signal] public delegate void score_changedEventHandler(int score);
    [Signal] public delegate void local_player_spawnedEventHandler(PlayerCharacter player);

    // players dictionary (int peer_id -> PlayerCharacter)
    public Dictionary players { get; private set; } = new();

    // score property with change notification
    private int _score;
    [Export]
    public int score
    {
        get => _score;
        set
        {
            _score = value;
            EmitSignal(nameof(score_changed), _score);
        }
    }

    // local_player reference with spawn signal
    private PlayerCharacter _localPlayer;
    [Export]
    public PlayerCharacter local_player
    {
        get => _localPlayer;
        set
        {
            _localPlayer = value;
            EmitSignal(nameof(local_player_spawned), _localPlayer);
        }
    }

    // RPC method invoked by server to increment score
    [Rpc(MultiplayerPeer.RpcMode.Authority)]
    public void scored()
    {
        score += 1;
    }
}
