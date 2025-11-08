using Godot;

public partial class ScoreLabel : Label
{
    private Label _amount;

    public override void _Ready()
    {
        _amount = GetNode<Label>("Amount");
        var global = GetNode<GlobalData>("/root/GlobalData");
        global.Connect(GlobalData.SignalNameScoreChanged, new Callable(this, nameof(OnScoreChanged)));
    }

    private void OnScoreChanged(int score)
    {
        _amount.Text = score.ToString();
    }
}
