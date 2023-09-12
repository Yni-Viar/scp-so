using Godot;
using System;

[Tool]
public partial class Item : Resource
{
    [Export]
    public string InternalName { get; set; }
    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D Texture { get; set; }

    [Export]
    public string PickablePath { get; set; }

    [Export]
    public string PickupSoundPath { get; set; }

    [Export]
    public string FirstPersonPrefabPath { get; set; }

    public Item() : this(null, null, null, null, null, null) {}
    public Item(string internalName, string name, Texture2D texture, string pickablePath, string pickupSoundPath, string firstPersonPrefabPath)
    {
        InternalName = internalName;
        Name = name;
        Texture = texture;
        PickablePath = pickablePath;
        PickupSoundPath = pickupSoundPath;
        FirstPersonPrefabPath = firstPersonPrefabPath;
    }
}
