using Godot;
using System;

public partial class Commands : Node
{
    static byte itemLimit = 0;
    byte itemMax = 12; //it will be configurable in future.
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("forceclass", new Callable(this, "Forceclass"), "Forceclass the player (1 argument needed)");
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("classlist", new Callable(this, "ClassList"), "Returns class names (for forceclass)");
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("tp", new Callable(this, "TeleportCmd"), "Teleports you. (1 arguments needed)");
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("itemlist", new Callable(this, "ItemList"), "Returns item names");
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("give", new Callable(this, "GiveItem"), "Gives an item to inventory (Limit for each player equals 12 items)");
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
        if (args.Length == 1 && ResourceLoader.Exists("res://FPSController/PlayerClassResources/" + args[0] + ".tres"))
        {
            GetParent<PlayerScript>().CallForceclass(args[0]);
            return "Forceclassed to " + args[0];
        }
        else
        {
            return "You need ONLY 1 argument to forceclass. E.g. to spawn as SCP-173, you need to write \"forceclass scp173\"";
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
                GetParent<PlayerScript>().CallTeleport(args[0]);
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
    /// GDSh command.
    /// </summary>
    /// <param name="args">Necessary by GDsh but not used</param>
    /// <returns>List of classes, available ingame</returns>
    string ClassList(string[] args)
    {
        Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> classes = ClassParser.ReadJson("user://playerclasses.json");
        string[] groups = new string[] { "spawnableHuman", "arrivingHuman", "spawnableScps" };
        string r = "";
        for (int i = 0; i < groups.Length; i++)
        {
            foreach (var val in classes[groups[i]])
            {
                r += val + "\n";
            }
        }
        
        return r;
    }
    /// <summary>
    /// GDSh command.
    /// </summary>
    /// <param name="args">Necessary by GDsh but not used</param>
    /// <returns>Shows all available items</returns>
    string ItemList(string[] args)
    {
        string r = "";
        foreach (var val in ItemParser.ReadJson("user://itemlist.json"))
        {
            r += val.Key + "\n";
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
        if (itemLimit > itemMax)
        {
            return "Error! You exceeded limit for a single player";
        }
        if (args.Length == 1)
        {
            if (ItemParser.ReadJson("user://itemlist.json").ContainsKey(args[0]))
            {
                //inventory.AddItem(ResourceLoader.Load(JsonParser.ReadJson("user://itemlist.json")[args[0]]));
                //gives the string for adding an item, rpc will convert this in resource
                GiveItemCmd(ItemParser.ReadJson("user://itemlist.json")[args[0]]);
                itemLimit++;
                return "Item " + args[0] + " spawned next to you";
            }
            else
            {
                return "Unknown item. Cannot spawn.";
            }
        }
        else
        {
            return "Unknown item. Cannot spawn. Did you input the item?";
        }
    }
    /// <summary>
    /// Gives item
    /// </summary>
    /// <param name="itemPath">Path to item resource</param>
    void GiveItemCmd(string itemPath)
    {
        Item item = ResourceLoader.Load<Item>(itemPath);
        GetTree().Root.GetNode<Inventory>("Main/Game/" + Multiplayer.GetUniqueId() + "/InventoryContainer/Inventory").Rpc("DropItemRpc", item.PickablePath);
    }
}
