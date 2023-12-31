using Godot;
using System;
/// <summary>
/// Weapon Manager.
/// THIS CODE WAS WRITTEN BY HUBERT MOSZKA FOR SCP: SECRET LAB v.6.0.0.
/// Author's commentary:
/// This stream is licensed under the CC-BY-SA 3.0 license
/// 
/// If stream is licensed under this license, so the code is also licensed by this license (I think...)
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
    [Export] float weaponCooldown;
    float fireCooldown;
    [Export] bool isTranquilizer = false;

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
    [Export] Godot.Collections.Array<AudioStream> poofSounds = new Godot.Collections.Array<AudioStream>();
    [Export] AudioStream reloadSound;
    Camera3D camera;
    AudioStreamPlayer3D audio;
    RayCast3D rayCast;
    AnimationPlayer anim;
    bool isPlayerScript = false; //checks if this is first-person model.
    

    internal override void OnStart()
    {
        recoilTarget = new Vector3 { X = rng.RandiRange(-20, 20), Y = rng.RandiRange(0, 70), Z = rng.RandiRange(-20, 20)};
        audio = GetNode<AudioStreamPlayer3D>("WeaponSound");
        isPlayerScript = GetParent().GetParent().GetParentOrNull<PlayerScript>() != null;
        if (isPlayerScript)
        {
            if (GetParent().GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
            {
                camera = GetParent().GetParent().GetNode<Camera3D>("PlayerCamera");
                rayCast = GetNode<RayCast3D>("ShootingRange");
            }
        }
    }

    internal override void OnUpdate(double delta)
    {
        if (isPlayerScript)
        {
            if (GetParent().GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
            {
                if (isAuto ? Input.IsActionPressed("weapon_shoot") : Input.IsActionJustPressed("weapon_shoot"))
                {
                    if (IsDeductCooldownEnded())
                    {
                        Shoot();
                    }
                }
                if (Input.IsActionJustPressed("weapon_zoom") && allowZoom)
                {
                    UpdateFov();
                }
                if (Input.IsActionJustPressed("weapon_reload") && GetParent().GetParent().GetParent<PlayerScript>().GetNode<AmmoSystem>("AmmoSystem").ammo[ammoType] > 0)
                {
                    Rpc("Reload");
                }
                if (fireCooldown > 0)
                {
                    fireCooldown -= (float)delta;
                }
            }
        }
    }
    /// <summary>
    /// Spawn bullet shot (if is not a player)
    /// </summary>
    /// <param name="point">Point, where bullet collided</param>
    /// <param name="normal">Normal of the point (for rotating the bullet to player)</param>
    void SpawnDecal(Vector3 point, Vector3 normal)
    {
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
    /// <summary>
    /// The main shooting mechanic (singleplayer)
    /// </summary>
    void Shoot()
    {
        if (rayCast.IsColliding() && GetParent().GetParent().GetParent<PlayerScript>().GetNode<AmmoSystem>("AmmoSystem").CurrentAmmo[ammoType] > 0)
        {
            Rpc("ShootRpc");
            var collider = rayCast.GetCollider();
            if (collider is PlayerScript playerScript && playerScript.Name != Multiplayer.GetUniqueId().ToString())
            {
                if (isTranquilizer)
                {
                    //Stun
                    playerScript.RpcId(int.Parse(playerScript.Name), "SanityManage", -damage / 2, "Recontained by " + Name);
                }
                else
                {
                    //Deplete HP
                    if (playerScript.classKey == "scp106")
                    {
                        playerScript.RpcId(int.Parse(playerScript.Name), "HealthManage", -damage / 24, "Shot by " + Name);
                    }
                    else
                    {
                        playerScript.RpcId(int.Parse(playerScript.Name), "HealthManage", -damage / 4 /*GlobalPosition.DistanceTo(playerScript.GlobalPosition)*/, "Shot by " + Name);
                    }
                }
            }
            else
            {
                SpawnDecal(rayCast.GetCollisionPoint() + rayCast.GetCollisionNormal() * 0.9f, rayCast.GetCollisionNormal()); //Hit point
            }
        }
        
    }
    /// <summary>
    /// Networked part of shooting. Available since 0.7.0
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void ShootRpc()
    {
        //Minus one bullet
        GetParent().GetParent().GetParent<PlayerScript>().GetNode<AmmoSystem>("AmmoSystem").CurrentAmmo[ammoType]--;
        //Play animation and audio
        audio.Stream = poofSounds[rng.RandiRange(0, poofSounds.Count - 1)];
        audio.Play();

        //fire cooldown
        fireCooldown = weaponCooldown;

        //Recoil
        recoilTarget = new Vector3 { X = rng.RandiRange(-20, 20), Y = 0, Z = 0 };
        Vector3 recoilVector = recoilTarget.Lerp(Vector3.Zero, recoilScale);
        Vector3 finalizeRecoil = GetParent().GetParent().GetParent<PlayerScript>().GetNode<Node3D>("PlayerHead").RotationDegrees.Lerp(recoilVector, recoilSpeed);
        GetParent().GetParent().GetParent<PlayerScript>().GetNode<Node3D>("PlayerHead").RotationDegrees += new Vector3(Mathf.Clamp(finalizeRecoil.X, Mathf.DegToRad(-85f), Mathf.DegToRad(85f)), 0, 0);
        rayCast.RotationDegrees = recoilTarget;
    }
    /// <summary>
    /// Reloading mechanic.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void Reload()
    {
        //The main reloading system.
        GetParent().GetParent().GetParent<PlayerScript>().GetNode<AmmoSystem>("AmmoSystem").ReloadAmmo(ammoType, maxAmmo);
        //Sounds
        if (GetParent().GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            SetState("reload");
        }
        if (reloadSound != null)
        {
            audio.Stream = reloadSound;
            audio.Play();
        }
    }
    /// <summary>
    /// Check fire cooldown
    /// </summary>
    /// <returns>If is cooldown - then false, else true</returns>
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
    /// <summary>
    /// Changes player FOV
    /// </summary>
    void UpdateFov()
    {
        camera.Fov = Mathf.Lerp(zoomed ? zoomFov : camera.Fov, zoomed ? camera.Fov : zoomFov, fovAjustingSpeed);
        zoomed = !zoomed;
    }
    /// <summary>
    /// Set animation to a weapon.
    /// </summary>
    /// <param name="s">Animation name</param>
    private void SetState(string s)
    {
        if (GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation != s || !GetNode<AnimationPlayer>("AnimationPlayer").IsPlaying())
        {
            //Change the animation.
            GetNode<AnimationPlayer>("AnimationPlayer").Play(s);
        }
    }
}
