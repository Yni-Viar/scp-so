using Godot;
using System;

public partial class GuardPlayerScript : HumanPlayerScript
{
    internal override void OnUpdate()
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
}
