using Godot;
using System;

public partial class PdaItem : Item
{
    public override void OnUsed(PlayerScript target)
    {
        GD.Print("Used PDA");
    }
}
