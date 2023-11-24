using Godot;
using System;
using static System.Collections.Specialized.BitVector32;

public partial class MtfPlayerScript : HumanPlayerScript
{
    internal override void OnUpdate(double delta)
    {
        /*
         * 
        if (GetParent().GetParent<PlayerScript>().dir.IsZeroApprox())
        {
            Rpc("SetState", "MTF_Idle");
        }
        else
        {
            if (Input.IsActionPressed("move_sprint"))
            {
                Rpc("SetState", "MTF_Running");
            }
            else
            {
                Rpc("SetState", "MTF_Walking");
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
}
