using Godot;
using System;

public partial class ItemScp018 : ItemAction
{
    internal override void OnUse(PlayerScript player)
    {
        Rpc("SpawnProjectile", player.GetPath());
        base.OnUse(player);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void SpawnProjectile(string playerPath)
    {
        Scp018Projectile scp = ResourceLoader.Load<PackedScene>("res://InventorySystem/Items/Projectiles/scp018projectile.tscn").Instantiate<Scp018Projectile>();
        scp.Position = GetNode<Marker3D>(playerPath + "/PlayerHead/ItemSpawn").GlobalPosition;
        GetTree().Root.GetNode<Node3D>("Main/Game/Items").AddChild(scp);
    }
}
