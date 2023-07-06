using Godot;
using System;

public partial class JsonParser : Node
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public static Godot.Collections.Dictionary<string, string> ReadJson(string itemData)
    {
        if (FileAccess.FileExists(itemData))
        {
            FileAccess file = FileAccess.Open(itemData, FileAccess.ModeFlags.Read);
            Variant parsedResult = Json.ParseString(file.GetAsText());
            return (Godot.Collections.Dictionary<string, string>)parsedResult;
        }
        else
        {
            GD.Print("The data isn't saved into userdata. Saving...");
            Variant jsonData = Json.Stringify(ItemList.items);
            FileAccess file = FileAccess.Open(itemData, FileAccess.ModeFlags.Write);
            file.StoreLine((string)jsonData);
            file.Close();
            return ItemList.items;
        }
    }
}
