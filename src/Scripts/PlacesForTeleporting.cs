using Godot;
using System;
//It will be a JSON script in future.
public class PlacesForTeleporting
{
    internal static Godot.Collections.Dictionary<string, string> defaultData = new Godot.Collections.Dictionary<string, string> {
        { "hube", "Main/Game/PD/PD_basement/entityspawn"},
        { "basement", "Main/Game/PD/PD_basement/entityspawn" },
        { "lcz_archive", "Main/Game/MapGenLcz/LC_room1_archive/entityspawn" },
        { "lcz_securityroom", "Main/Game/MapGenLcz/LC_room2_sl/entityspawn" },
        { "hcz_173spawn", "Main/Game/MapGenHcz/HC_cont1_173/entityspawn" },
        { "hcz_testroom", "Main/Game/MapGenHcz/HC_cont2_testroom/entityspawn" },
        { "lcz_650spawn", "Main/Game/MapGenLcz/LC_cont2_650/entityspawn" },
        { "hcz_3199spawn", "Main/Game/MapGenHcz/HC_cont1_3199/entityspawn" },
        { "surface", "Main/Game/SurfaceMap/entityspawn"},
        { "pd_start", "Main/Game/PD/PD_startroom/entityspawn" }
    };
}
