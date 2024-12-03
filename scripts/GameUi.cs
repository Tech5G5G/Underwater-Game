using Godot;
using System;

public partial class GameUi : Control
{
    public Label FPSCounter;
    public Control PauseMenu;
    public Control Keybinds;
    public TextureRect Logo;

    public Button ToggleFPS;
    public Button ToggleFullscreen;
    public Button ShowKeybinds;
    public Button Exit;

    public override void _Ready()
	{
        FPSCounter = GetNode<Label>("FPS");
        PauseMenu = GetNode<Control>("PauseMenu");
        Keybinds = GetNode<Control>("PauseMenu/Keybinds");
        Logo = GetNode<TextureRect>("PauseMenu/TextureRect");

        ToggleFPS = PauseMenu.GetNode<Button>("ToggleFPS");
        ToggleFPS.ButtonUp += () => FPSCounter.Visible = !FPSCounter.Visible;

        ToggleFullscreen = PauseMenu.GetNode<Button>("ToggleFullscreen");
        ToggleFullscreen.ButtonUp += () => _ToggleFullscreen();

        ShowKeybinds = PauseMenu.GetNode<Button>("ShowKeybinds");
        ShowKeybinds.ButtonUp += () =>
        {
            Keybinds.Visible = true;
            Logo.Visible = Logo.Visible = ToggleFPS.Visible = ToggleFullscreen.Visible = ShowKeybinds.Visible = Exit.Visible = false;
        };

        Exit = PauseMenu.GetNode<Button>("Exit");
        Exit.ButtonUp += () => GetTree().Quit();

        this.ProcessMode = ProcessModeEnum.Always;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            if (Keybinds.Visible)
            {
                Keybinds.Visible = false;
                Logo.Visible = Logo.Visible = ToggleFPS.Visible = ToggleFullscreen.Visible = ShowKeybinds.Visible = Exit.Visible = true;
            }
            else
            {
                var toggle = PauseMenu.Visible;

                if (!toggle)
                {
                    PauseMenu.Visible = !toggle;
                    Input.MouseMode = Input.MouseModeEnum.Visible;
                    GetTree().Paused = true;
                }
                else
                {
                    PauseMenu.Visible = !toggle;
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                    GetTree().Paused = false;
                }
            }
        }

        if (Input.IsActionJustPressed("toggle_fullscreen"))
            _ToggleFullscreen();
    }

    public void _ToggleFullscreen() => DisplayServer.WindowSetMode(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen ? DisplayServer.WindowMode.Windowed : DisplayServer.WindowMode.ExclusiveFullscreen);

    public override void _Process(double delta)
	{
        if (FPSCounter.Visible)
            FPSCounter.Text = $"FPS: {Engine.GetFramesPerSecond()}";
    }
}
