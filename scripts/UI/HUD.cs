using Godot;
using System;

public partial class HUD : Control
{
    public static bool OpenMenu { get; set; } = true;

    public PackedScene PauseMenuScene = GD.Load<PackedScene>("res://scenes/PauseMenu.tscn");

    public override void _Ready()
    {
        FPSCounter = GetNode<Label>("FPS");

        this.ProcessMode = ProcessModeEnum.Always;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("pause"))
            PauseGame();
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
