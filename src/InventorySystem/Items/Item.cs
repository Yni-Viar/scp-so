using Godot;
using System;

public partial class Item : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D Texture { get; set; }

    [Export]
    public string PickablePath { get; set; }

    [Export]
    public bool OneTimeUse { get; set; }

    public Item() : this(null, null, null, true) {}
    public Item(string name, Texture2D texture, string pickablePath, bool oneTimeUse)
    {
        Name = name;
        Texture = texture;
        PickablePath = pickablePath;
        OneTimeUse = oneTimeUse;
    }

    public virtual void OnUsed(PlayerScript target)
    {
    }
}
