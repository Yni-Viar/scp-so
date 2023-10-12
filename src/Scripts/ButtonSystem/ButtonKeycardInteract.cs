/*
MIT License

Copyright (c) 2022 Nagi

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Godot;
using System;

public partial class ButtonKeycardInteract : StaticBody3D
{
    [Signal]
    public delegate void InteractedEventHandler(Node3D player, int keycardRequire);

    void Interact(Node3D _player)
    {
        if (_player is PlayerScript player)
        {
            //Yni's shitcode below :(
            // Regular SCPs can open up doors up to level 3, upper levels of keycards can be opened by SCP-079
            if (GetNode<PlayerScript>(player.GetPath()).scpNumber > 0)
            {
                EmitSignal("Interacted", player, 2);
                AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonSound");
                sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg");
                sfx.Play();
            }
            if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("keyomni") != null)
            {
                EmitSignal("Interacted", player, 5);
                AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonSound");
                sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg");
                sfx.Play();
            }
            else if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key5") != null)
            {
                EmitSignal("Interacted", player, 4);
                AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonSound");
                sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg");
                sfx.Play();
            }
            else if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key4") != null)
            {
                EmitSignal("Interacted", player, 3);
                AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonSound");
                sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg");
                sfx.Play();
            }
            else if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key3") != null)
            {
                EmitSignal("Interacted", player, 2);
                AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonSound");
                sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg");
                sfx.Play();
            }
            else if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key2") != null)
            {
                EmitSignal("Interacted", player, 1);
                AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonSound");
                sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg");
                sfx.Play();
            }
            else if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key1") != null)
		    {
		    	EmitSignal("Interacted", player, 0);
                AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonSound");
                sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg");
                sfx.Play();
		    }
            else
            {
                AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonSound");
                sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse2.ogg");
                sfx.Play();
            }
        }
        if (_player is CctvCamera cam)
        {
            EmitSignal("Interacted", cam, 5);
            AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonSound");
            sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg");
            sfx.Play();
        }
    }
}
