using Godot;
using System;

public partial class DoorStaticOpener : StaticBody3D
{
	/*// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}*/

	void CallOpen()
	{
		GetParent().GetParent<Door>().Rpc("DoorControl", this.GetPath(), -1);
	}

    void CallLock()
    {
        GetParent().GetParent<Door>().Rpc("DoorLock");
    }
}
