using Godot;
using System;

public partial class Scp650PlayerScript : Node3D
{
    RandomNumberGenerator rng = new RandomNumberGenerator();
    string[] poseArr = new string[]{"Default", "Pose 1", "Pose 4", "Pose 5", "Pose 6", "Pose 7", "Pose 8", "Pose 9", "Pose 10"};
    // bool isWatching = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetNode<Node3D>("Armature").Hide();
            GetNode<Control>("AbilityUI").Show();
        }
        GetParent().GetParent<PlayerScript>().SetCollisionMaskValue(3, true);
        GetParent().GetParent<PlayerScript>().CanMove = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            if (Input.IsActionJustPressed("scp650_teleport"))// && !isWatching) //buggy
            {
                GetParent().GetParent().GetParent().GetParent().GetNode<FacilityManager>("Game").Rpc("RandomTeleport", GetParent().GetParent<PlayerScript>().Name);
                Rpc("SetState", poseArr[rng.RandiRange(0, poseArr.Length - 1)]);
            }
        }
        
        /*if (isWatching)
        {
            GD.Print("Humans are watching at me!");
        }*/
	}

    /*[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    void Scp650(bool watching)
    {
        if (watching)
        {
            isWatching = true;
        }
        else
        {
            isWatching = false;
        }
	}*/
    /// <summary>
    /// Set animation to an entity.
    /// </summary>
    /// <param name="s">Animation name</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void SetState(string s)
    {
        if (GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation != s)
        {
            //Change the animation.
            GetNode<AnimationPlayer>("AnimationPlayer").Play(s);
        }
    }
}
