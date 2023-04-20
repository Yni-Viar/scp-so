using Godot;
using System;

public partial class PlayerScript : CharacterBody3D
{
	/* This script was created by dzejpi. License - The Unlicense. 
	 * Some parts used from elmarcoh (this script is also public domain).
	 */
    RandomNumberGenerator rng = new RandomNumberGenerator();
	Node3D playerHead;
	RayCast3D ray;
	RayCast3D bottomRaycast;
	RayCast3D topRaycast;
	TextureRect gameOverScreen;
    Label gameOverText;
    AudioStreamPlayer3D walkSounds;

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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerHead = GetNode<Node3D>("PlayerHead");
		ray = GetNode<RayCast3D>("PlayerHead/RayCast3D");
		bottomRaycast = GetNode<RayCast3D>("PlayerFeet/StairCheck");
		topRaycast = GetNode<RayCast3D>("PlayerFeet/StairCheck2");
        walkSounds = GetNode<AudioStreamPlayer3D>("WalkSounds");
		gameOverScreen = GetNode<TextureRect>("UI/GameOver/GameOverScreen");
        gameOverText = GetNode<Label>("UI/GameOver/GameOverMessage");
		acceleration = groundAcceleration;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
		{
			InputEventMouseMotion m = (InputEventMouseMotion) @event;
			this.RotateY(Mathf.DegToRad(-m.Relative.X * mouseSensivity.Y / 10));
			playerHead.RotateX(Mathf.Clamp(-m.Relative.Y * mouseSensivity.X / 10, -90, 90));

			Vector3 cameraRot = playerHead.Rotation;
			cameraRot.X = Mathf.Clamp(cameraRot.X, Mathf.DegToRad(-85f), Mathf.DegToRad(85f));
		}
		direction = new Vector3();
		direction.Z = -Input.GetActionStrength("move_forward") + Input.GetActionStrength("move_backward");
		direction.X = -Input.GetActionStrength("move_left") + Input.GetActionStrength("move_right");
		direction = direction.Normalized().Rotated(Vector3.Up, Rotation.Y);

		if (Input.IsActionJustPressed("ui_cancel"))
		{
			GetTree().Quit();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
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
		if (Input.IsActionJustPressed("move_jump"))
		{
			isOnGround = false;
			gravityVector = Vector3.Up * jump;
		}
        if (Input.IsActionPressed("move_sprint"))
        {
            vel = vel.Lerp(direction * speed * 2, acceleration * (float)delta);
            isSprinting = true;
            isWalking = false;
        }
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

		Velocity = movement;
		UpDirection = Vector3.Up;
		MoveAndSlide();
	}
	private void OnNpcInteractBodyEntered(Node3D body)
	{
		if (body.Name == "scp173")
		{
			GameOver(0);
		}
	}

	private void GameOver(int whichScreen)
	{
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
        GetTree().Quit();
    }
}



