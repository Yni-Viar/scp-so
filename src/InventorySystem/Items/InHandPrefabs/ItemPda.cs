using Godot;
using System;

public partial class ItemPda : ItemAction
{
    string guiPosition = "MainScreen";
    internal override void OnUpdate(double delta)
    {
        //shitcode (please help D: )
        if (Input.IsActionJustPressed("pda_map"))
        {
            GetNode<Control>("SubViewport/GUI/" + guiPosition).Visible = false;
            guiPosition = "Map";
            GetNode<Control>("SubViewport/GUI/" + guiPosition).Visible = true;
        }
        if (Input.IsActionJustPressed("pda_fastkeycode"))
        {
            GetNode<Control>("SubViewport/GUI/" + guiPosition).Visible = false;
            guiPosition = "FastKeycode";
            GetNode<Control>("SubViewport/GUI/" + guiPosition).Visible = true;
        }
        if (Input.IsActionJustPressed("pda_back"))
        {
            GetNode<Control>("SubViewport/GUI/" + guiPosition).Visible = false;
            guiPosition = "MainScreen";
            GetNode<Control>("SubViewport/GUI/" + guiPosition).Visible = true;
        }
    }
}
