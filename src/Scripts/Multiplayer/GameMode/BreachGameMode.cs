using Godot;
using System;
/// <summary>
/// Default game-mode
/// </summary>
public partial class BreachGameMode : FacilityManager
{
    [Export] internal string omegaWarheadKeycode;
    internal string alphaWarheadKeycode = GD.Hash(Globals.version).ToString().Substr(0, 4);
    [Export] uint scpLimit = 4; //SCP Limit
    Godot.Collections.Array<int> usedScps = new Godot.Collections.Array<int>(); //Already spawned SCPs
    internal override void OnServerStart()
    {
        omegaWarheadKeycode = rng.RandiRange(1000, 9999).ToString();
        foreach (Node node in GetTree().GetNodesInGroup("WarheadCodeLabel3d"))
        {
            if (node is Label3D label)
            {
                label.Text = omegaWarheadKeycode;
            }
        }
        WaitForBeginning();
    }
    internal override void OnUpdate(double delta)
    {
        if (Multiplayer.IsServer())
        {
            if (IsRoundStarted)
            {
                RespawnMTF();
                CheckRoundEnd();
            }
        }
        if (!IsRoundStarted)
        {
            GetNode<Label>("PlayerUI/PreRoundStartPanel/PreRoundStart/Amount").Text = playersList.Count.ToString();
        }
    }

    /// <summary>
    /// Waits for people gather before the round starts.
    /// </summary>
    async void WaitForBeginning()
    {
        await ToSignal(GetTree().CreateTimer(25.0), "timeout");
        if (!IsRoundStarted)
        {
            BeginGame();
        }
        if (GetParent<NetworkManager>().spawnNpcs)
        {
            //Spawn NPC with 67% chance
            if (rng.RandiRange(0, 100) <= 67 && playersList.Count > 1)
            {
                //GetTree().Root.GetNode<PlayerAction>("Main/Game/PlayerAction").Rpc("SpawnObject", rng.RandiRange(0, data.Npc.Count - 1), 2, 1);
                int key = rng.RandiRange(0, data.Npc.Count - 1);
                GetNode<ItemManager>("NPCs").RpcId(1, "CallAddOrRemoveItem", true, key, GetTree().Root.GetNode<Node3D>("Main/Game/" + Multiplayer.GetUniqueId() + "/PlayerHead/ItemSpawn").GetPath());
            }
        }
    }

    /// <summary>
    /// Round start. Adds the players in the list and tosses their classes.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    void BeginGame()
    {
        Godot.Collections.Array<Node> players = GetTree().GetNodesInGroup("Players");
        uint i = 1;
        IsRoundStarted = true;
        foreach (Node player in players)
        {
            if (player is PlayerScript playerScript)
            {
                RpcId(int.Parse(playerScript.Name), "SetPlayerClass", playerScript.Name, TossPlayerClass(i), "Round start.", false);
                i++;
            }
        }
    }
    /// <summary>
    /// Respawns MTF e-11 "Nine-Tailed Fox" every 5 minutes.
    /// </summary>
    async void RespawnMTF()
    {
        await ToSignal(GetTree().CreateTimer(300.0), "timeout");
        foreach (string item in playersList)
        {
            PlayerScript player = GetNode<PlayerScript>(item);
            if (player.classKey == 0) //spectator
            {
                RpcId(int.Parse(item), "SetPlayerClass", item, "mtfe11", "MTF arrive.", false);
            }
        }
    }

