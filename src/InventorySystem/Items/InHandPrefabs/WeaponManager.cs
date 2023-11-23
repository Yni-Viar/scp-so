using Godot;
using System;

public partial class WeaponManager : ItemAction
{
    [Export] string bulletDecalPath = "res://Decals/shot.tscn";
    [Export] float recoilScale;
    [Export] int maxAmmo;
    [Export] int ammoType;
    [Export] float damage;
    [Export] float reloadingTime;
    [Export] bool isAuto;
    [Export] bool allowZoom;
    [Export] float zoomFov = 80;
    //[Export] float targetFov;
    [Export] float fovAjustingSpeed;
    bool zoomed = false;
    Camera3D camera;
    [Export] float shotsPerSecond;
    float fireCooldown;
    RayCast3D rayCast;
    internal override void OnStart()
    {
        if (GetParent().GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            camera = GetParent().GetParent().GetNode<Camera3D>("PlayerCamera");
            rayCast = GetNode<RayCast3D>("ShootingRange");
        }
    }

    internal override void OnUpdate(double delta)
    {
        if (GetParent().GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            if (isAuto ? Input.IsActionPressed("weapon_shoot") : Input.IsActionJustPressed("weapon_shoot"))
            {
                if (IsDeductCooldownEnded())
                {
                    Rpc("Shoot");
                }
            }
            if (Input.IsActionJustPressed("weapon_zoom") && allowZoom)
            {
                UpdateFov();
            }
            if (fireCooldown > 0)
            {
                fireCooldown -= (float)delta;
            }
        }
    }

    void SpawnDecal(Vector3 point, Vector3 normal)
    {
        CpuParticles3D decal = ResourceLoader.Load<PackedScene>(bulletDecalPath).Instantiate<CpuParticles3D>();
        GetTree().Root.GetNode<Node3D>("Main/Game/Decals/").AddChild(decal);
        decal.GlobalPosition = point;
        if (normal == Vector3.Down)
        {
            decal.LookAt(normal, Vector3.Down);
        }
        else
        {
            decal.LookAt(normal);
        }
        CpuParticles3D smokeDecal = ResourceLoader.Load<PackedScene>("res://Decals/smoke.tscn").Instantiate<CpuParticles3D>();
        GetTree().Root.GetNode<Node3D>("Main/Game/Decals/").AddChild(smokeDecal);
        smokeDecal.GlobalPosition = point;
        if (normal == Vector3.Down)
        {
            smokeDecal.LookAt(normal, Vector3.Down);
        }
        else
        {
            smokeDecal.LookAt(normal);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void Shoot()
    {
        if (rayCast.IsColliding())
        {
            var collider = rayCast.GetCollider();
            GD.Print(rayCast.GetCollider());
            if (collider is PlayerScript playerScript)
            {
                if (playerScript.classKey == "scp106")
                {
                    playerScript.RpcId(int.Parse(playerScript.Name), "HealthManage", -damage / GlobalPosition.DistanceTo(playerScript.GlobalPosition) / 10, "Shot by " + Name);
                }
                else
                {
                    playerScript.RpcId(int.Parse(playerScript.Name), "HealthManage", -damage / GlobalPosition.DistanceTo(playerScript.GlobalPosition), "Shot by " + Name);
                }
            }
            else
            {
                SpawnDecal(rayCast.GetCollisionPoint() + rayCast.GetCollisionNormal() * 0.9f, rayCast.GetCollisionNormal()); //Hit point
            }
        }
    }

    bool IsDeductCooldownEnded()
    {
        if (fireCooldown > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    internal override void OnUse(PlayerScript player)
    {
        base.OnUse(player);
    }

    void UpdateFov()
    {
        camera.Fov = Mathf.Lerp(zoomed ? zoomFov : camera.Fov, zoomed ? camera.Fov : zoomFov, fovAjustingSpeed);
        zoomed = !zoomed;
    }
}
