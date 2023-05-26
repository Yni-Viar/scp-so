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
    RayCast3D ray;

    //stair check
    RayCast3D bottomRaycast;
    RayCast3D topRaycast;

    //Walk sounds and gameover screens.
    Control blinkImage;
    TextureRect gameOverScreen;
    Label gameOverText;
    AudioStreamPlayer3D walkSounds;
    AudioStreamPlayer3D gameOverSound;

    //blinking
    double blinkTimer = 0d;
    float blinkWaiting;


    float speed = 4.5f;
    float jump = 4.5f;
    float gravity = 9.8f;

    float groundAcceleration = 8.0f;
    float airAcceleration = 8.0f;
    float acceleration;

    float slidePrevention = 10.0f;
    Vector2 mouseSensivity = new Vector2(0.125f, 2f);

    Vector3 direction = new Vector3();
    Vector3 vel = new Vector3();
    Vector3 movement = new Vector3();
    Vector3 gravityVector = new Vector3();

    bool isOnGround = true;
    bool isSprinting = false;
    bool isWalking = false;
    bool gameOver = false;

    //item-specific properties
    public Pickable holdingItem = null;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        playerHead = GetNode<Node3D>("PlayerHead");
        ray = GetNode<RayCast3D>("PlayerHead/RayCast3D");
        blinkImage = GetNode<Control>("UI/Blink");
        bottomRaycast = GetNode<RayCast3D>("PlayerFeet/StairCheck");
        topRaycast = GetNode<RayCast3D>("PlayerFeet/StairCheck2");
        walkSounds = GetNode<AudioStreamPlayer3D>("WalkSounds");
        gameOverSound = GetNode<AudioStreamPlayer3D>("GameOverSound");
        gameOverScreen = GetNode<TextureRect>("UI/GameOver/GameOverScreen");
        gameOverText = GetNode<Label>("UI/GameOver/GameOverMessage");
        acceleration = groundAcceleration;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        blinkWaiting = 5.2f;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!gameOver)
        {
            blinkTimer += delta;
        }

        if (blinkTimer > blinkWaiting)
        {
            Blink();
            blinkTimer = 0d;
        }
    }

    public override void _Input(InputEvent @event)
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
        direction = new Vector3();
        direction.Z = -Input.GetActionStrength("move_forward") + Input.GetActionStrength("move_backward");
        direction.X = -Input.GetActionStrength("move_left") + Input.GetActionStrength("move_right");
        direction = direction.Normalized().Rotated(Vector3.Up, Rotation.Y);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!gameOver)
        {
            //Anti-endless fall check. Needed to jump properly.
            if (IsOnFloor())
            {
                gravityVector = -GetFloorNormal() * slidePrevention;
                acceleration = groundAcceleration;
                isOnGround = true;
            }
            else
            {
                if (isOnGround)
                {
                    gravityVector = Vector3.Zero;
                    isOnGround = false;
                }
                else
                {
                    gravityVector += Vector3.Down * gravity * (float)delta;
                    acceleration = airAcceleration;
                }
            }
            //jumping
            if (Input.IsActionJustPressed("move_jump") && isOnGround)
            {
                isOnGround = false;
                gravityVector = Vector3.Up * jump;
            }
            //running
            if (Input.IsActionPressed("move_sprint"))
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

            //check if we don't stay still and is footstep audio playing;
            if (Velocity.Length() != 0 && !walkSounds.Playing)
            {
                if (isSprinting)
                {
                    walkSounds.Stream = GD.Load<AudioStream>("res://Sounds/Character/Human/Step/Run" + rng.RandiRange(1, 8) + ".ogg");
                    walkSounds.PitchScale = 2;
                    walkSounds.Play();
                }
                else if (isWalking)
                {
                    walkSounds.Stream = GD.Load<AudioStream>("res://Sounds/Character/Human/Step/Step" + rng.RandiRange(1, 8) + ".ogg");
                    walkSounds.PitchScale = 1;
                    walkSounds.Play();
                }
            }
            else if (Velocity.Length() == 0 && walkSounds.Playing)
            {
                walkSounds.Stop();
            }
            
            //Picking up items.
            if (ray.IsColliding() && Input.IsActionJustPressed("interact_item"))
            {
                if (holdingItem != null)
                {
                    //drop holding item
                    holdingItem.PickUpItem(this);
                }
                else
                {
                    var collidedWith = ray.GetCollider();
                    if (collidedWith.HasMethod("PickUpItem")) //pick up item.
                    {
                        collidedWith.Call("PickUpItem", this);
                    }
                    if (collidedWith is ButtonInteract)
                    {
                        collidedWith.Call("Interact", this);
                    }
                }
            }
            

            //needed for walking up and down stairs
            if (bottomRaycast.IsColliding() && !topRaycast.IsColliding())
            {
                if (Input.IsActionPressed("move_backward") || Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right"))
                {
                    FloorMaxAngle = 0.785398f; //45 degrees
                }
                else
                {
                    FloorMaxAngle = 1.308996f; //75 degrees, need to climb stairs
                }
            }

            movement.Z = vel.Z + gravityVector.Z;
            movement.X = vel.X + gravityVector.X;
            movement.Y = gravityVector.Y;
        }
        

        Velocity = movement;
        UpDirection = Vector3.Up;
        MoveAndSlide();
    }
    private void OnNpcInteractBodyEntered(Node3D body)
    {
        //CRUNCH!!!
        if (body.Name == "scp173" && !gameOver)
        {
            gameOverSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/173/NeckSnap" + rng.RandiRange(1, 3) + ".ogg");
            gameOverSound.Play();
            GameOver(0);
        }
    }
    private async void Blink()
    {
        //main blinking method
        blinkImage.Show();
        PlayerCommon.isBlinking = true;
        await ToSignal(GetTree().CreateTimer(0.3), "timeout");
        PlayerCommon.isBlinking = false;
        blinkImage.Hide();
    }
    private void GameOver(int whichScreen)
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
    }

    private void OnExitPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }
}