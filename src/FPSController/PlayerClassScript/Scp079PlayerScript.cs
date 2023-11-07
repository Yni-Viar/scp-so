using Godot;
using System;

public partial class Scp079PlayerScript : Node3D
{
    string currentCam = "";
    short counterPeople = 0;
    short counterScps = 0;
    [Export(PropertyHint.Range, "0,100,0.01")] internal float energy = 100f;
    public override void _Ready()
    {
        GetParent().GetParent<PlayerScript>().CanMove = false;
        GetParent().GetParent<PlayerScript>().SetCollisionMaskValue(3, true);
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetParent().GetParent<PlayerScript>().GetNode<Camera3D>("PlayerHead/PlayerCamera").Current = false;
            GetNode<Control>("UI").Show();
        }
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetNode<ProgressBar>("UI/EnergyBar").Value = energy;
            if (Input.IsActionJustPressed("scp079_map"))
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
            if (Input.IsActionJustPressed("scp079_door_lock"))
            {
                foreach (Node item in GetTree().Root.GetNode<Area3D>(currentCam).GetChildren())
                {
                    if (item is Door door)
                    {
                        door.Rpc("DoorLock");
                        energy -= 50f;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Switches camera.
    /// </summary>
    /// <param name="zone">Specified zone</param>
    /// <param name="cam">name of the room (temporary)</param>
    internal void SwitchCamera(string zone, string cam)
    {
        if (currentCam != "")
        {
            //GetTree().Root.GetNode<CctvCamera>(currentCam).Initialize(false, this);
            GetTree().Root.GetNode<Area3D>(currentCam + "/CheckArea").Monitoring = false;
            GetTree().Root.GetNode<Area3D>(currentCam + "/CheckArea").BodyEntered -= OnBodyEntered;
            counterPeople = 0;
            counterScps = 0;
        }
        currentCam = "Main/Game/MapGen" + zone + "/" + cam;
        if (GetTree().Root.GetNodeOrNull(currentCam) != null)
        {
            GetTree().Root.GetNode<Area3D>(currentCam + "/CheckArea").Monitoring = true;
            GetTree().Root.GetNode<Area3D>(currentCam + "/CheckArea").BodyEntered += OnBodyEntered;
        }
        else
        {
            currentCam = "Main/Game/MapGenLcz/LC_cont1_079";
            GetTree().Root.GetNode<Area3D>(currentCam + "/CheckArea").Monitoring = true;
            GetTree().Root.GetNode<Area3D>(currentCam + "/CheckArea").BodyEntered += OnBodyEntered;
        }
        ShowInfo(cam, counterPeople, counterScps);
    }

    void OnBodyEntered(Node3D body)
    {
        if (body is PlayerScript player)
        {
            if (player.scpNumber == -1)
            {
                counterPeople++;
            }
            else if (player.scpNumber >= 0)
            {
                counterScps++;
            }
        }
    }

    void ShowInfo(string roomName, short peopleQuantity, short scpQuantity)
    {
        Label label = GetNode<Label>("UI/Info");
        label.Text = "Room name: " + roomName + "\nQuantity of people: " + peopleQuantity + "\nQuantity of SCPs: " + scpQuantity + "\nClick [L] to lock all doors";
    }
}