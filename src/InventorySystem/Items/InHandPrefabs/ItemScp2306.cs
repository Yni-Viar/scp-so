using Godot;
using System;
/// <summary>
/// SCP-2306 item script.
/// </summary>
public partial class ItemScp2306 : ItemAction
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
        if (player.GetNode<RayCast3D>("PlayerHead/RayCast3D").IsColliding())
        {
            if (player.GetNode<RayCast3D>("PlayerHead/RayCast3D").GetCollider() is Scp2522Recontain recontain)
            {
                if (Multiplayer.IsServer())
                {
                    recontain.Call("Insert");
                }
                else
                {
                    recontain.RpcId(1, "Insert");
                }
                base.OnUse(player);
            }
        }
    }
}
