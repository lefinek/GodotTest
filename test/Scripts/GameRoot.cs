using Godot;
using Godot.Collections;

public partial class GameRoot : Node2D
{
    [Export] public float SpawnTime { get; set; } = 1.0f;

    private GUIRoot _gui;
    private PackedScene _levelScene = GD.Load<PackedScene>("res://levels/basic_level.tscn");
    private EOSGMultiplayerPeer _peer = new EOSGMultiplayerPeer();
    private MultiplayerSpawner _multiplayerSpawner;

    private Dictionary _spawnTimers = new(); // peer_id -> float time remaining

    public override void _Ready()
    {
        _gui = GetNode<GUIRoot>("../GUI");
        _multiplayerSpawner = GetNode<MultiplayerSpawner>("MultiplayerSpawner");
        _peer.PeerConnectionClosed += OnPeerConnectionClosed;
        _peer.PeerConnected += OnPeerConnected;
        _peer.PeerDisconnected += OnPeerDisconnected;
    }

    public override void _Process(double delta)
    {
        if (_spawnTimers.Count == 0) return;
        var toRemove = new Godot.Collections.Array();
        foreach (int peer in _spawnTimers.Keys)
        {
            _spawnTimers[peer] = (float)_spawnTimers[peer] - (float)delta;
            if ((float)_spawnTimers[peer] <= 0f)
            {
                SpawnPlayer(peer);
                toRemove.Add(peer);
            }
        }
        foreach (int peer in toRemove)
        {
            _spawnTimers.Remove(peer);
        }
    }

    public void OnCreateServerButtonPressed()
    {
        var result = _peer.CreateServer("beef");
        if (result != Error.Ok) return;
        Multiplayer.MultiplayerPeer = _peer;
        var level = _levelScene.Instantiate();
        GetNode<Node2D>("Level").AddChild(level);
        GetNode<Control>("../GUI/PlayerHUD").Visible = true;
        SpawnPlayer(1);
    }

    public void SpawnPlayer(int peerId)
    {
        var playerScene = GD.Load<PackedScene>("res://player/Player.tscn");
        var player = playerScene.Instantiate<PlayerCharacter>();
        player.Name = peerId.ToString();
        player.OwnerId = peerId;
        var rect = GetViewport().GetVisibleRect().Size;
        var randPos = new Vector2(GD.Randi() % (int)rect.X, GD.Randi() % (int)rect.Y);
        player.Position = randPos;
        player.Connect(PlayerCharacter.SignalNameDied, new Callable(this, nameof(OnPlayerDied)));
        GetNode<Node2D>("Level").AddChild(player, true);
        var global = GetNode<GlobalData>("/root/GlobalData");
        global.Players[peerId] = player;
    }

    public void UnspawnPlayer(int peerId)
    {
        var global = GetNode<GlobalData>("/root/GlobalData");
        if (global.Players.ContainsKey(peerId))
        {
            (global.Players[peerId] as PlayerCharacter)?.QueueFree();
        }
        else
        {
            GD.PrintErr("Failed to unspawn player. Player not found.");
        }
    }

    public void OnConnectToServerMenuConnectPressed(string socketId, string remoteUserId)
    {
        var result = _peer.CreateClient(socketId, remoteUserId);
        if (result != Error.Ok) return;
        Multiplayer.MultiplayerPeer = _peer;
    }

    private void OnPeerConnected(int peerId)
    {
        if (_peer.GetActiveMode() == EOS.P2P.Mode.Client && peerId == 1)
        {
            _gui.ConnectionStatus.Visible = false;
            GetNode<Control>("../GUI/PlayerHUD").Visible = true;
        }
        if (_peer.GetActiveMode() == EOS.P2P.Mode.Server)
        {
            SpawnPlayer(peerId);
        }
    }

    private void OnPeerDisconnected(int peerId)
    {
        GD.Print("Peer has disconnected. Peer id: " + peerId);
        if (_peer.GetActiveMode() == EOS.P2P.Mode.Server)
        {
            UnspawnPlayer(peerId);
        }
    }

    public void OnDisconnectButtonPressed()
    {
        ClearLevel();
        _peer.Close();
        _gui.SetConnectionStatusLabel("Disconnected by local user.");
        _gui.ConnectionStatus.GetNode("BackButton").Set("visible", true);
        _gui.ConnectionStatus.Visible = true;
        GetNode<Control>("../GUI/PlayerHUD").Visible = false;
    }

    private void OnPeerConnectionClosed(Godot.Collections.Dictionary data)
    {
        var reason = (EOS.P2P.ConnectionClosedReason)data["reason"];
        switch (reason)
        {
            case EOS.P2P.ConnectionClosedReason.ClosedByPeer:
                if (_peer.GetActiveMode() != EOS.P2P.Mode.Server)
                {
                    GetNode<Control>("../GUI/PlayerHUD").Visible = false;
                    _gui.ConnectionStatus.Visible = true;
                    _gui.SetConnectionStatusLabel("Host has disconnected");
                    _gui.ConnectionStatus.GetNode("BackButton").Set("visible", true);
                }
                break;
            default:
                _gui.SetConnectionStatusLabel("Connection Failed");
                _gui.ConnectionStatus.GetNode("BackButton").Set("visible", true);
                break;
        }
    }

    private void OnPlayerDied(PlayerCharacter player)
    {
        player.Disconnect(PlayerCharacter.SignalNameDied, new Callable(this, nameof(OnPlayerDied)));
        UnspawnPlayer(player.OwnerId);
        _spawnTimers[player.OwnerId] = SpawnTime;
    }

    private void ClearLevel()
    {
        foreach (var child in GetNode<Node2D>("Level").GetChildren())
        {
            (child as Node)?.QueueFree();
        }
    }
}
