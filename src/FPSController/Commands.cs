using Godot;
using System;

public partial class Commands : Node
{
    static uint itemLimit = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("forceclass", new Callable(this, "Forceclass"), "Forceclass the player (needed the number of class, unlike previous versions)");
            GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("classlist", new Callable(this, "ClassList"), "Returns class names (for forceclass)");
            GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("tp", new Callable(this, "TeleportCmd"), "Teleports you. (1 arguments needed)");
            GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("itemlist", new Callable(this, "ItemList"), "Returns item names");
            GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("give", new Callable(this, "GiveItem"), "Gives an item to inventory (needed the number of item, unlike previous versions)");
            GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("giveammo", new Callable(this, "GiveAmmo"), "Gives an ammo to inventory (needed the number of ammo, unlike previous versions)");
            GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("givehp", new Callable(this, "SetHealth"), "Sets health on a current player");
            GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("givesanity", new Callable(this, "GiveSanity"), "Sets sanity on a current player");
            GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("roundstart", new Callable(this, "RoundStart"), "Starts round immediately");
            //GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("npclist", new Callable(this, "NpcList"), "Available NPCs");
            //GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("ammotypes", new Callable(this, "AmmoType"), "Available ammo types");
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    /// <summary>
    /// GDSh command. Calls helper CallForceclass() method to change the player class.
    /// </summary>
    /// <param name="args">Player class to become</param>
    /// <returns>Success or failure for changing the player class</returns>
    string Forceclass(string[] args)
    {
        if (args.Length == 1 && int.TryParse(args[0], out int classId))
        {
            if (classId < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Classes.Count && classId >= 0)
            {
                GetTree().Root.GetNode<PlayerAction>("Main/Game/PlayerAction").CallForceclass(classId, "Forceclass.");
                return "Forceclassed to " + args[0];
            }
            else
            {
                return "Wrong number. E.g. to spawn as Scientist, you need to write \"forceclass 1\"";
            }
        }
        else
        {
            return "You need ONLY 1 argument (number) to forceclass.";
        }
    }

    /// <summary>
    /// GDSh command. Calls helper CallTeleport() method to teleport player to the point.
    /// </summary>
    /// <param name="args">Place for teleporting</param>
    /// <returns>If the place exist, and args == 1, teleports player. If is unknown arg, - display all places for teleporting</returns>
    string TeleportCmd(string[] args)
    {
        if (args.Length == 1)
        {
            if (PlacesForTeleporting.defaultData.ContainsKey(args[0]))
            {
                GetTree().Root.GetNode<PlayerAction>("Main/Game/PlayerAction").CallTeleport(args[0]);
                return "Teleported to: " + args[0];
            }
            else
            {
                string r = "Wrong place. Available places for teleporting: \n";
                foreach (string key in PlacesForTeleporting.defaultData.Keys)
                {
                    r += key + "\n";
                }
                return r;
            }
        }
        else
        {
            return "You need ONLY 1 argument to teleport.";
        }
    }
    /// <summary>
    /// GDSh command. Reworked in 0.8.0-dev
    /// </summary>
    /// <param name="args">Necessary by GDsh but not used</param>
    /// <returns>List of classes, available ingame</returns>
    string ClassList(string[] args)
    {
        int i = 0;
        string r = "";
        
        foreach (BaseClass val in GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Classes)
        {
            r += i + " - " + val.ClassName + "\n";
            i++;
        }
        return r;
    }

    /// <summary>
    /// GDSh command. Reworked in 0.8.0-dev
    /// </summary>
    /// <param name="args">Necessary by GDsh but not used</param>
    /// <returns>Shows all available items</returns>
    string ItemList(string[] args)
    {
        int i = 0;
        string r = "";

        foreach (Item val in GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Items)
        {
            r += i + " - " + val.Name + "\n";
            i++;
        }
        return r;
    }
    /// <summary>
    /// GDSh command.
    /// </summary>
    /// <param name="args">Name of the item</param>
    /// <returns>Calls helper class for giving item, if it exists in itemlist.json</returns>
    string GiveItem(string[] args)
    {
        if (itemLimit > GetTree().Root.GetNode<FacilityManager>("Main/Game/").maxSpawnableObjects)
        {
            return "Error! You exceeded limit for a single player";
        }
        if (args.Length == 1 && int.TryParse(args[0], out int itemId))
        {
            if (itemId < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Items.Count && itemId >= 0)
            {
                GiveItemCmd(int.Parse(args[0]), 0);
                itemLimit++;
                return "Item " + args[0] + " spawned next to you";
            }
            else
            {
                return "Unknown item. Cannot spawn. E.g. to spawn Keycard Omni, you need to write \"give 5\"";
            }
        }
        else
        {
            return "Unknown item. Cannot spawn. Did you input the number of item?";
        }
    }
    
