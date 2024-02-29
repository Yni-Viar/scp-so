using Godot;
using System;

public class PlacesForTeleporting
{
    internal static Godot.Collections.Dictionary<string, string> defaultData = new Godot.Collections.Dictionary<string, string> {
        { "surface", "Main/Game/SurfaceMap/entityspawn"},
        { "rz_offices", "Main/Game/MapGenRz/RZ_room2_offices/entityspawn" },
        { "rz_offices2", "Main/Game/MapGenRz/RZ_room2_offices2/entityspawn" },
        { "rz_servers2", "Main/Game/MapGenRz/RZ_room2_servers/entityspawn" },
        { "rz_cafeteria", "Main/Game/MapGenRz/RZ_room2_cafeteria/entityspawn" },
        { "rz_cafeteria_lower", "Main/Game/MapGenRz/RZ_room2_cafeteria/entityspawn2" },
        { "rz_toilets", "Main/Game/MapGenRz/RZ_room2_toilets/entityspawn" },
        { "rz_poffices", "Main/Game/MapGenRz/RZ_room2_poffices/entityspawn" }, 
        { "lcz_archive", "Main/Game/MapGenLcz/LC_room1_archive/entityspawn" },
        { "lcz_securityroom", "Main/Game/MapGenLcz/LC_room2_sl/entityspawn" },
        { "lcz_scpitems", "Main/Game/MapGenLcz/LC_cont2_scps/entityspawn" },
        { "lcz_079cont", "Main/Game/MapGenLcz/LC_cont1_079/entityspawn2" },
        { "lcz_079spawn", "Main/Game/MapGenLcz/LC_cont1_079/entityspawn" },
        { "lcz_650spawn", "Main/Game/MapGenLcz/LC_cont2_650/entityspawn" },
        { "hcz_173spawn", "Main/Game/MapGenHcz/HC_cont1_173/entityspawn" },
        { "hcz_testroom", "Main/Game/MapGenHcz/HC_cont2_testroom/entityspawn" },
        { "hcz_3199spawn", "Main/Game/MapGenHcz/HC_cont1_3199/entityspawn" },
        { "hcz_106spawn", "Main/Game/MapGenHcz/HC_cont1_106/entityspawn" },
        { "hcz_warhead", "Main/Game/MapGenHcz/HC_room2_nuke/entityspawn" },
        { "pd_start", "Main/Game/PD/PD_startroom/entityspawn" },
        { "pd_basement", "Main/Game/PD/PD_basement/entityspawn" },
    };
}
