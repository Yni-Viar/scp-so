using Godot;
using System;

[Tool]
public partial class Item : Resource
{
    [ExportGroup("Common item properties")]
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

    [Export]
    public bool OneTimeUse { get; set; }

    [ExportGroup("SCP-914 Outputs (path to a PICKABLE, not ITEM)")]
    [Export]
    public string[] Rough { get; set; }
    [Export]
    public string[] Coarse { get; set; }
    [Export]
    public string[] OneToOne { get; set; }
    [Export]
    public string[] Fine { get; set; }
    [Export]
    public string[] VeryFine { get; set; }

    public Item() : this(null, null, null, null, null, null, true, null, null, null, null, null) {}
    public Item(string internalName, string name, Texture2D texture, string pickablePath, string pickupSoundPath, 
        string firstPersonPrefabPath, bool oneTimeUse, string[] rough, string[] coarse, string[] oneToOne, string[] fine, string[] veryFine)
    {
        InternalName = internalName;
        Name = name;
        Texture = texture;
        PickablePath = pickablePath;
        PickupSoundPath = pickupSoundPath;
        FirstPersonPrefabPath = firstPersonPrefabPath;
        OneTimeUse = oneTimeUse;
        Rough = rough ?? System.Array.Empty<string>(); ;
        Coarse = coarse ?? System.Array.Empty<string>(); ;
        OneToOne = oneToOne ?? System.Array.Empty<string>(); ;
        Fine = fine ?? System.Array.Empty<string>(); ;
        VeryFine = veryFine ?? System.Array.Empty<string>(); ;
    }
}
