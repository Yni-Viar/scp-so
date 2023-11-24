using Godot;
using System;

public partial class ScientistPlayerScript : HumanPlayerScript
{
    internal override void OnUpdate(double delta)
    {
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
    }
}
