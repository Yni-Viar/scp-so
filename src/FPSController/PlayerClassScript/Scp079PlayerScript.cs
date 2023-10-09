using Godot;
using System;

public partial class Scp079PlayerScript : Node
{
    // to be added in future.
    //Godot.Collections.Array<Node> dcczCamList = new Godot.Collections.Array<Node>();
    //Godot.Collections.Array<Node> hczCamList = new Godot.Collections.Array<Node>();
    //Godot.Collections.Array<Node> lczCamList = new Godot.Collections.Array<Node>();
    //Godot.Collections.Array<Node> rzCamList = new Godot.Collections.Array<Node>();
    string currentCam = "";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetParent().GetParent<PlayerScript>().GetNode<Camera3D>("PlayerHead/PlayerCamera").Current = false;
            //dcczCamList = GetTree().GetNodesInGroup("DCCZ_CCTV");
            /*hczCamList = GetTree().GetNodesInGroup("HCZ_CCTV");
            lczCamList = GetTree().GetNodesInGroup("LCZ_CCTV");
            rzCamList = GetTree().GetNodesInGroup("RZ_CCTV");
            
            if (currentCam == "")
            {
                foreach (Node cam in lczCamList)
                {
                    if (cam.Name.Equals("cctv079"))
                    {
                        SwitchCamera()
                    }
                }
            }*/
            SwitchCamera("Lcz", "LC_cont1_079");
        }
        GetParent().GetParent<PlayerScript>().CanMove = false;
        Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("scp079_map"))
        {
            GetNode<Control>("UI/MapGenHUD").Visible = !GetNode<Control>("UI/MapGenHUD").Visible;
        }
	}

    internal void SwitchCamera(string zone, string cam)
    {
        if (currentCam != "")
        {
            GetTree().Root.GetNode<CctvCamera>(currentCam).Initialize(false);
        }
        currentCam = "Main/Game/MapGen" + zone +"/" + cam + "/cctv";
        if (GetTree().Root.GetNodeOrNull<CctvCamera>(currentCam) != null)
        {
            GetTree().Root.GetNode<CctvCamera>(currentCam).Initialize(true);
        }
        else
        {
            currentCam = "Main/Game/MapGenLcz/LC_cont1_079/cctv";
            GetTree().Root.GetNode<CctvCamera>(currentCam).Initialize(true);
        }
    }
}