    /// <summary>
    /// Gives an object to this player, calling ItemManager.
    /// </summary>
    /// <param name="key">Object ID</param>
    /// <param name="type">Type of object. 0 is item, 1 is ammo, 2 is NPC</param>
    void GiveItemCmd(int key, int type)
    {
        switch (type)
        {
            case 0:
                if (key < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Items.Count && key >= 0)
                {
                    GetParent().GetParent().GetNode<ItemManager>("Items").RpcId(1, "CallAddOrRemoveItem", true, key, GetTree().Root.GetNode<Node3D>("Main/Game/" + Multiplayer.GetUniqueId() + "/PlayerHead/ItemSpawn").GetPath());
                }
                else
                {
                    GD.Print("Error. Could not spawn an item.");
                }
                break;
            case 1:
                if (key < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Ammo.Count && key >= 0)
                {
                    GetParent().GetParent().GetNode<ItemManager>("Ammo").RpcId(1, "CallAddOrRemoveItem", true, key, GetTree().Root.GetNode<Node3D>("Main/Game/" + Multiplayer.GetUniqueId() + "/PlayerHead/ItemSpawn").GetPath());
                }
                else
                {
                    GD.Print("Error. Could not spawn ammo.");
                }
                break;
            case 2:
                if (key < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Npc.Count && key >= 0)
                {
                    GetParent().GetParent().GetNode<ItemManager>("NPCs").RpcId(1, "CallAddOrRemoveItem", true, key, GetTree().Root.GetNode<Node3D>("Main/Game/" + Multiplayer.GetUniqueId() + "/PlayerHead/ItemSpawn").GetPath());
                }
                else
                {
                    GD.Print("Error. Could not spawn an NPC.");
                }
                break;
            default:
                GD.Print("Error. Could not parse type of object. Please, use correct parameters");
                break;
        }
    }
    /// <summary>
    /// Sets health on a current player
    /// </summary>
    /// <param name="args">How much HP is given</param>
    /// <returns>Result</returns>
    string SetHealth(string[] args)
    {
        if (args.Length == 1)
        {
            GetParent<PlayerScript>().RpcId(Multiplayer.GetUniqueId(), "HealthManage", Convert.ToDouble(args[0]), "Forced health change.", 0);
            return "Given " + args[0] + " health (if is possible).";
        }
        else
        {
            return "Error. You need only 1 argument, e.g. givehp 100";
        }
    }
    /// <summary>
    /// Gives ammo for the player. Available, since 0.7.0-dev.
    /// </summary>
    /// <param name="args">Ammo name, as in ammotype_[currentversion].json or as in Globals.ammo, if settings are default.</param>
    /// <returns>Result</returns>
    string GiveAmmo(string[] args)
    {
        if (itemLimit > GetTree().Root.GetNode<FacilityManager>("Main/Game/").maxSpawnableObjects)
        {
            return "Error! You exceeded limit for a single player";
        }
        if (args.Length == 1 && int.TryParse(args[0], out int itemId))
        {
            if (itemId < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Ammo.Count && itemId >= 0)
            {
                GiveItemCmd(int.Parse(args[0]), 1);
                itemLimit++;
                return "Ammo " + args[0] + " spawned next to you";
            }
            else
            {
                return "Unknown ammo. Cannot spawn. E.g. to spawn pistol ammo, you need to write \"giveammo 0\"";
            }
        }
        else
        {
            return "Unknown ammo. Cannot spawn. Did you input the number of ammo type?";
        }
    }
    /// <summary>
    /// Gives sanity to the player. Available since 0.7.0-RC.
    /// </summary>
    /// <param name="args">How much Sanity is given</param>
    /// <returns>Result</returns>
    string GiveSanity(string[] args)
    {
        if (args.Length == 1)
        {
            GetParent<PlayerScript>().RpcId(Multiplayer.GetUniqueId(), "SanityManage", Convert.ToDouble(args[0]), "Forced sanity change.", 1);
            return "Given " + args[0] + " sanity (if is possible).";
        }
        else
        {
            return "Error. You need only 1 argument, e.g. givesanity 100";
        }
    }
    /// <summary>
    /// Forces round start. Available since 0.7.2-dev
    /// </summary>
    /// <param name="args">Necessary by GDsh but not used</param>
    /// <returns>A result message</returns>
    string RoundStart(string[] args)
    {
        GetTree().Root.GetNode<FacilityManager>("Main/Game").Call("ForceRoundStart");
        return "Starting the round... (If round is already started, nothing happens)";
    }
    /* 0.7.x content, that was deprecated at 0.8.0.
    /// <summary>
    /// GDSh command.
    /// </summary>
    /// <param name="args">Necessary by GDsh but not used</param>
    /// <returns>All available ammo types</returns>
    string AmmoType(string[] args)
    {
        string r = "";
        foreach (var val in ItemParser.ReadJson("user://ammotype_" + Globals.itemsCompatibility + ".json", Globals.ItemType.item))
        {
            r += val.Key + "\n";
        }
        return r;
    }
    /// <summary>
    /// GDSh command.
    /// </summary>
    /// <param name="args">Necessary by GDsh but not used</param>
    /// <returns>All available NPCs</returns>
    string NpcList(string[] args)
    {
        string r = "";
        foreach (var val in ItemParser.ReadJson("user://npcs_" + Globals.itemsCompatibility + ".json", Globals.ItemType.item))
        {
            r += val.Key + "\n";
        }
        return r;
    }
    */
}
