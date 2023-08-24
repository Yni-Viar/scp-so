/* using Godot;
using System;


// As of 0.4.0-dev the JSON parser is unused.

public partial class ClassParser : Node
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public static Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> ReadJson(string data)
    {
        if (FileAccess.FileExists(data))
        {
            FileAccess file = FileAccess.Open(data, FileAccess.ModeFlags.Read);
            Variant parsedResult = Json.ParseString(file.GetAsText());
            return (Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>>)parsedResult;
        }
        else
        {
            GD.Print("The data isn't saved into userdata. Saving...");
            Variant jsonData = Json.Stringify(ClassData.playerClasses);
            FileAccess file = FileAccess.Open(data, FileAccess.ModeFlags.Write);
            file.StoreLine((string)jsonData);
            file.Close();
            return ClassData.playerClasses;
        }
    }
}
*/