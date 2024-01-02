using Godot;
using System;


public partial class ItemParser : Node
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
    /// <returns>Result</returns>
    public static Godot.Collections.Dictionary<string, string> ReadJson(string placeToSave, Globals.ItemType type)
    {
        if (FileAccess.FileExists(placeToSave))
        {
            FileAccess file = FileAccess.Open(placeToSave, FileAccess.ModeFlags.Read);
            Variant parsedResult = Json.ParseString(file.GetAsText());
            file.Close();
            return (Godot.Collections.Dictionary<string, string>)parsedResult;
        }
        else
        {
            switch (type)
            {
                case Globals.ItemType.item:
                    return SaveJson(placeToSave, Globals.items);
                case Globals.ItemType.ammo:
                    return SaveJson(placeToSave, Globals.ammo);
                case Globals.ItemType.npc:
                    return SaveJson(placeToSave, Globals.npcs);
                default:
                    GD.Print("Error! Type of object is unknown. Returning null...");
                    return null;
            }
        }
    }
    /// <summary>
    /// Saves item JSON
    /// </summary>
    /// <param name="placeToSave">Place to save JSON</param>
    /// <param name="data">item data</param>
    /// <returns>Default data</returns>
    public static Godot.Collections.Dictionary<string, string> SaveJson(string placeToSave, Godot.Collections.Dictionary<string, string> data)
    {
        GD.Print("The data isn't saved into userdata. Saving...");
        Variant jsonData = Json.Stringify(data);
        FileAccess file = FileAccess.Open(placeToSave, FileAccess.ModeFlags.Write);
        file.StoreLine((string)jsonData);
        file.Close();
        return data;
    }
}