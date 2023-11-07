using Godot;
using System;

public partial class TaskResource : Resource
{
    [Export]
    public string TaskName { get; set; }

    [Export(PropertyHint.MultilineText)]
    public string TaskDescription { get; set; }

    [Export]
    public string[] Keywords { get; set; }
    // Make sure you provide a parameterless constructor.
    // In C#, a parameterless constructor is different from a
    // constructor with all default values.
    // Without a parameterless constructor, Godot will have problems
    // creating and editing your resource via the inspector.
    public TaskResource() : this(null, null, null) { }

    public TaskResource(string taskName, string taskDescription, string[] keywords)
    {
        TaskName = taskName;
        TaskDescription = taskDescription;
        Keywords = keywords ?? System.Array.Empty<string>();
    }
}
