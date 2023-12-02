using Godot;
using System;

public partial class ClassDPlayerScript : HumanPlayerScript
{
    internal override void OnStart()
    {
        Rpc("SetRandomFace");
    }
    internal override void OnUpdate(double delta)
	{
        /* 0.7.0-dev code.
         * if (GetParent().GetParent<PlayerScript>().dir.IsZeroApprox())
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
        */
        GetNode<AnimationTree>("AnimationTree").Active = true;
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            //movement animations
            if (Input.IsActionPressed("move_forward") || Input.IsActionPressed("move_backward") || Input.IsActionPressed("move_right") || Input.IsActionPressed("move_left"))
            {
                if (Input.IsActionPressed("move_sprint"))
                {
                    //Rpc("SetState", "state_machine", "blend_amount", 1.0f);
                    Rpc("SetState", "state_machine", "blend_amount",
                        Mathf.Lerp(GetNode<AnimationTree>("AnimationTree").Get("parameters/state_machine/blend_amount").AsDouble(), 1, delta * GetParent().GetParent<PlayerScript>().speed * 2));
                }
                else
                {
                    Rpc("SetState", "state_machine", "blend_amount",
                        Mathf.Lerp(GetNode<AnimationTree>("AnimationTree").Get("parameters/state_machine/blend_amount").AsDouble(), 0, delta * GetParent().GetParent<PlayerScript>().speed * 2));
                }
            }
            else
            {
                Rpc("SetState", "state_machine", "blend_amount",
                    Mathf.Lerp(GetNode<AnimationTree>("AnimationTree").Get("parameters/state_machine/blend_amount").AsDouble(), -1, delta * GetParent().GetParent<PlayerScript>().speed * 2));
            }
            //item in hand animation
            if (GetNode<Marker3D>("Armature/Skeleton3D/ItemAttachment/ItemInHand").GetChildCount() > 0)
            {
                Rpc("SetState", "items_blend", "blend_amount", 1.0f);
            }
            else
            {
                Rpc("SetState", "items_blend", "blend_amount", 0.0f);
            }
        }
    }

    /// <summary>
    /// Sets random Class-D face, like skins. Available since 0.7.0-dev.
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
