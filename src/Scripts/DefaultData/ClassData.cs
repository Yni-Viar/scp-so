using Godot;
using System;

public partial class ClassData : Node
{
    public static Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> playerClasses = 
      new Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>>{
        {"spectator", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "MTF Omega-0 'Ará Orún'"},
            {"spawnPoint", "Main/Game/MapGenLcz/LC_room1_archive/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/spectator.tscn"},
            {"speed", 2f},
            {"jump", 2f},
            {"footstepSounds", new Godot.Collections.Array<string>{}},
            {"scpNumber", -2},
            {"health", 100f}
        }},
        {"default", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "Test"},
            {"spawnPoint", "Main/Game/MapGenLcz/LC_room2_sl/entityspawn"},
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
            {"scpNumber", -1},
            {"health", 100f}
        }},
        {"scp173", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "SCP-173"},
            {"spawnPoint", "Main/Game/MapGenHcz/HC_cont1_173/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/scp173.tscn"},
            {"speed", 9f},
            {"jump", 3f},
            {"footstepSounds", new Godot.Collections.Array<string>{
                "res://Sounds/Character/173/Rattle1.ogg",
                "res://Sounds/Character/173/Rattle2.ogg",
                "res://Sounds/Character/173/Rattle3.ogg"
            }},
            {"scpNumber", 173},
            {"health", 10000f}
        }},
        {"scp650", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "SCP-650"},
            {"spawnPoint", "Main/Game/MapGenLcz/LC_cont2_650/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/scp650.tscn"},
            {"speed", 0f},
            {"jump", 0f},
            {"footstepSounds", new Godot.Collections.Array<string>{
                "res://Sounds/Character/173/Rattle1.ogg",
                "res://Sounds/Character/173/Rattle2.ogg",
                "res://Sounds/Character/173/Rattle3.ogg"
            }},
            {"scpNumber", 650},
            {"health", 5000f}
        }},
        {"scp3199", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "SCP-3199"},
            {"spawnPoint", "Main/Game/MapGenHcz/HC_cont2_testroom/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/scp3199.tscn"},
            {"speed", 4f},
            {"jump", 4f},
            {"footstepSounds", new Godot.Collections.Array<string>{
                "res://Sounds/Character/SCPCommon/StepSCP1.ogg",
                "res://Sounds/Character/SCPCommon/StepSCP2.ogg",
                "res://Sounds/Character/SCPCommon/StepSCP3.ogg",
                "res://Sounds/Character/SCPCommon/StepSCP4.ogg"
            }},
            {"scpNumber", 3199},
            {"health", 2000f}
        }}
    };
}