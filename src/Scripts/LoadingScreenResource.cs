using Godot;
using System;

[Tool]
public partial class LoadingScreenResource : Resource
{
    [Export]
    public string Name { get; set; }
    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; }

    [Export]
    public Texture2D Texture { get; set; }

    public LoadingScreenResource() : this(null, null, null) {}
    public LoadingScreenResource(string name, string description, Texture2D texture)
    {
        Name = name;
        Description = description;
        Texture = texture;
    }
}
