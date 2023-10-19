using Godot;
using System;

public partial class ItemMedkit : ItemAction
{
    internal override void OnUse(PlayerScript player)
    {
        player.RpcId(int.Parse(player.Name), "HealthManage", 50);
        base.OnUse(player);
    }
}
