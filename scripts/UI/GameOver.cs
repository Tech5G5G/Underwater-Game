using Godot;
using System;

public partial class GameOver : Control
{
	public Button Resume;
	public Button Menu;

	public override void _Ready()
	{
		var tree = GetTree();
		tree.Paused = true;

		(Resume = GetNode<Button>("Control/Respawn")).Pressed += () =>
		{ 
            tree.Paused = false;
			tree.ChangeSceneToFile("res://scenes/map.tscn");
		};
		(Menu = GetNode<Button>("Control/Menu")).Pressed += () =>
		{
            player.HealthPower = null;
            tree.Paused = false;
			tree.ChangeSceneToFile("res://scenes/MainMenu.tscn");
		};

        Input.MouseMode = Input.MouseModeEnum.Visible;
		this.ProcessMode = ProcessModeEnum.Always;
	}
}
