using Godot;
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

    public static Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> ReadJson(string placeToSave)
    {
        if (FileAccess.FileExists(placeToSave))
        {
            FileAccess file = FileAccess.Open(placeToSave, FileAccess.ModeFlags.Read);
            Variant parsedResult = Json.ParseString(file.GetAsText());
            return (Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>)parsedResult;
        }
        else
        {
            return SaveJson(placeToSave, DefaultClassList.classData);
        }
    }

    public static Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> SaveJson(string placeToSave, Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> data)
    {
        GD.Print("The data isn't saved into userdata. Saving...");
        Variant jsonData = Json.Stringify(data);
        FileAccess file = FileAccess.Open(placeToSave, FileAccess.ModeFlags.Write);
        file.StoreLine((string)jsonData);
        file.Close();
        return data;
    }
}