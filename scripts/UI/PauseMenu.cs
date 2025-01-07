using Godot;
using System;

public partial class PauseMenu : Control
{
    public Control Keybinds;
    public TextureRect Logo;

    public Button ToggleFPS;
    public Button ToggleFullscreen;
    public Button ShowKeybinds;
    public Button Menu;
    public Button Exit;

    public override void _Ready()
	{
        GD.Print("ADD SETTINGS BUTTON ON THE BUS!!!");

        Keybinds = GetNode<Control>("Keybinds");
        Logo = GetNode<TextureRect>("TextureRect");

        ToggleFPS = GetNode<Button>("ToggleFPS");
        ToggleFPS.ButtonUp += () => HUD.FPSCounter.Visible = !HUD.FPSCounter.Visible;

        ToggleFullscreen = GetNode<Button>("ToggleFullscreen");
        ToggleFullscreen.ButtonUp += () => HUD._ToggleFullscreen();

        ShowKeybinds = GetNode<Button>("ShowKeybinds");
        ShowKeybinds.ButtonUp += () =>
        {
            Keybinds.Visible = true;
            Logo.Visible = Logo.Visible = ToggleFPS.Visible = ToggleFullscreen.Visible = ShowKeybinds.Visible = Exit.Visible = false;
        };

        Menu = GetNode<Button>("Menu");
        Menu.ButtonUp += () =>
        {
            player.HealthPower = null;
            var tree = GetTree();
            tree.ChangeSceneToFile("res://scenes/MainMenu.tscn");
            tree.Paused = false;
        };

        Exit = GetNode<Button>("Exit");
        Exit.ButtonUp += () => GetTree().Quit();

        this.ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            if (Keybinds.Visible)
            {
                HUD.OpenMenu = false;
                Keybinds.Visible = false;
                Logo.Visible = Logo.Visible = ToggleFPS.Visible = ToggleFullscreen.Visible = ShowKeybinds.Visible = Menu.Visible = Exit.Visible = true;
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
                GetTree().Paused = false;
                HUD.OpenMenu = false;
                Free();
            }
        }
    }
}
