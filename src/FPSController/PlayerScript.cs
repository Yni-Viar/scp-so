using Godot;
using System;

public partial class PlayerScript : CharacterBody3D
{
    /* This script was created by dzejpi. License - The Unlicense. 
     * Some parts used from elmarcoh (this script is also public domain).
     * 
     * I (Yni) added some parts, such as blinking or game over triggers.
     * Currently the script is a big mess, so it need refactoring :(
     */
    [Signal]
    public delegate void ItemInHandChangedEventHandler(string itemToReplace);

    RandomNumberGenerator rng = new RandomNumberGenerator();
    Node3D playerHead;
    Settings settings;
    internal RayCast3D ray;

    //Walk sounds.
    AudioStreamPlayer3D walkSounds;
    AudioStreamPlayer3D interactSound;
    [Export] internal float currentHealth = 1f;

    double decayTimer = 0d;

    [Export] internal string classKey;
    [Export] internal string className;
    [Export] internal string spawnPoint;
    [Export] internal string playerModelSource;
    [Export] internal float health = 1f;
    [Export] internal float speed = 0f;
    [Export] internal float jump = 0f;
    [Export] internal bool sprintEnabled = false;
    [Export] internal bool moveSoundsEnabled = false;
    [Export] internal string[] footstepSounds;
    [Export] internal string[] sprintSounds;
    [Export] internal Globals.Team team;

    // Currently, ragdolls are unstable. (Or give me a sign, that they are working). So these "ragdolls" are just death animations.
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
        settings = GetTree().Root.GetNode<Settings>("Settings");
        if (IsMultiplayerAuthority())
        {
            GetNode<Camera3D>("PlayerHead/PlayerCamera").Current = true;
            playerHead = GetNode<Node3D>("PlayerHead");
            ray = GetNode<RayCast3D>("PlayerHead/RayCast3D");
            walkSounds = GetNode<AudioStreamPlayer3D>("WalkSounds");
            interactSound = GetNode<AudioStreamPlayer3D>("InteractSound");
            acceleration = groundAcceleration;
            FloorMaxAngle = 1.48353f; //85 degrees.
        }
    }

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
        if (IsMultiplayerAuthority())
        {
            if (@event is InputEventMouseMotion)
            {
                InputEventMouseMotion m = (InputEventMouseMotion) @event;
                if (!customCamera)
                {
                    this.RotateY(Mathf.DegToRad(-m.Relative.X * settings.MouseSensitivity * 2f)); //pretty magic numbers
                    playerHead.RotateX(Mathf.Clamp(-m.Relative.Y * settings.MouseSensitivity * 0.125f, -90, 90));
    
                    Vector3 cameraRot = playerHead.Rotation;
                    cameraRot.X = Mathf.Clamp(playerHead.Rotation.X, Mathf.DegToRad(-85f), Mathf.DegToRad(85f));
                    playerHead.Rotation = cameraRot;
                }
            }

            if (Input.IsActionJustPressed("mode_kinematic"))
            {
                GetParent().GetNode<Control>("PlayerUI").Visible = !GetParent().GetNode<Control>("PlayerUI").Visible;
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
                if (collidedWith is ButtonInteract)
                {
                    collidedWith.Call("Interact", this);
                }
                if (collidedWith is ButtonKeycardInteract)
                {
                    collidedWith.Call("Interact", this);
                }
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
                if (collidedWith is ItemAction action && action.GetPath().ToString().Contains(Name))
                {
                    collidedWith.Call("OnUse", this);
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

            GetParent().GetNode<Label>("PlayerUI/HealthInfo").Text = Mathf.Ceil(currentHealth).ToString();
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
            Node firstPersonHandRoot = GetNode<Marker3D>("PlayerHead/PlayerHand");
            foreach (Node itemUsedBefore in firstPersonHandRoot.GetChildren())
            {
                itemUsedBefore.QueueFree();
            }
            Item item = GD.Load<Item>("res://InventorySystem/Items/" + itemName + ".tres");
            ItemAction tmpModel = ResourceLoader.Load<PackedScene>(item.FirstPersonPrefabPath).Instantiate<ItemAction>();
            tmpModel.oneTimeUse = item.OneTimeUse;
            tmpModel.index = int.Parse(itemIndex);
            firstPersonHandRoot.AddChild(tmpModel, true);
        }
        else
        {
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
                walkSounds.Stream = GD.Load<AudioStream>(footstepSounds[rng.RandiRange(0, footstepSounds.Length - 1)]);
                walkSounds.Play();
            }
            if (isSprinting)
            {
                walkSounds.Stream = GD.Load<AudioStream>(sprintSounds[rng.RandiRange(0, footstepSounds.Length - 1)]);
                walkSounds.Play();
            }
        }
    }

    /// <summary>
    /// Helper method to teleport.
    /// </summary>
    /// <param name="placeToTeleport">Place to teleport</param>
    internal void CallTeleport(string placeToTeleport)
    {
        GetParent().GetParent().GetNode<FacilityManager>("Game").Rpc("TeleportTo", Multiplayer.GetUniqueId().ToString(), PlacesForTeleporting.defaultData[placeToTeleport]);
    }

    /// <summary>
    /// Helper method to call FacilityManager for changing player class.
    /// </summary>
    /// <param name="to">Player class to become</param>
    internal void CallForceclass(string to)
    {
        GetParent().GetParent().GetNode<FacilityManager>("Game").Rpc("SetPlayerClass", Multiplayer.GetUniqueId().ToString(), to);
    }

    /// <summary>
    /// Add or depletes health (don't work on spectators). If health is below 0, the players forceclasses as spectator.
    /// </summary>
    /// <param name="amount">How much health to add/deplete</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void HealthManage(double amount)
    {
        if (scpNumber != -2)
        {
            if (currentHealth + amount <= health)
            {
                currentHealth += (float)amount;
            }
            else
            {
                currentHealth = health;
            }
        }
        else
        {
            GD.Print("You cannot change HP for spectators");
        }
        
        if (currentHealth <= 0)
        {
            GetParent<FacilityManager>().Rpc("SpawnRagdoll", this.Name, ragdollSource);
            CallForceclass("spectator");
        }
    }

    /// <summary>
    /// "Pocket dimension" controller
    /// </summary>
    void Decay()
    {
        if (scpNumber != 106)
        {
            HealthManage(-1);
        }
        else
        {
            HealthManage(1);
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
}