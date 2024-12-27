using Godot;
using System;

public partial class MenuMap : Node3D
{
    private PackedScene fishScene = GD.Load<PackedScene>("res://scenes/Fish.tscn");

    public override void _Ready()
	{
        Random ran = new();
        for (int i = 20; i > 0; i--)
        {
            var fish = fishScene.Instantiate<Fish>();
            fish.Position = new Vector3(ran.Next(0, 100), ran.Next(1, 100), ran.Next(0, 100));
            AddChild(fish);
        }
    }
}
