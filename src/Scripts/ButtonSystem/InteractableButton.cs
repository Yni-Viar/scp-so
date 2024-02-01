using Godot;
using System;
/// <summary>
/// Generic button.
/// </summary>
public partial class InteractableButton : InteractableCommon
{
    /*
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    */

    internal override void Interact(Node3D player)
    {
        if (player is PlayerScript || player is CctvCamera)
        {
            if (GetParent().HasMethod("Interact"))
            {
                GetParent().Call("Interact", player);
            }
            base.Interact(player);
        }
    }
}
