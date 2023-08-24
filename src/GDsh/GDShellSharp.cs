using Godot;
using System;

public partial class GDShellSharp : Node
{
    /*
    GDsh console singleton
    This singleton used for executing, adding or deleting commands.[br]
    [b]Built-in commands:[/b][br]
    [code]help[/code][br]
    [code]echo[/code][br]
    [code]gdfetch[/code]
    [br][br]
    Use [method add_command] to add new commands and [method remove_command] to delete them
    */
    
    // The list of commands that can be executed along with associated callables
    Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> commands;
        
    
    PackedScene console = GD.Load<PackedScene>("res://GDsh/InGameConsole.tscn");
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        commands = new Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>>{
            {"help", new Godot.Collections.Dictionary<string, Variant>{
                {"callable", new Callable(this, "Help")}, //},
                {"description", "Print the list of available commands"},
            }},
            {"echo", new Godot.Collections.Dictionary<string, Variant>{
                {"callable", new Callable(this, "Echo")},
                {"description", "Print given input"},
            }},
            {"system_info", new Godot.Collections.Dictionary<string, Variant>{
                {"callable", new Callable(this, "SystemInfo")},
                {"description", "Display system information"},
            }},
        };
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    /*public override void _Process(double delta)
	{
	}*/
    /// <summary>
    /// Calls command.
    /// </summary>
    /// <param name="command">Name of the command</param>
    /// <param name="args">It's arguments (if it has)</param>
    /// <returns>Success or failure of calling</returns>
    internal Variant CallCommand(string command, string[] args)
    {
        if (commands.ContainsKey(command))
        {
            return commands[command]["callable"].AsCallable().Call(args);
        }
        else
        {
            return "No such command: " + command;
        }
    }
    /// <summary>
    /// Adds new command.
    /// </summary>
    /// <param name="name">Name of the command</param>
    /// <param name="callable">Which method it calls</param>
    /// <param name="descr">Description</param>
    internal void AddCommand(string name, Callable callable, string descr = "")
    {
        commands[name] = new Godot.Collections.Dictionary<string, Variant>{
            {"callable", callable},
            {"description", descr},
        };
    }
    /// <summary>
    /// Removes command
    /// </summary>
    /// <param name="name">Name of the command</param>
    /// <returns>Removes command</returns>
    bool RemoveCommand(string name)
    {
        return commands.Remove(name);
    }
    /// <summary>
    /// Returns game version.
    /// </summary>
    /// <returns>Returns game version.</returns>
    string GetVersionInfo()
    {
        return Globals.version;
    }
    /// <summary>
    /// Built-in command.
    /// </summary>
    /// <param name="args">Unused, but is necessary for GDSh</param>
    /// <returns>List of commands</returns>
    string Help(string[] args)
    {
        string r = "";
        r += "[ul]";
        foreach (var command in commands)
        {
            r += command.Key + " " + commands[command.Key]["description"] + "\n";
        }
        r += "[/ul]";
        return r;
    }
    /// <summary>
    /// Built-in command.
    /// </summary>
    /// <param name="args">Command's args</param>
    /// <returns>Echoes the command</returns>
    string Echo(string[] args)
    {
        return " " + args;
    }
    /// <summary>
    /// Built-in command.
    /// </summary>
    /// <param name="args">Unused, but is necessary for GDSh</param>
    /// <returns>System info (such as game version, version of Godot Engine and uptime)</returns>
    string SystemInfo(string[] args)
    {
        string r = "";
        string userName = OS.GetEnvironment("USERNAME") + "@" + ProjectSettings.GetSetting("application/config/name");
        ulong uptime = Time.GetTicksMsec() / 1000;
        string uptimeStr = "";
        if (uptime > 3599)
        {
            uptimeStr += (Mathf.FloorToInt(uptime / 3600)).ToString() + " hours ";
        }
        if (uptime > 59)
        {
            uptimeStr += (Mathf.FloorToInt(uptime % 3600 / 60)).ToString() + " minutes ";
        }
        uptimeStr += (Mathf.FloorToInt(uptime % 60)).ToString() + " seconds";

        Godot.Collections.Dictionary<string, string> stats =
            new Godot.Collections.Dictionary<string, string>{
                {"OS", OS.GetName()},
                {"Host", OS.GetModelName()},
                {"Engine version", "Godot" + Engine.GetVersionInfo()},
                {"Uptime", uptimeStr},
                {"Game version", GetVersionInfo()},
                {"Resolution", (GetTree().Root.GetVisibleRect().Size).ToString()},
                {"CPU", OS.GetProcessorName()},
                {"GPU", RenderingServer.GetVideoAdapterName()},
                {"Memory", (OS.GetStaticMemoryUsage()).ToString()}
            };
        
        foreach (var line in stats)
        {
            r += line.Key + ": " + stats[line.Key] + "\n";
        }
        return r;
    }
}
