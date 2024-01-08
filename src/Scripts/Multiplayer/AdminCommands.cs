using Godot;
using System;

public partial class AdminCommands : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("admin_logon", new Callable(this, "AdminGrant"), "Grants admin privilegies (1 argument - password)");
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("admin_ban", new Callable(this, "AdminBan"), "Bans a player (1 argument - peer id)");
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("admin_kick", new Callable(this, "AdminKick"), "Kick a player (1 argument - peer id)");
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("get_player_peers", new Callable(this, "GetPeers"), "Gets players peers");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    // Bugged admin grant.
    string AdminGrant(string[] args)
    {
        if (string.IsNullOrEmpty(args[0]))
        {
            return "Provide a password";
        }
        else
        {
            AskForAdminConnector(args[0]);
            return "...";
        }
    }
    /// <summary>
    /// GDSh command, which bans a player.
    /// </summary>
    /// <param name="args">First arg is admin password (don't mistake for admin panel (or moderator) password, which grants GDSh access). Second arg is name of the peer</param>
    /// <returns>Success</returns>
    string AdminBan(string[] args)
    {
        if (GetTree().Root.GetNode<PlayerScript>("Main/Game/" + Multiplayer.GetUniqueId()).IsAdmin)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                return "Provide a peer's id. You can find it in get_player_peers command";
            }
            if (args[0] == "1")
            {
                return "You cannot ban the server";
            }
            GetTree().Root.GetNode<NetworkManager>("Main").Call("Ban", args[0]);
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
        if (GetTree().Root.GetNode<PlayerScript>("Main/Game/" + Multiplayer.GetUniqueId()).IsAdmin)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                return "Provide a peer's id. You can find it in get_player_peers command";
            }
            if (args[0] == "1")
            {
                return "You cannot kick the server";
            }
            GetTree().Root.GetNode<NetworkManager>("Main").RpcId(int.Parse(args[0]), "Kick");
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
    /// Server-side RPC method, checks admin password.
    /// </summary>
    /// <param name="peerId">Id of peer, who called</param>
    /// <param name="password">Admin password</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    void AskForAdmin(int peerId, string password)
    {
        if (GD.Hash(password) == GetTree().Root.GetNode<NetworkManager>("Main").GetAdmin)
        {
            GetParent().GetNode<PlayerScript>(peerId.ToString()).RpcId(peerId, "GrantAdminPrivilegies");
        }
    }

    void AskForAdminConnector(string password)
    {
        RpcId(1, "AskForAdmin", Multiplayer.GetUniqueId(), password);
    }

    /// <summary>
    /// Asks for moderator privilegies.
    /// </summary>
    /// <param name="id">Player ID</param>
    /// <param name="password">Given password</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    void AskForModeratorPrivilegies(int id, string password)
    {
        if (GD.Hash(password) == GetTree().Root.GetNode<NetworkManager>("Main").GetModerator)
        {
            GetParent().GetNode<PlayerScript>(id.ToString()).RpcId(id, "GrantModeratorPrivilegies");
        }
    }
}
