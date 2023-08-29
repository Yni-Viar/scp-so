/*using Godot;
using System;

public partial class TxtParser : Node //It was only used in 0.5.0-dev
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
*/