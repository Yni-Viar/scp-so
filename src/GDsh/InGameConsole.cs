using Godot;
using System;

public partial class InGameConsole : Control
{
	Godot.Collections.Array<string> history = new Godot.Collections.Array<string>();
	int currentCommand = 0;
	RichTextLabel log;
	Label tip;
	CodeEdit prompt;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		log = GetNode<RichTextLabel>("VBox/Log");
		tip = GetNode<Label>("Tooltip");
		prompt = GetNode<CodeEdit>("VBox/HBox/Prompt");
		prompt.GrabFocus();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{
	}*/

	void SendCommand(string text)
	{
		history.Add(text);
		currentCommand = history.Count;
		string[] list = text.StripEdges().Split(" ");
		string command = list[0];
		string[] args;
		if (list.Length == 2)
		{
			args = new string[]{list[1]};
		}
        else if (list.Length > 2)
        {
            args = list[1..(list.Length - 1)];
        }
		else
		{
			args = new string[]{""};
		}
		log.Text += ("[color=yellow]" + command + "[/color] [color=gray]" + " " + args + "[/color] \n");
		Variant result = GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").CallCommand(command, args);
		log.Text += result + "\n";
	}
	
	private void OnLogMetaClicked(Variant meta)
    {
        OS.ShellOpen(meta.AsString());
    }

    private void OnLogMetaHoverStarted(Variant meta)
    {
        tip.Text = meta.AsString();
        tip.GlobalPosition = GetViewport().GetMousePosition() + new Vector2(16, 16);
        tip.Show();
    }

    private void OnLogMetaHoverEnded(Variant meta)
    {
        tip.Hide();
        tip.Text = "";
    }

    private void OnLogFocusEntered()
    {
        prompt.GrabFocus();
    }

    private void OnPromptGuiInput(InputEvent @event)
    {
        if (@event is InputEventKey key && @event.IsPressed())
        {
            if (key.Keycode == Key.Enter)
            {
                GetTree().Root.SetInputAsHandled();
                OnSubmitPressed();
            }
            else if (key.Keycode == Key.Up)
            {
                GetTree().Root.SetInputAsHandled();
                if (currentCommand > 0)
                {
                    currentCommand--;
                    prompt.Text = history[currentCommand];
                }
            }
            else if (key.Keycode == Key.Down)
            {
                GetTree().Root.SetInputAsHandled();
                if (currentCommand < history.Count)
                {
                    currentCommand++;
                    if (currentCommand < history.Count)
                    {
                        prompt.Text = history[currentCommand];
                    }
                    else
                    {
                        prompt.Text = "";
                    }
                }
            }
        }
    }

    private void OnSubmitPressed()
    {
        prompt.GrabFocus();
        if (prompt.Text != "")
        {
            SendCommand(prompt.Text);
        }
    }
}


