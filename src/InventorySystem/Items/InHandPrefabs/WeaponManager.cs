using Godot;
using System;
/// <summary>
/// Weapon Manager.
/// THIS CODE WAS WRITTEN BY HUBERT MOSZKA FOR SCP: SECRET LAB v.6.0.0.
/// Author's commentary:
/// This stream is licensed under the CC-BY-SA 3.0 license
/// 
/// If stream is licensed under this license, so the code is also lice3nsed by this license (I think...)
/// </summary>
public partial class WeaponManager : ItemAction
{
    RandomNumberGenerator rng = new RandomNumberGenerator();

    //generic weapon properties
    [Export] int maxAmmo;
    [Export] int ammoType;
    [Export] float damage;
    [Export] float reloadingTime;
    [Export] bool isAuto;
    [Export] float shotsPerSecond;
    float fireCooldown;

    //Zooming
    [Export] bool allowZoom;
    [Export] float zoomFov = 80;
    //[Export] float targetFov;
    [Export] float fovAjustingSpeed;
    bool zoomed = false;

    //Recoil
    [Export] float recoilScale;
    [Export] float recoilSpeed;
    Vector3 recoilTarget;

    //Godot-specific variables
    [Export] string bulletDecalPath = "res://Decals/shot.tscn";
    [Export] Godot.Collections.Array<AudioStream> poofSounds = new Godot.Collections.Array<AudioStream>();
    [Export] AudioStream reloadSound;
    Camera3D camera;
    AudioStreamPlayer3D audio;
    RayCast3D rayCast;
    

    internal override void OnStart()
    {
        recoilTarget = new Vector3 { X = rng.RandiRange(-20, 20), Y = rng.RandiRange(0, 70), Z = rng.RandiRange(-20, 20)};
        audio = GetNode<AudioStreamPlayer3D>("WeaponSound");
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
                audio.Stream = poofSounds[rng.RandiRange(0, poofSounds.Count)];
                audio.Play();
                // do animation (in future)
            }
            else
            {
                SpawnDecal(rayCast.GetCollisionPoint() + rayCast.GetCollisionNormal() * 0.9f, rayCast.GetCollisionNormal()); //Hit point
            }
        }
        Vector3 recoilVector = Vector3.Zero.Lerp(recoilTarget, recoilScale);
        Vector3 recoilResult = GetParent().GetParent().GetParent<PlayerScript>().GetNode<Node3D>("PlayerHead").Rotation.Lerp(recoilVector, recoilSpeed);
    }

    //[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void Reload()
    {
        audio.Stream = reloadSound;
        audio.Play();
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
