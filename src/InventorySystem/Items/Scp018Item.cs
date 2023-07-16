using Godot;
using System;

public partial class Scp018Item : Item
{
    public override void OnUsed(PlayerScript target)
    {
        Scp018Activated scp = (Scp018Activated)ResourceLoader.Load<PackedScene>("res://Assets/Items/scp018_activated.tscn").Instantiate();
        target.GetNode<Marker3D>("PlayerHead/ItemSpawn").AddChild(scp);
    }
}
