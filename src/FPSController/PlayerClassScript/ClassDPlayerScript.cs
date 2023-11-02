using Godot;
using System;

public partial class ClassDPlayerScript : HumanPlayerScript
{
    internal override void OnStart()
    {
        Rpc("SetRandomSkin");
    }
    internal override void AnimationCycle()
	{
        if (GetParent().GetParent<PlayerScript>().dir.IsZeroApprox())
        {
            Rpc("SetState", "Idle");
        }
        else
        {
            if (Input.IsActionPressed("move_sprint"))
            {
                Rpc("SetState", "Running");
            }
            else
            {
                Rpc("SetState", "Walking");
            }
        }
    }

    /// <summary>
    /// Sets random Class-D face, like skins.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void SetRandomFace()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        StandardMaterial3D mat = new StandardMaterial3D();
        mat.AlbedoTexture = ResourceLoader.Load<Texture2D>("res://Assets/Models/classd/Faces/face" + rng.RandiRange(1, 2) + ".png");
        GetNode<MeshInstance3D>("Armature/Skeleton3D/base_obj").MaterialOverride = mat;
    }
}
