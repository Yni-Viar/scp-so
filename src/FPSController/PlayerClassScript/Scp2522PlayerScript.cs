using Godot;
using System;

public partial class Scp2522PlayerScript : ComputerPlayerScript
{
    // to be added in future.
    //Godot.Collections.Array<Node> dcczCamList = new Godot.Collections.Array<Node>();
    //Godot.Collections.Array<Node> hczCamList = new Godot.Collections.Array<Node>();
    //Godot.Collections.Array<Node> lczCamList = new Godot.Collections.Array<Node>();
    //Godot.Collections.Array<Node> rzCamList = new Godot.Collections.Array<Node>();
    string currentCam = "";
    [Export(PropertyHint.Range, "0,100,0.01")] internal float energy = 100f;
	// Called when the node enters the scene tree for the first time.
	internal override void OnStart()
	{
        GetParent().GetParent<PlayerScript>().CanMove = false;
        GetParent().GetParent<PlayerScript>().SetCollisionMaskValue(3, true);
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetParent().GetParent<PlayerScript>().GetNode<Camera3D>("PlayerHead/PlayerCamera").Current = false;
            GetNode<Control>("UI").Show();
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
        Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	internal override void OnUpdate(double delta)
	{
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetNode<ProgressBar>("UI/EnergyBar").Value = energy;
            if (Input.IsActionJustPressed("scp_map"))
            {
                if (GetParent().GetParent().GetParent().GetNode<PlayerUI>("PlayerUI").SpecialScreen)
                {
                    GetParent().GetParent().GetParent().GetNode<PlayerUI>("PlayerUI").SpecialScreen = false;
                    GetNode<Control>("UI/MapGenHUD").Visible = false;
                }
                else
                {
                    GetParent().GetParent().GetParent().GetNode<PlayerUI>("PlayerUI").SpecialScreen = true;
                    GetNode<Control>("UI/MapGenHUD").Visible = true;
                }
            }
            if (energy < 100)
            {
                energy += 0.01f;
            }
        }
	}
    /// <summary>
    /// Switches camera.
    /// </summary>
    /// <param name="zone">Specified zone</param>
    /// <param name="cam">name of the room (temporary)</param>
    internal override void SwitchCamera(string zone, string cam)
    {
        if (currentCam != "")
        {
            GetTree().Root.GetNode<CctvCamera>(currentCam).Initialize(false, this);
        }
        currentCam = "Main/Game/MapGen" + zone +"/" + cam + "/cctv";
        if (GetTree().Root.GetNodeOrNull<CctvCamera>(currentCam) != null)
        {
            GetTree().Root.GetNode<CctvCamera>(currentCam).Initialize(true, this);
        }
        else
        {
            currentCam = "Main/Game/MapGenLcz/LC_cont1_079/cctv";
            GetTree().Root.GetNode<CctvCamera>(currentCam).Initialize(true, this);
        }
    }
}
