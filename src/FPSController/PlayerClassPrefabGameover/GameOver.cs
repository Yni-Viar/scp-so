using Godot;
using System;

public partial class GameOver : Node3D
{
    enum ObjectType { Static, Animated, Ragdoll}
    [Export] string state;
    [Export] internal bool isContainable;
    [Export] ObjectType objectType = ObjectType.Static;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
        switch (objectType)
        {
            case ObjectType.Animated:
                SetState(state);
                break;
            case ObjectType.Ragdoll:
                GetNode<Skeleton3D>("Armature/Skeleton3D").PhysicalBonesStartSimulation();
                break;
        }
        await ToSignal(GetTree().CreateTimer(30.0), "timeout");
        Despawn();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    internal void SetState(string s)
    {
        if (GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation != s)
        {
            //Change the animation.
            GetNode<AnimationPlayer>("AnimationPlayer").Play(s);
        }
    }

    /// <summary>
    /// Despawn. Used by Recontain mechanic + body remover. Available since 0.7.0-RC.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void Despawn()
    {
        if (objectType == ObjectType.Ragdoll)
        {
            GetNode<Skeleton3D>("Armature/Skeleton3D").PhysicalBonesStopSimulation();
        }
        QueueFree();
    }
}
