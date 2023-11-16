using Godot;
using System;


public partial class TaskParser : Node
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    /// <summary>
    /// Reads item JSON
    /// </summary>
    /// <param name="placeToSave">Place to read (or save if not exist or version changed) JSON</param>
    /// <returns></returns>
    public static Godot.Collections.Array<string> ReadJson(string placeToSave)
    {
        if (FileAccess.FileExists(placeToSave))
        {
            FileAccess file = FileAccess.Open(placeToSave, FileAccess.ModeFlags.Read);
            Variant parsedResult = Json.ParseString(file.GetAsText());
            return (Godot.Collections.Array<string>)parsedResult;
        }
        else
        {
            return SaveJson(placeToSave, Globals.FoundationTasks);
        }
    }
    /// <summary>
    /// Saves item JSON
    /// </summary>
    /// <param name="placeToSave">Place to save JSON</param>
    /// <param name="data">item data</param>
    /// <returns>Default data</returns>
    public static Godot.Collections.Array<string> SaveJson(string placeToSave, Godot.Collections.Array<string> data)
    {
        GD.Print("The data isn't saved into userdata. Saving...");
        Variant jsonData = Json.Stringify(data);
        FileAccess file = FileAccess.Open(placeToSave, FileAccess.ModeFlags.Write);
        file.StoreLine((string)jsonData);
        file.Close();
        return data;
    }
}