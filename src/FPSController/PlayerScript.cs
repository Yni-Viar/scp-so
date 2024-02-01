using Godot;
using System;
/// <summary>
/// This script was originally created by dzejpi. License - The Unlicense.
/// Some parts used from elmarcoh (this script is also public domain).
/// 
/// This is base script for a player.
/// Currently the script is a big mess, so it need refactoring :(
/// </summary>
public partial class PlayerScript : CharacterBody3D
{
    [Signal]
    public delegate void ItemInHandChangedEventHandler(string itemToReplace);

    RandomNumberGenerator rng = new RandomNumberGenerator();
    Node3D playerHead;
    Settings settings;
    internal RayCast3D ray;
    internal RayCast3D watchRay;

    //Walk sounds.
    AudioStreamPlayer3D walkSounds;
    AudioStreamPlayer3D interactSound;
    internal float[] currentHealth = new float[] { 1f, 50f };

    double decayTimer = 0d;

    [Export] bool isModerator = false;
    internal bool IsModerator 
    {
        get => isModerator;
    }
    [Export] bool isAdmin = false;
    internal bool IsAdmin
    {
        get => isAdmin;
    }
    
    [Export] internal string playerName;
    [Export] internal int classKey;
    [Export] internal string className;
    [Export] internal string classDescription;
    [Export] internal string spawnPoint;
    [Export] internal string playerModelSource;
    internal float[] health = new float[] { 1f, 50f };
    [Export] internal float speed = 0f;
    [Export] internal float jump = 0f;
    [Export] internal bool sprintEnabled = false;
    [Export] internal bool moveSoundsEnabled = false;
    [Export] internal string[] footstepSounds;
    [Export] internal string[] sprintSounds;
    [Export] internal Globals.Team team;
    [Export] internal string ragdollSource;
    
    float gravity = 9.8f;
    // SCP Number. Set -1 for humans, -2 for spectators.
    [Export] internal int scpNumber = -2;

    // beginning with 0.4.0-dev, you cannot move while waiting for match. Instead, every class re-enables the movement toggle.
    [Export] bool canMove = false; 
    internal bool CanMove {get=>canMove; set=>canMove = value;}
    
    [Export] private string[] usingItem;
    internal string[] UsingItem
    {
        get => usingItem;
        set
        {
            usingItem = value;
            Rpc("UpdateItemsInHand", usingItem[0], usingItem[1]);
        }
    }

    [Export] internal float cameraPosition = 0f;
    [Export] internal bool customMusic = false;
    [Export] bool customCamera = false;
    
    float groundAcceleration = 8.0f;
    float acceleration;

    internal Vector3 dir = new Vector3();
    Vector3 vel = new Vector3();
    Vector3 movement = new Vector3();
    Vector3 gravityVector = new Vector3();

    bool isSprinting = false;
    bool isWalking = false;

