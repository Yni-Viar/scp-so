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
    [Export] internal float health = 100f;
    [Export] internal float speed = 0f;
    [Export] internal float jump = 0f;
    [Export] internal Godot.Collections.Array<string> footstepSounds;
    float gravity = 9.8f;
    // SCP Number. Set -1 for humans, -2 for spectators.
    [Export] internal int scpNumber = -2;

    [Export] bool canMove = true;
    internal bool CanMove {get=>canMove; set=>canMove = value;}
    
    float groundAcceleration = 8.0f;
    float airAcceleration = 8.0f;
    float acceleration;

    // float slidePrevention = 10.0f;
    Vector2 mouseSensivity = new Vector2(0.125f, 2f);

    // Vector3 direction;
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
            // gameOverScreen = GetNode<TextureRect>("UI/GameOver/GameOverScreen");
            // gameOverText = GetNode<Label>("UI/GameOver/GameOverMessage");
            acceleration = groundAcceleration;
            // Input.MouseMode = Input.MouseModeEnum.Captured;
            FloorMaxAngle = 1.308996f;
        }
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("forceclass", new Callable(this, "Forceclass"), "Forceclass the player (1 argument needed)");
        GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("classlist", new Callable(this, "ClassList"), "Returns class names (for forceclass)");
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
            
            if (canMove)
            {
                //running
                if (Input.IsActionPressed("move_sprint") && scpNumber == -1) //SCPs cannot sprint
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
            if (!direction.IsZeroApprox() && !walkSounds.Playing && scpNumber != -2)
            {
                if (isSprinting)
                {
                    walkSounds.Stream = GD.Load<AudioStream>("res://Sounds/Character/Human/Step/Run" + rng.RandiRange(1, 8) + ".ogg");
                    walkSounds.PitchScale = 2;
                    walkSounds.Play();
                }
                else if (isWalking)
                {
                    walkSounds.Stream = GD.Load<AudioStream>(footstepSounds[rng.RandiRange(0, footstepSounds.Count - 1)]);
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
            }
            //needed for walking up and down stairs, deprecated in 0.2.0 for optimizing the game.
            /*if (bottomRaycast.IsColliding() && !topRaycast.IsColliding())
            {
                if (Input.IsActionPressed("move_backward") || Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right"))
                {
                    FloorMaxAngle = 0.785398f; //45 degrees
                }
                else
                {
                    FloorMaxAngle = 1.308996f; //75 degrees, need to climb stairs
                }
            }*/
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
    private void OnNpcInteractBodyEntered(Node3D body)
    {
    }

    string Forceclass(string[] args)
    {
        if (args.Length == 1 && ClassParser.ReadJson("user://playerclass_0.4.0.json").Keys.Contains(args[0]))
        {
            CallForceclass(args[0]);
            return "Tried to forceclass to " + args[0];
        }
        else
        {
            return "You need ONLY 1 argument to forceclass. E.g. to spawn as SCP-173, you need to write \"forceclass scp173\"";
        }
    }

    void CallForceclass(string to)
    {
        GetParent().GetParent().GetNode<FacilityManager>("Game").Rpc("SetPlayerClass", Multiplayer.GetUniqueId().ToString(), to);
    }

    string ClassList(string[] args)
    {
        string r = "";
        foreach (var val in ClassParser.ReadJson("user://playerclass_0.4.0.json"))
        {
            r += val.Key + "\n";
        }
        return r;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void HealthManage(int amount)
    {
        health += amount;
        
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
    /*internal void GameOver(int whichScreen) // DEPRECATED, since 0.2.0-MP-dev, will be rewritten after player class manager + health will be implemented
    {
        gameOver = true;
        GD.Print("Game Over!");
        //I know that this code is not good. But I am a newb programmer... :( 
        //-Yni
        switch (whichScreen)
        {
            //left for future.
            case 0:
                gameOverScreen.Texture = GD.Load<Texture2D>("res://Assets/GameOverScreens/GameOverCause_173.png");
                gameOverText.Text = "Crunch!";
                break;
        }
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetNode<Control>("UI/GameOver").Show();
    }*/
}