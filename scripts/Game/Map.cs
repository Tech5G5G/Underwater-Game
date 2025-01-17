using Godot;
using System;

public partial class Map : Node3D
{
    private PackedScene blueFishScene = GD.Load<PackedScene>("res://fishes/BlueFish.tscn");
    private PackedScene greenFishScene = GD.Load<PackedScene>("res://fishes/GreenFish.tscn");
    private PackedScene epikFishScene = GD.Load<PackedScene>("res://fishes/EpikFish.tscn");
    private PackedScene nextBotFishScene = GD.Load<PackedScene>("res://fishes/BotFish.tscn");

    public override void _Ready()
    {
        Random ran = new();
        for (int i = 20; i > 0; i--)
        {
            Fish fish = null;
            switch (ran.Next(1, 1001))
            {
                case <= 939:
                    fish = blueFishScene.Instantiate<Fish>();
                    fish.Type = FishType.Default;
                    break;
                case <= 989:
                    fish = greenFishScene.Instantiate<Fish>();
                    fish.Type = FishType.QuickBoi;
                    break;
                case <= 999:
                    fish = epikFishScene.Instantiate<Fish>();
                    fish.Type = FishType.Epik;
                    break;
                case 1000:
                    fish = nextBotFishScene.Instantiate<Fish>();
                    fish.Type = FishType.NextBot;
                    break;
                default:
                    throw new IndexOutOfRangeException("Number is not between 1 and 1000.");
            };
            fish.Position = new Vector3(ran.Next(0, 100), ran.Next(1, 100), ran.Next(0, 100));
            AddChild(fish);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("toggle_fullscreen"))
            SettingsWindow.ToggleFullscreen();
    }
}
