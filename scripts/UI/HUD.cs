using Godot;
using System;

public partial class HUD : Control
{
    public static Label FPSCounter { get; set; }
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
        {
            if (!OpenMenu)
            {
                OpenMenu = true;
                return;
            }

            AddChild(PauseMenuScene.Instantiate<PauseMenu>());
            Input.MouseMode = Input.MouseModeEnum.Visible;
            GetTree().Paused = true;
        }
    }

    public override void _Process(double delta)
    {
        if (FPSCounter.Visible)
            FPSCounter.Text = $"FPS: {Engine.GetFramesPerSecond()}";
    }
}
