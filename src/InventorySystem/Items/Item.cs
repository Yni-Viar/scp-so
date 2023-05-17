using Godot;
using System;

public partial class Item : Resource
{
    [Export]
    public string name { get; set; }

    [Export]
    public Texture2D texture { get; set; }

    [Export]
    public string pickablePath { get; set; }

    public Item() : this(null, null, null) {}
    public Item(string _name, Texture2D _texture, string _pickablePath)
    {
        name = _name;
        texture = _texture;
        pickablePath = _pickablePath;
    }
}
