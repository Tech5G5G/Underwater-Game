using Godot;
using System;

public partial class HUD : Control
{
    public static bool OpenMenu { get; set; } = true;

    PackedScene listWindow = GD.Load<PackedScene>("res://scenes/List.tscn");
    public PackedScene PauseMenuScene = GD.Load<PackedScene>("res://scenes/PauseMenu.tscn");

    public override void _Ready()
    {
        this.ProcessMode = ProcessModeEnum.Always;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("pause"))
            PauseGame();

        if (Input.IsActionJustPressed("open_list"))
        {
            var tree = GetTree();
            Input.MouseMode = Input.MouseModeEnum.Visible;
            var window = listWindow.Instantiate<Window>();
            window.CloseRequested += () =>
            {
                tree.Paused = false;
                Input.MouseMode = Input.MouseModeEnum.Captured;
            };
            AddChild(window);
            window.Show();
            tree.Paused = true;
        }
    }

    private void PauseGame()
    {
        if (!OpenMenu)
        {
            OpenMenu = true;
            return;
        }

        var menu = PauseMenuScene.Instantiate<PauseMenu>();
        AddChild(menu);

        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().Paused = true;
    }
}
