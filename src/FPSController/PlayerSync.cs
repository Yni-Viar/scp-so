using Godot;
using System;

public partial class PlayerSync : MultiplayerSynchronizer
{
    internal Vector2 direction;
    /* old buggy code
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void Jump()
    {
        isJumping = true;
    }
    */
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        direction = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
	}
}
