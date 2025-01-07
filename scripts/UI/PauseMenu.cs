using Godot;
using System;

public partial class PauseMenu : Control
{
    public Button Resume;
    public Button List;
    public Button Settings;
    public Button Menu;
    public Button Exit;

    PackedScene settingsWindow = GD.Load<PackedScene>("res://scenes/SettingsWindow.tscn");

    public override void _Ready()
    {
        (Resume = GetNode<Button>("Resume")).ButtonUp += () => CloseMenu();
        (List = GetNode<Button>("List")).ButtonUp += () => {/*Show research list here*/};
        (Exit = GetNode<Button>("Exit")).ButtonUp += () => GetTree().Quit();
        (Settings = GetNode<Button>("Settings")).ButtonUp += () =>
        {
            var window = settingsWindow.Instantiate<Window>();
            AddChild(window);
            window.Show();
        };
        (Menu = GetNode<Button>("Menu")).ButtonUp += () =>
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
        Free();
    }
}
