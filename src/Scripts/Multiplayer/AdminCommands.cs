using Godot;
using System;
using System.Threading.Tasks;

public partial class AdminCommands : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("admin", new Callable(this, "AdminGrant"), "Grants admin privilegies (1 argument - password)");
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("admin_ban", new Callable(this, "AdminBan"), "Bans a player (1 argument - peer id)");
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("admin_kick", new Callable(this, "AdminKick"), "Kick a player (1 argument - peer id)");
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("get_player_peers", new Callable(this, "GetPeers"), "Gets players peers");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    // Bugged admin grant.
    /*string AdminGrant(string[] args)
    {
        if (string.IsNullOrEmpty(args[0]))
        {
            return "Provide a password";
        }
        else
        {
            GetTree().Root.GetNode<PlayerScript>("Main/Game/" + Multiplayer.GetUniqueId()).RpcId(1, "GrantAdminPrivilegies", true, args[0]);
            return "...";
        }
    }*/
    /// <summary>
    /// GDSh command, which bans a player.
    /// </summary>
    /// <param name="args">First arg is admin password (don't mistake for admin panel (or moderator) password, which grants GDSh access). Second arg is name of the peer</param>
    /// <returns>Success</returns>
    string AdminBan(string[] args)
    {
        if (/*GetTree().Root.GetNode<PlayerScript>("Main/Game/" + Multiplayer.GetUniqueId()).IsAdmin*/ Multiplayer.IsServer())
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                return "Provide a peer's id. You can find it in get_player_peers command";
            }
            if (args[0] == "1")
            {
                return "You cannot ban the server";
            }
            string peerIp = GetTree().Root.GetNode<NetworkManager>("Main").GetPeer(int.Parse(args[0]));
            GetTree().Root.GetNode<NetworkManager>("Main").RpcId(int.Parse(args[0]), "Kick");
            RpcId(1, "AddDetentionNote", peerIp);
            return "Successfully banned!";
        }
        else
        {
            return "You are not granted to do this action";
        }
    }
    /// <summary>
    /// GDSh command, which kicks a player.
    /// </summary>
    /// <param name="args">First arg is admin password (don't mistake for admin panel (or moderator) password, which grants GDSh access). Second arg is name of the peer</param>
    /// <returns>Success</returns>
    string AdminKick(string[] args)
    {
        if (/*GetTree().Root.GetNode<PlayerScript>("Main/Game/" + Multiplayer.GetUniqueId()).IsAdmin*/ Multiplayer.IsServer())
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                return "Provide a peer's id. You can find it in get_player_peers command";
            }
            if (args[0] == "1")
            {
                return "You cannot kick the server";
            }
            string peerIp = GetTree().Root.GetNode<NetworkManager>("Main").GetPeer(int.Parse(args[0]));
            GetTree().Root.GetNode<NetworkManager>("Main").RpcId(int.Parse(args[1]), "Kick");
            return "Successfully kicked!";
        }
        else
        {
            return "You are not granted to do this action";
        }
    }
    /// <summary>
    /// GDSh command, which helps admins to find rule breakers and cheaters.
    /// </summary>
    /// <param name="args">Necessary by GDsh but not used</param>
    /// <returns>List of peers and players</returns>
    string GetPeers(string[] args)
    {
        string s = "";
        foreach (Node peer in GetParent<FacilityManager>().GetChildren())
        {
            if (peer is PlayerScript player)
            {
                s += player.Name + " - " + player.playerName + "\n";
            }
        }
        return s;
    }
    /// <summary>
    /// Ban helper method, adds banned IP to the list.
    /// </summary>
    /// <param name="ip"></param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    void AddDetentionNote(string ip)
    {
        string s = TxtParser.Load("user://ipbans.txt");
        s += "\n" + ip;
        TxtParser.Save("user://ipbans.txt", s);
    }
}
