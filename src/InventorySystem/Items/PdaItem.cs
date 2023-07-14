using Godot;
using System;

public partial class PdaItem : Item
{
    public override void OnUsed()
    {
        GD.Print("Used PDA");
    }
}
