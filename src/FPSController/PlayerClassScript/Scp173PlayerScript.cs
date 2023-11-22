using Godot;
using System;

public partial class Scp173PlayerScript : Node3D
{
    RandomNumberGenerator rng = new RandomNumberGenerator();
    RayCast3D ray;
    RayCast3D vision;
    AudioStreamPlayer3D interactSound;
    string[] poseArr = new string[] { "173_Pose1", "173_Pose2", "173_Pose3", "173_Pose4", "173_Pose5", "173_Pose6", "173_Pose7", "173_TPose" };
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetNode<Node3D>("SCP173_Rig").Hide();
            GetNode<Control>("AbilityUI").Show();
            GetParent().GetParent<PlayerScript>().SetCollisionMaskValue(3, true);
            GetParent().GetParent<PlayerScript>().SetCollisionLayerValue(5, true);
            GetParent().GetParent<PlayerScript>().CanMove = true;
        }
        Rpc("SetRandomFace");
        ray = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/RayCast3D");
        vision = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/VisionRadius");
        interactSound = GetParent().GetParent<PlayerScript>().GetNode<AudioStreamPlayer3D>("InteractSound");
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override async void _Process(double delta)
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
            if (vision.IsColliding())
            {
                var collidedWith = vision.GetCollider();
                if (collidedWith is PlayerScript player)
                {
                    if (player.scpNumber == -1 && player.GetNode("PlayerModel").GetChildOrNull<HumanPlayerScript>(0) != null)
                    {
                        if (player.GetNode("PlayerModel").GetChildOrNull<HumanPlayerScript>(0).isWatchingAtScp173)
                        {
                            LookingAtScp173(true, delta);
                            await ToSignal(GetTree().CreateTimer(5.0), "timeout");
                            LookingAtScp173(false, delta);
                        }
                        else
                        {
                            LookingAtScp173(false, delta);
                        }
                    }
                    if (player.scpNumber == 131) //SCP-131 cannot blink, so SCP-173, get trapped.
                    {
                        if (player.GetNode("PlayerModel").GetChildOrNull<Scp131PlayerScript>(0).isWatchingAtScp173)
                        {
                            LookingAtScp173(true, delta);
                        }
                        else
                        {
                            LookingAtScp173(false, delta);
                        }
                    }
                    else
                    {
                        LookingAtScp173(false, delta);
                    }
                }
                else
                {
                    LookingAtScp173(false, delta);
                }
            }
        }
    }

    /// <summary>
    /// Method, that holds blinking.
    /// </summary>
    void LookingAtScp173(bool isWatching, double delta)
    {
        if (isWatching)
        {
            //If SCP-173 is not moving, it should stand still!
            if (GetParent().GetParent<PlayerScript>().CanMove)
            {
                GetParent().GetParent<PlayerScript>().CanMove = false;
            }
        }
        else
        { //move freely
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
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void SetRandomFace()
    {
        ShaderMaterial mat = new ShaderMaterial();
        mat.Shader = ResourceLoader.Load<Shader>("res://Shaders/MixShader/mix.gdshader");
        mat.SetShaderParameter("texture_a", ResourceLoader.Load<Texture2D>("res://Assets/Models/scp173-BaseTexture/scp173NEO_low_Merged_PM3D_Sphere3D4_AlbedoTransparency.png"));
        mat.SetShaderParameter("texture_b", ResourceLoader.Load<Texture2D>("res://Assets/Models/scp173-FaceTextures/face_" + rng.RandiRange(1, 10).ToString() + ".png"));
        GetNode<MeshInstance3D>("SCP173_Rig/Skeleton3D/scp173_MESH").MaterialOverride = mat;
    }
}
