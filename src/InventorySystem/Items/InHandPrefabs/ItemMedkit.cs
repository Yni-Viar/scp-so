using Godot;
using System;

public partial class ItemMedkit : ItemAction
{
    bool isPlayerScript = false;
    internal override void OnStart()
    {
        isPlayerScript = GetParent().GetParent().GetParentOrNull<PlayerScript>() != null;
    }

    internal override void OnUpdate(double delta)
    {
        if (isPlayerScript)
        {
            if (Input.IsActionJustPressed("fire"))
            {
                OnUse(GetParent().GetParent().GetParent<PlayerScript>());
            }
        }
    }
    internal override void OnUse(PlayerScript player)
    {
        player.RpcId(int.Parse(player.Name), "HealthManage", 50, "Healed by medkit", 0);
        base.OnUse(player);
    }
}
