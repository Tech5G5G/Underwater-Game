using Godot;
using System;
using System.Diagnostics;

public partial class Map : Node3D
{
	private PackedScene fishScene = GD.Load<PackedScene>("res://models/Fish.tscn");

	public override void _Ready()
	{
		var timer = new Timer() { OneShot = false };
		AddChild(timer);
		timer.Timeout += () =>
		{
			var fish = fishScene.Instantiate<Fish>();
			Random ran = new();

			fish.Position = new Vector3(ran.Next(0, 20), ran.Next(1, 20), ran.Next(0, 20));
			AddChild(fish);
		};
		timer.Start(2);

		//Temp code to disable fog
		var environment = GetNode<WorldEnvironment>("WorldEnvironment");
		environment.Environment.VolumetricFogEnabled = false;
	}

	public override void _Process(double delta)
	{
	}
}
