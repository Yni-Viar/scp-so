using Godot;
using System;

public partial class PuzzleBase : Control
{
    [Export] string playerInputCode = "";
    [Export] Godot.Collections.Dictionary<string, string> answerCodes = new Godot.Collections.Dictionary<string, string>();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        answerCodes.Add(GetTree().Root.GetNode<BreachGameMode>("Main/Game").omegaWarheadKeycode, "omega_warhead");
	}
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Visible) //trying to optimize
        {
            for (int i = 0; i < 10; i++)
            {
                if (Input.IsActionJustPressed("keypad_" + i))
                {
                    playerInputCode += i;
                }
            }
            if (playerInputCode.Length == 4)
            {
                string[] answerKeys = new string[answerCodes.Count];
                answerCodes.Keys.CopyTo(answerKeys, 0);
                for (int i = 0; i < answerKeys.Length; i++)
                {
                    if (playerInputCode == answerKeys[i])
                    {
                        Unlock(answerCodes[answerKeys[i]]);
                        GetNode<Label>("AccessLabel").Text = "Access granted!";
                    }
                }
                GD.Print("Verbose: types 4 digits.");
                playerInputCode = "";
            }
            else if (playerInputCode.Length > 4)
            {
                GD.Print("Verbose: overflow.");
                playerInputCode = "";
            }
        }
    }
    /// <summary>
    /// Calls script of that thing, that was unlocked.
    /// </summary>
    /// <param name="answer"></param>
    void Unlock(string answer)
    {
        switch (answer)
        {
            case "omega_warhead":
                if (!GetTree().Root.GetNode<WarheadController>("Main/Game/OmegaWarhead").detonationProgress)
                {
                    GetTree().Root.GetNode<WarheadController>("Main/Game/OmegaWarhead").Rpc("Countdown");
                }
                break;
            case "alpha_warhead":
                GD.Print("Not implemented :(");
                break;
        }
    }
}
