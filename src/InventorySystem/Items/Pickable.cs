using Godot;
using System;
public partial class Pickable : RigidBody3D
{
    [Export] internal Item itemResource;
    PlayerScript holder;
    // Note for Yni: if the RigidBody keeps falling, check rotation!
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{
	}*/
    /// <summary>
    /// Main method of picking. Calls helper method to add an item in inventory
    /// </summary>
    /// <param name="player">Player to add an item</param>
    internal void PickUpItem(PlayerScript player)
    {
        Inventory inv = player.GetNode<Inventory>("InventoryContainer/Inventory");
        AudioStreamPlayer3D interactSound = player.GetNode<AudioStreamPlayer3D>("InteractSound");
        interactSound.Stream = ResourceLoader.Load<AudioStream>(itemResource.PickupSoundPath);
        interactSound.Play();
        if (!Multiplayer.IsServer())
        {
            RpcId(Multiplayer.GetUniqueId(), "PickUpRpc", inv.GetPath());
        }
        else
        {
            PickUpRpc(inv.GetPath());
        }
    }
    /// <summary>
    /// Helper method to add an item
    /// </summary>
    /// <param name="invPath">Path to player inventory</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void PickUpRpc(string invPath)
    {
        // This opens JSON, find item and add it to inventory.
        if (ItemParser.ReadJson("user://itemlist_" + Globals.itemsCompatibility + ".json").ContainsKey(itemResource.InternalName))
        {
            //after trimming, we will get an item from json.
            GetNode<Inventory>(invPath).AddItem(ResourceLoader.Load(ItemParser.ReadJson("user://itemlist_" + Globals.itemsCompatibility + ".json")[SceneFilePath.TrimSuffix(".tscn").TrimPrefix("res://InventorySystem/Items/PickablePrefabs/")]));
            Rpc("DestroyPickedItem");

        }
        else
        {
            GD.PrintErr("Unknown item. Cannot add to inventory.");
        }
    }
    /// <summary>
    /// Removes item in the map after picking.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void DestroyPickedItem()
    {
        this.QueueFree();
    }
}
