using Godot;
using System;

public partial class InventorySlotDisplay : CenterContainer
{
	Inventory inventory = new Inventory();
	TextureRect itemTextureRect;
	bool itemHover = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//The main inventory script is a node, so you need to assign inventory to a node...
		inventory = GetParent().GetParent().GetParent().GetNode<Inventory>("Inventory");
		itemTextureRect = GetNode<TextureRect>("ItemTextureRect");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GetParent().GetParent().GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
		{
			// Use your item
			if (Input.IsActionJustPressed("interact") && itemHover)
			{
				int itemIndex = GetIndex();
				Resource item = inventory.GetItem(itemIndex);
				if (item is Item _item)
				{
					/*if (_item.OneTimeUse == true) //old code from v0.2.0 development... But I will use Hubert's method of using items.
					{
						inventory.RemoveItem(itemIndex, false);
					}
					_item.OnUsed((PlayerScript)(GetTree().GetNodesInGroup("Players")[0]));*/
					//Node3D firstPersonItem = (Node3D)GD.Load<PackedScene>(_item.FirstPersonPrefabPath).Instantiate();
					//firstPersonItem.Position = GetParent().GetParent().GetParent().GetParent().GetParent<PlayerScript>().GetNode<Marker3D>("PlayerHead/PlayerHand").GlobalPosition;

					GetParent().GetParent().GetParent().GetParent<PlayerScript>().UsingItem = _item.InternalName;
                    GetParent().GetParent().GetParent().GetParent<PlayerScript>().GetNode<ColorRect>("InventoryContainer").Hide();
				}
				else
				{
                    GetParent().GetParent().GetParent().GetParent<PlayerScript>().UsingItem = null;
                    GetParent().GetParent().GetParent().GetParent<PlayerScript>().GetNode<ColorRect>("InventoryContainer").Hide();
                }
			}
			if (Input.IsActionJustPressed("inventory_remove_item") && itemHover)
			{
				int itemIndex = GetIndex();
				//Resource item = inventory.RemoveItem(itemIndex, true);
			    inventory.RemoveItem(itemIndex, true);
			}
		}
		
	}

	internal void DisplayItem(Resource item)
	{
		if (item is Item _item)
		{
			itemTextureRect.Texture = _item.Texture;
		}
		else
		{
			itemTextureRect.Texture = null;
		}
	}

	//Methods below are used for dragging items
	public override Variant _GetDragData(Vector2 atPosition)
	{
		
		int itemIndex = GetIndex();
		Resource item = inventory.GetItem(itemIndex);
		if (item is Item _item)
		{
			Godot.Collections.Dictionary data = new Godot.Collections.Dictionary();
			data["item"] = _item;
			data["itemIndex"] = itemIndex;
			TextureRect dragPreview = new TextureRect();
			dragPreview.Texture = _item.Texture;
			SetDragPreview(dragPreview);
			inventory.dragData = data;
			return data;
		}
		else
		{
			return default;
		}
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		if (data.VariantType == Variant.Type.Dictionary)
		{
			Godot.Collections.Dictionary _data = (Godot.Collections.Dictionary)data;
			return (_data.ContainsKey("item"));
		}
		else
		{
			return false; // This is a workaround. :(
		}
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		if (data.VariantType == Variant.Type.Dictionary)
		{
			Godot.Collections.Dictionary _data = (Godot.Collections.Dictionary)data;
			int myItemIndex = GetIndex();
			Resource myItem = inventory.items[myItemIndex];
			inventory.SwapItems(myItemIndex, (int)_data["itemIndex"]);
			inventory.SetItem(myItemIndex, (Godot.Resource)_data["item"]);
			inventory.dragData = null;
		}
	}

	private void OnFakeButtonMouseEntered()
	{
		itemHover = true;
	}

	private void OnFakeButtonMouseExited()
	{
		itemHover = false;
	}
}