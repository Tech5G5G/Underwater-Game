using Godot;
using System;

public partial class PauseMenu : Control
{
    public Button Resume;
    public Button List;
    public Button Settings;
    public Button Menu;
    public Button Exit;

    PackedScene listWindow = GD.Load<PackedScene>("res://scenes/List.tscn");
    PackedScene settingsWindow = GD.Load<PackedScene>("res://scenes/SettingsWindow.tscn");

    public override void _Ready()
    {
        (Resume = GetNode<Button>("Resume")).Pressed += () => CloseMenu();
        (Exit = GetNode<Button>("Exit")).Pressed += () => GetTree().Quit();
        (List = GetNode<Button>("List")).Pressed += () =>
        {
            var window = listWindow.Instantiate<Window>();
            AddChild(window);
            window.Show();
        };
        (Settings = GetNode<Button>("Settings")).Pressed += () =>
        {
            var window = settingsWindow.Instantiate<Window>();
            AddChild(window);
            window.Show();
        };
        (Menu = GetNode<Button>("Menu")).Pressed += () =>
        {
            player.HealthPower = null;
            var tree = GetTree();
            tree.ChangeSceneToFile("res://scenes/MainMenu.tscn");
            tree.Paused = false;
        };

        this.ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("pause"))
            CloseMenu();
    }

    private void CloseMenu()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GetTree().Paused = false;
        HUD.OpenMenu = false;
        QueueFree();
    }
}
