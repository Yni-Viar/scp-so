using Godot;
using System;

public partial class ClassData : Node
{
    public static Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> playerClasses = 
      new Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>>{
        {"spectator", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "MTF Omega-0 'Ará Orún'"},
            {"spawnPoint", "Main/Game/MapGen/LC_room1_archive/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/spectator.tscn"},
            {"speed", 2f},
            {"jump", 2f},
            {"footstepSounds", new Godot.Collections.Array<string>{}},
            {"scpNumber", -2}
        }},
        {"default", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "Test"},
            {"spawnPoint", "Main/Game/MapGen/LC_room1_archive/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/Default.tscn"},
            {"speed", 4.5f},
            {"jump", 4.25f},
            {"footstepSounds", new Godot.Collections.Array<string>{
                "res://Sounds/Character/Human/Step/Step1.ogg",
                "res://Sounds/Character/Human/Step/Step2.ogg",
                "res://Sounds/Character/Human/Step/Step3.ogg",
                "res://Sounds/Character/Human/Step/Step4.ogg",
                "res://Sounds/Character/Human/Step/Step5.ogg",
                "res://Sounds/Character/Human/Step/Step6.ogg",
                "res://Sounds/Character/Human/Step/Step7.ogg",
                "res://Sounds/Character/Human/Step/Step8.ogg"
            }},
            {"scpNumber", -1}
        }},
        {"scp173", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "SCP-173"},
            {"spawnPoint", "Main/Game/MapGen/HC_cont1_173/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/scp173.tscn"},
            {"speed", 9f},
            {"jump", 3f},
            {"footstepSounds", new Godot.Collections.Array<string>{
                "res://Sounds/Character/173/Rattle1.ogg",
                "res://Sounds/Character/173/Rattle2.ogg",
                "res://Sounds/Character/173/Rattle3.ogg"
            }},
            {"scpNumber", 173}
        }},
    };
}