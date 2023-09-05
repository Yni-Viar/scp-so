using Godot;
using System;

public partial class TxtParser : Node
{

    public static void Save(string path, string content)
    {
        FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        file.StoreString(content);
    }

    public static string Load(string path)
    {
        FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        string content = file.GetAsText();
        return content;
    }
}