using Godot;
using System;

public partial class Map : Node3D
{
	private PackedScene blueFishScene = GD.Load<PackedScene>("res://fishes/BlueFish.tscn");
    private PackedScene greenFishScene = GD.Load<PackedScene>("res://fishes/GreenFish.tscn");
    private PackedScene epikFishScene = GD.Load<PackedScene>("res://fishes/EpikFish.tscn");

    private PackedScene settingsWindow = GD.Load<PackedScene>("res://scenes/SettingsWindow.tscn");

	public override void _Ready()
	{
        Random ran = new();
        for (int i = 20; i > 0; i--)
		{
            Fish fish = ran.Next(1, 8) switch
            {
                <= 3 => blueFishScene.Instantiate<Fish>(),
                <= 6 => greenFishScene.Instantiate<Fish>(),
                7 => epikFishScene.Instantiate<Fish>(),
                _ => throw new IndexOutOfRangeException("Number is not between 1 and 8.")
            };
            fish.Position = new Vector3(ran.Next(0, 100), ran.Next(1, 100), ran.Next(0, 100));
            AddChild(fish);
        }
	}
}
