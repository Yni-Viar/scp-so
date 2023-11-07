using Godot;
using System;

public partial class FoundationTasksManager : Node
{
    //to be added in future

    //[Signal]
    //public delegate void TaskIsDoneEventHandler();
    [Export] string scientistsTask;
    [Export] byte tasksLeft = 2;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CheckTask();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void CheckTask()
    {
        if (string.IsNullOrEmpty(scientistsTask))
        {
            AddNewTask();
        }
        if (GetParent<FacilityManager>().IsContainmentBreach)
        {
            AddNewTask("cb");
        }
    }

    void AddNewTask(string type = "random")
    {
        if (type == "random")
        {
            RandomNumberGenerator rng = new RandomNumberGenerator();
            scientistsTask = rng.RandiRange(0, 1).ToString();
        }
        else
        {
            scientistsTask = type;
        }
    }
}
