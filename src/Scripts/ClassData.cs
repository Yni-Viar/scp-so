using Godot;
using System;

// Deprecated since 0.5.0-dev
public partial class ClassData : Node
{
    /// <summary>
    /// The player class consist in these elements:
    /// {"id", new Godot.Collections.Dictionary<string, Variant>{
    ///     {"className", value}, 
    ///     {"spawnPoint", value},
    ///     {"playerModelSource", value},
    ///     {"speed", value},
    ///     {"jump", value},
    ///     {"sprintEnabled", value},
    ///     {"moveSoundsEnabled", value},
    ///     {"footstepSounds", new Godot.Collections.Array<string>{}},
    ///     {"sprintSounds", new Godot.Collections.Array<string>{}},
    ///     {"scpNumber", value},
    ///     {"health", value}
    /// }
    /// "id". Id of the class. Must be assigned before all elements
    /// In the subgroup, these values:
    /// "className" - string, public name of the class
    /// "spawnPoint" - string, point, where player will be teleported, when assigned a class.
    /// "playerModelSource" - string, patrh to prefab with unique class player script.
    /// "speed" - float.
    /// "jump" - float, jump speed.
    /// "sprintEnabled" - bool, can this player classs sprint or not.
    /// "moveSoundsEnabled" - bool, can this player produce footstep sounds. This is made for SCP-650 (and SCP-079 in future).
    /// "footstepSounds" - Godot.Collections.Array<string>{}, list of footstep sounds.
    /// "sprintSounds" - Godot.Collections.Array<string>{}, list of sprint sounds.
    /// "scpNumber" - int, number of SCP. Human classes has this number as -1, spectators - -2
    /// "health" - float, amount of health
    /// </summary>
    public static Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> playerClasses =
      new Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>>{
        {"spectator", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "MTF Omega-0 \"Ará Orún\""},
            {"spawnPoint", "Main/Game/SurfaceMap/entityspawn_sp"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/spectator.tscn"},
            {"speed", 2f},
            {"jump", 2f},
            {"sprintEnabled", false},
            {"moveSoundsEnabled", true},
            {"footstepSounds", new Godot.Collections.Array<string>{}},
            {"sprintSounds", new Godot.Collections.Array<string>{}},
            {"scpNumber", -2},
            {"team", "SPT"}, 
            {"health", 1f},
            {"customSpawnPoint", true}
        }},
        {"guard", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "Security staff"},
            {"spawnPoint", "Main/Game/MapGenLcz/LC_room2_sl/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/guard.tscn"},
            {"speed", 4.5f},
            {"jump", 4.25f},
            {"sprintEnabled", true},
            {"moveSoundsEnabled", true},
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
            {"sprintSounds", new Godot.Collections.Array<string>{
                "res://Sounds/Character/Human/Step/Run1.ogg",
                "res://Sounds/Character/Human/Step/Run2.ogg",
                "res://Sounds/Character/Human/Step/Run3.ogg",
                "res://Sounds/Character/Human/Step/Run4.ogg",
                "res://Sounds/Character/Human/Step/Run5.ogg",
                "res://Sounds/Character/Human/Step/Run6.ogg",
                "res://Sounds/Character/Human/Step/Run7.ogg",
                "res://Sounds/Character/Human/Step/Run8.ogg"
            }},
            {"scpNumber", -1},
            {"team", "SPT"},
            {"health", 100f},
            {"customSpawnPoint", false}
        }},
        {"mtfe11", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "MTF Epsilon-11 \"Nine-Tailed Fox\""},
            {"spawnPoint", "Main/Game/SurfaceMap/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/mtf.tscn"},
            {"speed", 4.51f},
            {"jump", 4.26f},
            {"sprintEnabled", true},
            {"moveSoundsEnabled", true},
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
            {"sprintSounds", new Godot.Collections.Array<string>{
                "res://Sounds/Character/Human/Step/Run1.ogg",
                "res://Sounds/Character/Human/Step/Run2.ogg",
                "res://Sounds/Character/Human/Step/Run3.ogg",
                "res://Sounds/Character/Human/Step/Run4.ogg",
                "res://Sounds/Character/Human/Step/Run5.ogg",
                "res://Sounds/Character/Human/Step/Run6.ogg",
                "res://Sounds/Character/Human/Step/Run7.ogg",
                "res://Sounds/Character/Human/Step/Run8.ogg"
            }},
            {"scpNumber", -1},
            {"health", 100f},
            {"customSpawnPoint", false}
        }},

        {"scp173", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "SCP-173 \"Sculpture\""},
            {"spawnPoint", "Main/Game/MapGenHcz/HC_cont1_173/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/scp173.tscn"},
            {"speed", 9f},
            {"jump", 3f},
            {"sprintEnabled", false},
            {"moveSoundsEnabled", true},
            {"footstepSounds", new Godot.Collections.Array<string>{
                "res://Sounds/Character/173/Rattle1.ogg",
                "res://Sounds/Character/173/Rattle2.ogg",
                "res://Sounds/Character/173/Rattle3.ogg"
            }},
            {"sprintSounds", new Godot.Collections.Array<string>{
                "res://Sounds/Character/173/Rattle1.ogg",
                "res://Sounds/Character/173/Rattle2.ogg",
                "res://Sounds/Character/173/Rattle3.ogg"
            }},
            {"scpNumber", 173},
            {"health", 10000f},
            {"customSpawnPoint", false}
        }},
        {"scp650", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "SCP-650 \"Startling Statue\""},
            {"spawnPoint", "Main/Game/MapGenLcz/LC_cont2_650/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/scp650.tscn"},
            {"speed", 0f},
            {"jump", 0f},
            {"sprintEnabled", false},
            {"moveSoundsEnabled", false},
            {"footstepSounds", new Godot.Collections.Array<string>{}},
            {"sprintSounds", new Godot.Collections.Array<string>{}},
            {"scpNumber", 650},
            {"health", 1500f},
            {"customSpawnPoint", false}
        }},
        {"scp3199", new Godot.Collections.Dictionary<string, Variant>{
            {"className", "SCP-3199 \"Humans, Refuted\""},
            {"spawnPoint", "Main/Game/MapGenHcz/HC_cont1_3199/entityspawn"},
            {"playerModelSource", "res://FPSController/PlayerClassPrefab/scp3199.tscn"},
            {"speed", 4.4f},
            {"jump", 4.48f},
            {"sprintEnabled", false},
            {"moveSoundsEnabled", true},
            {"footstepSounds", new Godot.Collections.Array<string>{
                "res://Sounds/Character/SCPCommon/StepSCP1.ogg",
                "res://Sounds/Character/SCPCommon/StepSCP2.ogg",
                "res://Sounds/Character/SCPCommon/StepSCP3.ogg",
                "res://Sounds/Character/SCPCommon/StepSCP4.ogg"
            }},
            {"sprintSounds", new Godot.Collections.Array<string>{
                "res://Sounds/Character/SCPCommon/StepSCP1.ogg",
                "res://Sounds/Character/SCPCommon/StepSCP2.ogg",
                "res://Sounds/Character/SCPCommon/StepSCP3.ogg",
                "res://Sounds/Character/SCPCommon/StepSCP4.ogg"
            }},
            {"scpNumber", 3199},
            {"health", 2000f},
            {"customSpawnPoint", false}
        }}
    };
}