    public override void _EnterTree()
    {
        SetMultiplayerAuthority(int.Parse(Name));
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Grant admin and moderator privilegies
        if (Multiplayer.IsServer())
        {
            isAdmin = isModerator = true;
        }
        // Player initialization.
        if (IsMultiplayerAuthority())
        {
            //Fixing sky glitch, if exiting the game from Surface Zone.
            GetParent().GetNode<WorldEnvironment>("WorldEnvironment").Environment.BackgroundMode = Godot.Environment.BGMode.Color;
            //Loading settings and player nickname
            settings = GetTree().Root.GetNode<Settings>("Settings");

            if (FileAccess.FileExists("user://playername.txt"))
            {
                string nick = TxtParser.Load("user://playername.txt");
                if (!string.IsNullOrEmpty(nick))
                {
                    playerName = nick;
                }
                else
                {
                    RandomNumberGenerator rng = new RandomNumberGenerator();
                    rng.Randomize();
                    playerName = "Unknown player " + rng.Randi();
                }
            }
            else
            {
                RandomNumberGenerator rng = new RandomNumberGenerator();
                rng.Randomize();
                playerName = "Unknown player " + rng.Randi();
            }

            GetNode<Camera3D>("PlayerHead/PlayerCamera").Current = true;
            playerHead = GetNode<Node3D>("PlayerHead");
            
            interactSound = GetNode<AudioStreamPlayer3D>("InteractSound");
            acceleration = groundAcceleration;
            FloorMaxAngle = 1.48353f; //85 degrees.
        }
        else
        {
            GetNode<Camera3D>("PlayerHead/PlayerCamera").Current = false;
        }
        walkSounds = GetNode<AudioStreamPlayer3D>("WalkSounds");
        ray = GetNode<RayCast3D>("PlayerHead/RayCast3D");
        ray.AddException(this);
        watchRay = GetNode<RayCast3D>("PlayerHead/VisionRadius");
        watchRay.AddException(this);
    }
    //unused
    internal bool IsLocalAuthority()
    {
        return GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    /*public override void _Process(double delta)
    {
    }*/

    public override void _Input(InputEvent @event)
    {
        if (IsMultiplayerAuthority() && !GetParent().GetNode<PlayerUI>("PlayerUI").SpecialScreen)
        {
            if (@event is InputEventMouseMotion)
            {
                InputEventMouseMotion m = (InputEventMouseMotion) @event;
                if (!customCamera)
                {
                    this.RotateY(-m.Relative.X * settings.MouseSensitivity * 0.05f); //pretty magic numbers
                    playerHead.RotateX(Mathf.Clamp(-m.Relative.Y * settings.MouseSensitivity * 0.05f, -90, 90));
    
                    Vector3 cameraRot = playerHead.Rotation;
                    cameraRot.X = Mathf.Clamp(playerHead.Rotation.X, Mathf.DegToRad(-85f), Mathf.DegToRad(85f));
                    playerHead.Rotation = cameraRot;
                }
            }

            if (Input.IsActionJustPressed("mode_kinematic"))
            {
                GetParent().GetNode<PlayerUI>("PlayerUI").Visible = !GetParent().GetNode<PlayerUI>("PlayerUI").Visible;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsMultiplayerAuthority())
        {

            //gravity
            if (IsOnFloor())
            {
                gravityVector = Vector3.Zero; //if is on floor, gravity should be 0, not previously summed value!
            }
            else
            {
                gravityVector += Vector3.Down * gravity * (float)delta;
            }

            //jumping
            if (GetNode<PlayerSync>("PlayerSync").isJumping && IsOnFloor())
            {
                gravityVector = Vector3.Up * jump;
                GetNode<PlayerSync>("PlayerSync").isJumping = false;
            }
            Vector3 direction = (Transform.Basis * new Vector3(GetNode<PlayerSync>("PlayerSync").direction.X, 0, GetNode<PlayerSync>("PlayerSync").direction.Y));
            dir = direction; // Save this for outer value, used by class player scripts, but don't reveal the "true" direction variable.
            
            if (canMove)
            {
                //running
                if (Input.IsActionPressed("move_sprint") && sprintEnabled)
                {
                    vel = vel.Lerp(direction * speed * 2, acceleration * (float)delta);
                    isSprinting = true;
                    isWalking = false;
                    
                }
                //walking
                else
                {
                    vel = vel.Lerp(direction * speed, acceleration * (float)delta);
                    isWalking = true;
                    isSprinting = false;
                    
                }

                movement.Z = vel.Z + gravityVector.Z;
                movement.X = vel.X + gravityVector.X;
                movement.Y = gravityVector.Y;
                Velocity = movement;
            }
            else
            {
                Velocity = Vector3.Zero;
            }
            
            //check if we don't stay still and is footstep audio playing;
            if (!direction.IsZeroApprox() && moveSoundsEnabled) //Deprecated in 0.5.0-dev
            {
                if (isSprinting)
                {
                    GetNode<AnimationPlayer>("AnimationPlayer").Play("Walk", -1, 2, false);
                }
                else if (isWalking)
                {
                    GetNode<AnimationPlayer>("AnimationPlayer").Play("Walk");
                }

                else if (direction.IsZeroApprox())
                {
                    GetNode<AnimationPlayer>("AnimationPlayer").Stop();
                }
            }
            

            //Interacting. Please, check spectators to not become ghosts ;)
            if (ray.IsColliding() && Input.IsActionJustPressed("interact") && scpNumber != -2)
            {
                var collidedWith = ray.GetCollider();
                if (collidedWith is InteractableCommon)
                {
                    collidedWith.Call("Interact", this);
                }
                /* old button scripts from 0.7.x and earlier.
                if (collidedWith is ButtonInteract)
                {
                    collidedWith.Call("Interact", this);
                }
                if (collidedWith is ButtonKeycardInteract)
                {
                    collidedWith.Call("Interact", this);
                }
                */
                if (collidedWith is Scp914DeviceKey)
                {
                    collidedWith.Call("RefineCall");
                }
                if (collidedWith is Scp914Gears)
                {
                    collidedWith.Call("Interact");
                }
                if (collidedWith is Pickable && scpNumber == -1)
                {
                    collidedWith.Call("PickUpItem", this);
                }
                if (collidedWith is DoorStaticOpener)
                {
                    collidedWith.Call("CallOpen");
                }
                if (collidedWith is LootableAmmo)
                {
                    collidedWith.Call("AddAmmo", this);
                }
            }
            
            if (!customMusic)
            {
                if (GlobalPosition.Y < -1500)
                {
                    GetParent<FacilityManager>().SetBackgroundMusic("res://Sounds/Music/PDTrench.ogg");
                }
                else if (GlobalPosition.Y < -500 && GlobalPosition.Y >= -1500)
                {
                    GetParent<FacilityManager>().SetBackgroundMusic("res://Sounds/Music/HeavyContainment.ogg");
                }
                else if (GlobalPosition.Y < -256 && GlobalPosition.Y >= -500)
                {
                    GetParent<FacilityManager>().SetBackgroundMusic("res://Sounds/Music/LightContainment.ogg");
                }
                else
                {
                    GetParent<FacilityManager>().SetBackgroundMusic("res://Sounds/Music/ResearchZone.ogg");
                }
            }

            //"Pocket dimension" check
            if (GlobalPosition.Y < -1500)
            {
                decayTimer += delta;
            }
            if (decayTimer > 3)
            {
                Decay();
                decayTimer = 0;
            }
            if (currentHealth[1] < health[1] && direction.IsZeroApprox())
            {
                currentHealth[1] += (float)delta / 10;
            }
            if (currentHealth[0] < health[0] && scpNumber >= 0 && direction.IsZeroApprox())
            {
                currentHealth[0] += (float)delta;
            }

            GetParent().GetNode<ProgressBar>("PlayerUI/HealthBar").Value = Mathf.Ceil(currentHealth[0]);
            GetParent().GetNode<ProgressBar>("PlayerUI/SanityBar").Value = Mathf.Ceil(currentHealth[1]);
        }
        UpDirection = Vector3.Up;
        MoveAndSlide();
    }
    /// <summary>
    /// Updates items in hand.
    /// </summary>
    /// <param name="itemName">Name of the item</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void UpdateItemsInHand(string itemName, string itemIndex)
    {
        if (ResourceLoader.Exists("res://InventorySystem/Items/" + itemName + ".tres"))
        {
            Item item = GD.Load<Item>("res://InventorySystem/Items/" + itemName + ".tres");
            // If third person view exists, else will be used first person view.
            if (GetNode<Node3D>("PlayerModel").GetChild(0).GetNodeOrNull<Marker3D>("Armature/Skeleton3D/ItemAttachment/ItemInHand") != null && !IsMultiplayerAuthority())
            {
                GetNode<Marker3D>("PlayerHead/PlayerHand").Hide();
                Node thirdPersonHandRoot = GetNode<Node3D>("PlayerModel").GetChild(0).GetNode<Marker3D>("Armature/Skeleton3D/ItemAttachment/ItemInHand");
                foreach (Node itemUsedBefore in thirdPersonHandRoot.GetChildren())
                {
                    itemUsedBefore.QueueFree();
                }
                /* old 0.7.x code.
                ItemAction tpsModel = ResourceLoader.Load<PackedScene>(item.FirstPersonPrefabPath).Instantiate<ItemAction>();
                tpsModel.oneTimeUse = item.OneTimeUse;
                tpsModel.index = int.Parse(itemIndex);
                thirdPersonHandRoot.AddChild(tpsModel, true);
                */

                Pickable tpsModel = ResourceLoader.Load<PackedScene>(item.PickablePath).Instantiate<Pickable>();
                tpsModel.Freeze = true;
                thirdPersonHandRoot.AddChild(tpsModel);
            }
            else
            {
                GetNode<Marker3D>("PlayerHead/PlayerHand").Show();
            }
            Node firstPersonHandRoot = GetNode<Marker3D>("PlayerHead/PlayerHand");
            foreach (Node itemUsedBefore in firstPersonHandRoot.GetChildren())
            {
                itemUsedBefore.QueueFree();
            }
            ItemAction fpsModel = ResourceLoader.Load<PackedScene>(item.FirstPersonPrefabPath).Instantiate<ItemAction>();
            fpsModel.oneTimeUse = item.OneTimeUse;
            fpsModel.index = int.Parse(itemIndex);
            firstPersonHandRoot.AddChild(fpsModel, true);
        }
        else
        {
            if (GetNode<Node3D>("PlayerModel").GetChildOrNull<Node3D>(0) != null)
            {
                if (GetNode<Node3D>("PlayerModel").GetChild(0).GetNodeOrNull<Marker3D>("Armature/Skeleton3D/ItemAttachment/ItemInHand") != null)
                {
                    Node thirdPersonHandRoot = GetNode<Node3D>("PlayerModel").GetChild(0).GetNode<Marker3D>("Armature/Skeleton3D/ItemAttachment/ItemInHand");
                    foreach (Node itemUsedBefore in thirdPersonHandRoot.GetChildren())
                    {
                        itemUsedBefore.QueueFree();
                    }
                }
            }
            Node firstPersonHandRoot = GetNode<Marker3D>("PlayerHead/PlayerHand");
            foreach (Node itemUsedBefore in firstPersonHandRoot.GetChildren())
            {
                itemUsedBefore.QueueFree();
            }
        }
    }

    /// <summary>
    /// Animation-based footstep system.
    /// </summary>
    void FootstepAnimate()
    {
        if (moveSoundsEnabled)
        {
            if (isWalking)
            {
                Rpc("PlayFootstepSound", false);
            }
            if (isSprinting)
            {
                Rpc("PlayFootstepSound", true);
            }
        }
    }
    /// <summary>
    /// Make footstep sounds audible to all.
    /// </summary>
    /// <param name="sprint">Is player sprinting</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void PlayFootstepSound(bool sprint)
    {
        walkSounds.Stream = sprint ? GD.Load<AudioStream>(sprintSounds[rng.RandiRange(0, footstepSounds.Length - 1)]) : GD.Load<AudioStream>(footstepSounds[rng.RandiRange(0, footstepSounds.Length - 1)]);
        walkSounds.Play();
    }

    /// <summary>
    /// Method that manages health of the player (don't work on spectators). If health is below 0, the players forceclasses as spectator.
    /// </summary>
    /// <param name="amount">How much health to add/deplete</param>
    /// <param name="depleteReason">Reason to add or deplete</param>
    /// <param name="typeOfHealth">Type of health (0 is health, 1 is sanity)</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void HealthManage(double amount, string depleteReason, int typeOfHealth)
    {
        if (scpNumber != -2)
        {
            if (currentHealth[typeOfHealth] + amount <= health[typeOfHealth])
            {
                currentHealth[typeOfHealth] += (float)amount;
            }
            else
            {
                currentHealth[typeOfHealth] = health[typeOfHealth];
            }
        }
        else
        {
            GD.Print("You cannot change HP for dead");
        }
        // Temporary feature, will be reworked in future...
        if (currentHealth[typeOfHealth] <= 0)
        {
            switch (typeOfHealth)
            {
                case 0:
                    GetParent<FacilityManager>().Rpc("SpawnRagdoll", this.Name, ragdollSource, false);
                    break;
                case 1:
                    GetParent<FacilityManager>().Rpc("SpawnRagdoll", this.Name, ragdollSource, true);
                    break;
            }
            GetTree().Root.GetNode<PlayerAction>("Main/Game/PlayerAction").CallForceclass(0, depleteReason);
        }
    }
    /// <summary>
    /// "Pocket dimension" controller
    /// </summary>
    void Decay()
    {
        if (scpNumber != 106)
        {
            HealthManage(-1, "Decayed at SCP-106's pocket dimension", 0);
        }
        else
        {
            HealthManage(1, "Decayed at SCP-106's pocket dimension", 0);
        }
    }
    /// <summary>
    /// Applies player camera shader. Only spatial shaders could be applied due to project structure.
    /// </summary>
    /// <param name="res">Child node path</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void ApplyShader(string res)
    {
        if (GetNodeOrNull("PlayerHead/PlayerCamera/" + res) != null && res != null && res != "")
        {
            GetNode<MeshInstance3D>("PlayerHead/PlayerCamera/" + res).Visible = true;
        }
        else
        {
            foreach (Node node in GetNode("PlayerHead/PlayerCamera/").GetChildren())
            {
                if (node is MeshInstance3D shader)
                {
                    shader.Visible = false;
                }
            }
        }
    }
    /// <summary>
    /// Applies player head position.
    /// </summary>
    /// <param name="cameraPos"></param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    internal void ApplyPlayerHeadPosition(float cameraPos)
    {
        playerHead.Position = new Vector3(0, cameraPos, 0);
    }
    /// <summary>
    /// Manages default camera. If custom camera, cursor won't be locked.
    /// </summary>
    /// <param name="defaultCamera">Check if is a default camera</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void CameraManager(bool defaultCamera)
    {
        if (defaultCamera)
        {
            GetNode<Camera3D>("PlayerHead/PlayerCamera").Current = true;
            if (!OS.IsDebugBuild()) //this method checks is this a debug build and does the current class have custom camera.
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
            customCamera = false;
        }
        else
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            customCamera = true;
        }
    }
    /// <summary>
    /// Sets player class color. Available since 0.7.0-dev
    /// </summary>
    /// <param name="color"></param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void UpdateClassUI(uint color)
    {
        Color classColor = new Color(color);
        if (GetParent().GetNode<ProgressBar>("PlayerUI/HealthBar").HasThemeStyleboxOverride("fill"))
        {
            GetParent().GetNode<ProgressBar>("PlayerUI/HealthBar").RemoveThemeStyleboxOverride("fill");
        }
        StyleBoxFlat classRepresent = new StyleBoxFlat();
        classRepresent.BgColor = classColor;
        GetParent().GetNode<ProgressBar>("PlayerUI/HealthBar").AddThemeStyleboxOverride("fill", classRepresent);
        GetParent().GetNode<ProgressBar>("PlayerUI/HealthBar").MaxValue = health[0];
        GetParent().GetNode<ProgressBar>("PlayerUI/HealthBar").Value = currentHealth[0];
        GetParent().GetNode<ProgressBar>("PlayerUI/SanityBar").MaxValue = health[1];
        GetParent().GetNode<ProgressBar>("PlayerUI/SanityBar").Value = currentHealth[1];
        if (GetParent().GetNode<Label>("PlayerUI/ClassInfo").HasThemeColorOverride("font_color"))
        {
            GetParent().GetNode<Label>("PlayerUI/ClassInfo").RemoveThemeColorOverride("font_color");
        }
        GetParent().GetNode<Label>("PlayerUI/ClassInfo").AddThemeColorOverride("font_color", classColor);
        GetParent().GetNode<Label>("PlayerUI/ClassInfo").Text = className;
        GetParent().GetNode<Label>("PlayerUI/ClassDescription").Text = classDescription;
        GetParent().GetNode<AnimationPlayer>("PlayerUI/AnimationPlayer").Play("forceclass");
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    void GrantModeratorPrivilegies()
    {
        isModerator = !isModerator;
        GD.Print("You " + (isModerator ? "log on into" : "log off from") + " into moderator's console");
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    void GrantAdminPrivilegies()
    {
        isAdmin = !isAdmin;
        GD.Print("You " + (isModerator ? "log on into" : "log off from") + " into moderator's console");
    }
}