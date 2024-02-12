using Godot;
using System;
/// <summary>
/// Basement code
/// </summary>
public partial class PDBasement : StaticBody3D
{
    [Export] string answerCode;
    [Export] string playerInputCode = "";
    string[] exitValues = new string[PlacesForTeleporting.defaultData.Values.Count];

    RandomNumberGenerator rng = new RandomNumberGenerator();
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        answerCode = rng.RandiRange(1, 4).ToString() + rng.RandiRange(1, 4).ToString() + rng.RandiRange(1, 4).ToString() + rng.RandiRange(1, 4).ToString();
        GetNode<Label3D>("EscapeCode").Text = answerCode;
        PlacesForTeleporting.defaultData.Values.CopyTo(exitValues, 0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!string.IsNullOrEmpty(playerInputCode))
        {
            if (playerInputCode.Length == 4)
            {
                if (playerInputCode == answerCode)
                {
                    Rpc("Unblock");
                    SetProcess(false);
                }
                else
                {
                    GetNode<AudioStreamPlayer>("AudioStreamPlayer").Stream = ResourceLoader.Load<AudioStream>("res://Sounds/Character/106/Laugh.ogg");
                    GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
                    playerInputCode = "";
                }
            }
        }
    }

    private void OnExitAreaBodyEntered(Node3D body)
    {
        if (body is PlayerScript player)
        {
            TeleportTo(player, exitValues[rng.RandiRange(0, exitValues.Length - 1)]);
        }
    }
    /// <summary>
    /// Unblocks the exit
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void Unblock()
    {
        GetNode<CollisionShape3D>("ExitBlock").Disabled = true;
    }
    /// <summary>
    /// Calls TeleportTo method remotely on FacilityManager. More info in FacilityManager's method.
    /// </summary>
    /// <param name="player">Player to teleport</param>
    /// <param name="position">Position, where player will be teleported</param>
    void TeleportTo(PlayerScript player, string position)
    {
        GetParent().GetParent().GetParent<FacilityManager>().Rpc("TeleportTo", player.Name, position);
    }

    void Interact(string name)
    {
        switch(name)
        {
            case "Button1":
                playerInputCode += "1";
                break;
            case "Button2":
                playerInputCode += "2";
                break;
            case "Button3":
                playerInputCode += "3";
                break;
            case "Button4":
                playerInputCode += "4";
                break;
        }
    }
}

