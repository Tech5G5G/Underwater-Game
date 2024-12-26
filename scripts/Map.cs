using Godot;
using System;
using System.Diagnostics;

public partial class Map : Node3D
{
	private PackedScene fishScene = GD.Load<PackedScene>("res://models/Fish.tscn");

	public override void _Ready()
	{
        Random ran = new();
        for (int i = 10; i > 0; i--)
		{
            var fish = fishScene.Instantiate<Fish>();
            fish.Position = new Vector3(ran.Next(0, 100), ran.Next(1, 100), ran.Next(0, 100));
            AddChild(fish);
        }
	}

	public override void _Process(double delta)
	{
	}
}