    /// <summary>
    /// Tosses player classes at round start. Will be reworked at next update.
    /// </summary>
    /// <param name="i">Counter</param>
    /// <returns>A random class</returns>
    int TossPlayerClass(uint i)
    {
        if (i == 2 || i % 8 == 0)
        {
            if (usedScps.Count > scpLimit) //if there are more SCPs than exist - spawn a human instead.
            {
                return multiSpawnClasses[rng.RandiRange(0, multiSpawnClasses.Count - 1)];
            }
            int randomScpClass;
            if (usedScps.Count > 1 && rng.RandiRange(0, 7) < 2) //Spawn a friendly SCP by some chance.
            {
                randomScpClass = rng.RandiRange(0, bonusSpawnClasses.Count - 1);
                while (usedScps.Contains(randomScpClass))
                {
                    randomScpClass = rng.RandiRange(0, bonusSpawnClasses.Count - 1);
                }
                usedScps.Add(randomScpClass);
                return randomScpClass;
            }
            else
            {
                randomScpClass = rng.RandiRange(0, singleSpawnClasses.Count - 1);
                while (usedScps.Contains(randomScpClass))
                {
                    randomScpClass = rng.RandiRange(0, singleSpawnClasses.Count - 1);
                } //Spawn SCP
                usedScps.Add(randomScpClass);
                return singleSpawnClasses[randomScpClass];
            }
            
        }
        else //Spawn a human.
        {
            return multiSpawnClasses[rng.RandiRange(0, multiSpawnClasses.Count - 1)];
        }
    }
    /// <summary>
    /// Round end checker.
    /// </summary>
    async void CheckRoundEnd()
    {
        await ToSignal(GetTree().CreateTimer(2.0), "timeout");
        if (playersList.Count > 1)
        {
            if (targets == 0 && tickets[0] > 0)
            {
                Rpc("RoundEnd", 0);
            }
            if (tickets[0] == 0)
            {
                //more will be added in future
                foreach (string item in playersList)
                {
                    switch (GetNode<PlayerScript>(item).team)
                    {
                        case Globals.Team.SCI:
                            tickets[1]++;
                            break;
                        case Globals.Team.CDP:
                            tickets[2]++;
                            break;
                        case Globals.Team.MTF:
                            tickets[1]++;
                            break;
                    }
                }
                if (tickets[2] == 0 && tickets[1] > 0)
                {
                    Rpc("RoundEnd", 1);
                }
                else if (tickets[2] > 0 && tickets[1] == 0)
                {
                    Rpc("RoundEnd", 2);
                }
                else
                {
                    Rpc("RoundEnd", 3);
                }
            }
        }
        else if (GetNode<WarheadController>("OmegaWarhead").detonated)
        {
            Rpc("RoundEnd", 4);
        }
    }
    /// <summary>
    /// Round end scenario. After 15 seconds shutdowns the server.
    /// </summary>
    /// <param name="whoWon">Team, that won.</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    async void RoundEnd(int whoWon)
    {
        switch (whoWon)
        {
            case 0:
                GetNode<RichTextLabel>("PlayerUI/GameEnd").Text = "SCPs won!\nThe server will be turned off soon...";
                break;
            case 1:
                GetNode<RichTextLabel>("PlayerUI/GameEnd").Text = "Foundation won!\nThe server will be turned off soon...";
                break;
            case 2:
                GetNode<RichTextLabel>("PlayerUI/GameEnd").Text = "Class-D won!\nThe server will be turned off soon...";
                break;
            case 3:
                GetNode<RichTextLabel>("PlayerUI/GameEnd").Text = "Stalemate!\nThe server will be turned off soon...";
                break;
            case 4:
                GetNode<RichTextLabel>("PlayerUI/GameEnd").Text = "Stalemate - The warhead has been detonated!\nThe server will be turned off soon...";
                break;
            default:
                break;
        }
        GetNode<AnimationPlayer>("PlayerUI/AnimationPlayer").Play("roundend");
        SetProcess(false);
        await ToSignal(GetTree().CreateTimer(15.0), "timeout");
        GetParent<NetworkManager>().ServerDisconnected();
    }

    /// <summary>
    /// Forces round start. Available since 0.7.2
    /// </summary>
    void ForceRoundStart()
    {
        if (!IsRoundStarted)
        {
            IsRoundStarted = true;
            if (Multiplayer.IsServer())
            {
                BeginGame();
            }
            else
            {
                RpcId(1, "BeginGame");
            }
        }
    }
}
