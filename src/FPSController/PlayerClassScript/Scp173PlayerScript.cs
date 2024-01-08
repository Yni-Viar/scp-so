using Godot;
using System;
/// <summary>
/// SCP-173 main script.
/// </summary>
public partial class Scp173PlayerScript : Node3D
{
    RandomNumberGenerator rng = new RandomNumberGenerator();
    RayCast3D ray;
    RayCast3D vision;
    AudioStreamPlayer3D interactSound;
    string[] poseArr = new string[] { "173_Pose1", "173_Pose2", "173_Pose3", "173_Pose4", "173_Pose5", "173_Pose6", "173_Pose7", "173_TPose" };
    [Export] bool isWatching = false;
    internal bool IsWatching
    {
        get => isWatching;
        set => isWatching = value;
    }
    [Export] int stareCounter = 0;
    internal int StareCounter
    {
        get => stareCounter;
        set => stareCounter = value;
    }
    float blinkTimer = 0f;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetNode<Node3D>("SCP173_Rig").Hide();
            GetNode<Control>("AbilityUI").Show();
            GetParent().GetParent<PlayerScript>().SetCollisionMaskValue(3, true);
            GetParent().GetParent<PlayerScript>().CanMove = true;
        }
        SetRandomFace();
        ray = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/RayCast3D");
        //vision = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/VisionRadius");
        interactSound = GetParent().GetParent<PlayerScript>().GetNode<AudioStreamPlayer3D>("InteractSound");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            if (Input.IsActionJustPressed("attack") && ray.IsColliding())
            {
                var collidedWith = ray.GetCollider();
                if (collidedWith is PlayerScript player)
                {
                    if (player.scpNumber == -1)
                    {
                        interactSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/173/NeckSnap" + rng.RandiRange(1, 3) + ".ogg");
                        interactSound.Play();
                        player.RpcId(int.Parse(player.Name), "HealthManage", -16777216, "Crunched by SCP-173");
                        Rpc("SetState", poseArr[rng.RandiRange(0, poseArr.Length - 1)]);
                    }
                }
            }
            Scp173Stare((float)delta);
        }
    }

    /// <summary>
    /// Method, that holds blinking. Available since 0.7.1-dev.
    /// </summary>
    /// <param name="delta">Used for timeout</param>
    async void Scp173Stare(float delta)
    {
        if (isWatching && stareCounter > 0 /*&& GetNode<LightDetector>("LightDetector").LightnessDetect() > 0.2*/) //Light detector is broken
        {
            //If SCP-173 is not moving, it should stand still!
            if (GetParent().GetParent<PlayerScript>().CanMove && blinkTimer > 0f)
            {
                GetParent().GetParent<PlayerScript>().CanMove = false;
            }
            else if (blinkTimer < 0f)//blink
            {
                GetParent().GetParent<PlayerScript>().CanMove = true;
                await ToSignal(GetTree().CreateTimer(0.5), "timeout");
                blinkTimer = 4.7f;
            }
            blinkTimer -= delta;
        }
        else
        { 
            //move freely
            GetParent().GetParent<PlayerScript>().CanMove = true;
        }
    }

    /// <summary>
    /// Set animation to an entity.
    /// </summary>
    /// <param name="s">Animation name</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void SetState(string s)
    {
        if (GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation != s)
        {
            //Change the animation.
            GetNode<AnimationPlayer>("AnimationPlayer").Play(s);
        }
    }
    /// <summary>
    /// Sets random 173 face, like skins. Available since 0.7.0-dev
    /// </summary>
    void SetRandomFace()
    {
        ShaderMaterial mat = new ShaderMaterial();
        mat.Shader = ResourceLoader.Load<Shader>("res://Shaders/MixShader/mix.gdshader");
        mat.SetShaderParameter("texture_a", ResourceLoader.Load<Texture2D>("res://Assets/Models/scp173-BaseTexture/scp173NEO_low_Merged_PM3D_Sphere3D4_AlbedoTransparency.png"));
        if (((int)Time.GetDateDictFromSystem(true)["month"]) == (int)Time.Month.December)
        {
            mat.SetShaderParameter("texture_b", ResourceLoader.Load<Texture2D>("res://Assets/Models/scp173-FaceTextures/face_festive_" + rng.RandiRange(1, 3).ToString() + ".png"));
        }
        //else if (((int)Time.GetDateDictFromSystem(true)["month"]) == (int)Time.Month.October) //to be implemented
        else
        {
            mat.SetShaderParameter("texture_b", ResourceLoader.Load<Texture2D>("res://Assets/Models/scp173-FaceTextures/face_" + rng.RandiRange(1, 10).ToString() + ".png"));
        }
        GetNode<MeshInstance3D>("SCP173_Rig/Skeleton3D/scp173_MESH").MaterialOverride = mat;
    }

    private void OnStareTriggerScreenEntered()
    {
        IsWatching = true;
    }

    private void OnStareTriggerScreenExited()
    {
        IsWatching = false;
    }

    private void OnStareAreaBodyEntered(Node3D body)
    {
        if (body is PlayerScript player && player.scpNumber == -1)
        {
            StareCounter++;
        }
        if (stareCounter > 0)
        {
            GetNode<VisibleOnScreenNotifier3D>("StareTrigger").Visible = true;
        }
    }

    private void OnStareAreaBodyExited(Node3D body)
    {
        if (body is PlayerScript player && player.scpNumber == -1)
        {
            StareCounter--;
        }
        if (stareCounter <= 0)
        {
            GetNode<VisibleOnScreenNotifier3D>("StareTrigger").Visible = false;
        }
    }
}
