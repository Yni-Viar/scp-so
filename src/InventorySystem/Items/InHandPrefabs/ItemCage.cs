using Godot;
using System;

public partial class ItemCage : ItemAction
{
    RayCast3D ray;
    bool isPlayerScript = false;
    bool isFoundation = true;
    internal override void OnStart()
    {
        isPlayerScript = GetParent().GetParent().GetParentOrNull<PlayerScript>() != null;
        if (isPlayerScript)
        {
            if (GetParent().GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
            {
                ray = GetNode<RayCast3D>("RayCast3D");
            }
        }
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
    /// <summary>
    /// Base method, which is default for first-person items.
    /// </summary>
    /// <param name="player"></param>
    internal override void OnUse(PlayerScript player)
    {
        if (GetParent().GetParent().GetParent<PlayerScript>().team == Globals.Team.CHI)
        {
            isFoundation = false;
        }
        if (ray.IsColliding())
        {
            var collidedWith = ray.GetCollider();
            GD.Print(collidedWith);
            if (collidedWith is GameOver stunned && stunned.isContainable) //contain the SCPs!
            {
                if (stunned.Name.ToString().Contains("scp"))
                {
                    GetTree().Root.GetNode<PlayerAction>("Main/Game/PlayerAction").Rpc("SpawnObject", "cage_contained", 0, Multiplayer.GetUniqueId());
                    stunned.Rpc("Despawn");
                    GetParent().GetParent().GetParent().GetParent<FacilityManager>().tickets[isFoundation ? 1 : 2]++;
                    base.OnUse(player);
                }
            }
        }
    }
}
