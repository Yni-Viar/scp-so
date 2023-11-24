using Godot;
using System;

public partial class ItemScp018 : ItemAction
{
    internal override void OnUpdate(double delta)
    {
        if (Input.IsActionJustPressed("fire"))
        {
            OnUse(GetParent().GetParent().GetParent<PlayerScript>());
        }
    }
    internal override void OnUse(PlayerScript player)
    {
        Rpc("SpawnProjectile", player.GetPath());
        base.OnUse(player);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void SpawnProjectile(string playerPath)
    {
        BallProjectile scp = ResourceLoader.Load<PackedScene>("res://InventorySystem/Items/Projectiles/scp018projectile.tscn").Instantiate<BallProjectile>();
        scp.Position = GetNode<Marker3D>(playerPath + "/PlayerHead/ItemSpawn").GlobalPosition;
        GetTree().Root.GetNode<Node3D>("Main/Game/Items").AddChild(scp);
    }
}
