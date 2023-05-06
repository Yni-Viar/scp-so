#if TOOLS
using Godot;
using System;

[Tool]
public partial class itemsystem : EditorPlugin
{
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
        // Add the new type with a name, a parent type, a script and an icon.
        var script = GD.Load<Script>("res://Addons/itemsystem/ItemPickup/Pickable.cs");
        var texture = GD.Load<Texture2D>("res://Addons/itemsystem/custom_label.svg");
        AddCustomType("Pickable", "RigidBody3D", script, texture);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        // Always remember to remove it from the engine when deactivated.
        RemoveCustomType("Pickable");
    }
}
#endif
