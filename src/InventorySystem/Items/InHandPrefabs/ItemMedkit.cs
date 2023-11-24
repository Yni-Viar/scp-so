using Godot;
using System;

public partial class ItemMedkit : ItemAction
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
        player.RpcId(int.Parse(player.Name), "HealthManage", 50, "Healed by medkit");
        base.OnUse(player);
    }
}
