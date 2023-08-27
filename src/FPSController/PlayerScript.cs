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
    RandomNumberGenerator rng = new RandomNumberGenerator();
    Node3D playerHead;
    internal RayCast3D ray;

    //Walk sounds and gameover screens.
    // Control blinkImage;
    // TextureRect gameOverScreen;
    // Label gameOverText;
    AudioStreamPlayer3D walkSounds;
    AudioStreamPlayer3D interactSound;

    //blinking, deprecated in 0.3.0-dev
    // double blinkTimer = 0d;
    // float blinkWaiting;

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
    float gravity = 9.8f;
    // SCP Number. Set -1 for humans, -2 for spectators.
    [Export] internal int scpNumber = -2;

    // beginning with 0.4.0-dev, you cannot move while waiting for match. Instead, every class re-enables the movement toggle.
    [Export] bool canMove = false; 
    internal bool CanMove {get=>canMove; set=>canMove = value;}
    
    float groundAcceleration = 8.0f;
    float airAcceleration = 8.0f;
    float acceleration;

    Vector2 mouseSensivity = new Vector2(0.125f, 2f);

    internal Vector3 dir = new Vector3();
    Vector3 vel = new Vector3();
    Vector3 movement = new Vector3();
    Vector3 gravityVector = new Vector3();

    bool isSprinting = false;
    bool isWalking = false;
    // internal bool gameOver = false;

    //item-specific properties, currently removed.
    // public Pickable holdingItem = null;

    public override void _EnterTree()
    {
        SetMultiplayerAuthority(int.Parse(Name));
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (IsMultiplayerAuthority())
        {
            GetNode<Camera3D>("PlayerHead/PlayerCamera").Current = true;
            playerHead = GetNode<Node3D>("PlayerHead");
            ray = GetNode<RayCast3D>("PlayerHead/RayCast3D");
            // blinkImage = GetNode<Control>("UI/Blink");
            walkSounds = GetNode<AudioStreamPlayer3D>("WalkSounds");
            interactSound = GetNode<AudioStreamPlayer3D>("InteractSound");
            acceleration = groundAcceleration;
            if (!OS.IsDebugBuild()) //this method checks is this a debug build.
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
            FloorMaxAngle = 1.439897f; //82.5 degrees.
        }
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("forceclass", new Callable(this, "Forceclass"), "Forceclass the player (1 argument needed)");
        // GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("classlist", new Callable(this, "ClassList"), "Returns class names (for forceclass)");
    }

    private bool IsLocalAuthority()
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
                this.RotateY(Mathf.DegToRad(-m.Relative.X * mouseSensivity.Y / 10));
                playerHead.RotateX(Mathf.Clamp(-m.Relative.Y * mouseSensivity.X / 10, -90, 90));
    
                Vector3 cameraRot = playerHead.Rotation;
                cameraRot.X = Mathf.Clamp(playerHead.Rotation.X, Mathf.DegToRad(-85f), Mathf.DegToRad(85f));
                playerHead.Rotation = cameraRot;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsMultiplayerAuthority())
        {
            /*if (scpNumber == -1) //deprecated in 0.3.0-dev
            {
                blinkTimer += delta;
            }*/
            if (!IsOnFloor())
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
            if (!direction.IsZeroApprox() && !walkSounds.Playing && moveSoundsEnabled)
            {
                if (isSprinting)
                {
                    walkSounds.Stream = GD.Load<AudioStream>(sprintSounds[rng.RandiRange(0, footstepSounds.Length - 1)]);
                    walkSounds.PitchScale = 2;
                    walkSounds.Play();
                }
                else if (isWalking)
                {
                    walkSounds.Stream = GD.Load<AudioStream>(footstepSounds[rng.RandiRange(0, footstepSounds.Length - 1)]);
                    walkSounds.PitchScale = 1;
                    walkSounds.Play();
                }

                else if (direction.IsZeroApprox() && walkSounds.Playing)
                {
                    walkSounds.Stop();
                }
            }
            

            //Interacting. (item subsystem will be rewritten)
            if (ray.IsColliding() && Input.IsActionJustPressed("interact"))
            {
                var collidedWith = ray.GetCollider();
                if (collidedWith is ButtonInteract)
                {
                    collidedWith.Call("Interact", this);
                }
                if (collidedWith is scp914devicekey)
                {
                    collidedWith.Call("RefineCall");
                }
                if (collidedWith is scp914gears)
                {
                    collidedWith.Call("Interact");
                }
            }
            /*if (blinkTimer > blinkWaiting) //deprecated in 0.3.0-dev, because of blink system rework.
            {
                Blink();
                blinkTimer = 0d;
            }*/

            GetParent().GetNode<Label>("PlayerUI/HealthInfo").Text = Mathf.Ceil(health).ToString();
        }
        UpDirection = Vector3.Up;
        MoveAndSlide();
    }
    //unused
    private void OnNpcInteractBodyEntered(Node3D body)
    {
    }
    /// <summary>
    /// GDSh command. Calls helper CallForceclass() method to change the player class.
    /// </summary>
    /// <param name="args">Player class to become</param>
    /// <returns>Success or failure for changing the player class</returns>
    string Forceclass(string[] args)
    {
        if (args.Length == 1 && ResourceLoader.Exists("res://FPSController/PlayerClassResources/" + args[0] + ".tres"))
        {
            CallForceclass(args[0]);
            return "Forceclassed to " + args[0];
        }
        else
        {
            return "You need ONLY 1 argument to forceclass. E.g. to spawn as SCP-173, you need to write \"forceclass scp173\"";
        }
    }

    /// <summary>
    /// Helper method to call FacilityManager for changing player class.
    /// </summary>
    /// <param name="to">Player class to become</param>
    private void CallForceclass(string to)
    {
        GetParent().GetParent().GetNode<FacilityManager>("Game").Rpc("SetPlayerClass", Multiplayer.GetUniqueId().ToString(), to);
    }

    //depreacted since 0.5.0-dev
    /*string ClassList(string[] args)
    {
        string r = "";
        foreach (var val in ClassData.playerClasses)
        {
            r += val.Key + "\n";
        }
        return r;
    }*/
    /// <summary>
    /// Add or depletes health (don't work on spectators). If health is below 0, the players forceclasses as spectator.
    /// </summary>
    /// <param name="amount">How much health to add/deplete</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void HealthManage(int amount)
    {
        if (scpNumber != -2)
        {
            health += amount;
        }
        else
        {
            GD.Print("You cannot change HP for spectators");
        }
        
        if (health <= 0)
        {
            CallForceclass("spectator");
        }
    }
    /*private async void Blink() //Deprecated in 0.3.0-dev due to blink system rework.
    {
        //main blinking method
        blinkImage.Show();
        isBlinking = true;
        await ToSignal(GetTree().CreateTimer(0.3), "timeout");
        isBlinking = false;
        blinkImage.Hide();
    }*/
}