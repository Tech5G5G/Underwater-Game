using Godot;
using System;
using System.Text.Json;

public partial class Menu : Node3D
{
    public MeshInstance3D playButton;
    public Area3D playButtonCollision;
    public StandardMaterial3D playButtonMaterial;

    public MeshInstance3D settingsButton;
    public Area3D settingsButtonCollision;
    public StandardMaterial3D settingsButtonMaterial;

    public MeshInstance3D exitButton;
    public Area3D exitButtonCollision;
    public StandardMaterial3D exitButtonMaterial;

    Vector2 mousePosition = Vector2.Zero;
    public Camera3D camera;
    bool moveCamera = true;

    PackedScene loadingScene = GD.Load<PackedScene>("res://scenes/LoadingScreen.tscn");
    PackedScene settingsWindow = GD.Load<PackedScene>("res://scenes/SettingsWindow.tscn");
    PackedScene PauseMenuScene = GD.Load<PackedScene>("res://scenes/PauseMenu.tscn");

    public override void _Ready()
    {
        camera = GetParent().GetNode<Camera3D>("Camera");

        playButton = this.GetNode<MeshInstance3D>("PlayButton");
        playButtonCollision = this.GetNode<Area3D>("PlayButton/Area3D");
        playButtonMaterial = (playButton.Mesh as BoxMesh).Material as StandardMaterial3D;
        playButtonCollision.InputRayPickable = true;
        playButtonCollision.MouseEntered += () => playButtonMaterial.AlbedoColor = Colors.Gray;
        playButtonCollision.MouseExited += () => playButtonMaterial.AlbedoColor = Colors.White;
        playButtonCollision.InputEvent += (camera, @event, position, normal, shapeIdx) =>
        {
            if (@event is not InputEventMouseButton mouseEvent)
                return;

            if (mouseEvent.Pressed)
            {
                playButtonMaterial.AlbedoColor = Colors.DimGray;
                return;
            }

            playButtonMaterial.AlbedoColor = Colors.Gray;

            StartGame();
        };

        settingsButton = this.GetNode<MeshInstance3D>("SettingsButton");
        settingsButtonCollision = this.GetNode<Area3D>("SettingsButton/Area3D");
        settingsButtonMaterial = (settingsButton.Mesh as BoxMesh).Material as StandardMaterial3D;
        settingsButtonCollision.InputRayPickable = true;
        settingsButtonCollision.MouseEntered += () => settingsButtonMaterial.AlbedoColor = Colors.Gray;
        settingsButtonCollision.MouseExited += () => settingsButtonMaterial.AlbedoColor = Colors.White;
        settingsButtonCollision.InputEvent += (camera, @event, position, normal, shapeIdx) =>
        {
            if (@event is not InputEventMouseButton mouseEvent)
                return;

            if (mouseEvent.Pressed)
            {
                settingsButtonMaterial.AlbedoColor = Colors.DimGray;
                return;
            }

            settingsButtonMaterial.AlbedoColor = Colors.Gray;

            var window = settingsWindow.Instantiate<Window>();
            AddChild(window);
            window.Show();
        };

        exitButton = this.GetNode<MeshInstance3D>("ExitButton");
        exitButtonCollision = this.GetNode<Area3D>("ExitButton/Area3D");
        exitButtonMaterial = (exitButton.Mesh as BoxMesh).Material as StandardMaterial3D;
        exitButtonCollision.InputRayPickable = true;
        exitButtonCollision.MouseEntered += () => exitButtonMaterial.AlbedoColor = Colors.Gray;
        exitButtonCollision.MouseExited += () => exitButtonMaterial.AlbedoColor = Colors.White;
        exitButtonCollision.InputEvent += (camera, @event, position, normal, shapeIdx) =>
        {
            if (@event is not InputEventMouseButton mouseEvent)
                return;

            if (mouseEvent.Pressed)
            {
                exitButtonMaterial.AlbedoColor = Colors.DimGray;
                return;
            }

            exitButtonMaterial.AlbedoColor = Colors.Gray;
            GetTree().Quit();
        };

        this.ProcessMode = ProcessModeEnum.Always;

        var file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.ReadWrite);
        GameSettings gameSettings;
        if (!FileAccess.FileExists("user://settings.json"))
        {
            gameSettings = new GameSettings() { Difficulty = (int)Difficulty.Normal, InvertMouse = false, MouseSensitivity = 0.25f, WindowMode = 0 };
            file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.WriteRead);
            file.StorePascalString(JsonSerializer.Serialize(gameSettings));
            file.Close();
        }
        else
            gameSettings = JsonSerializer.Deserialize<GameSettings>(file.GetPascalString());
        GameSettings.WindowModeSetting = gameSettings.WindowMode;
        GameSettings.InvertMouseSetting = gameSettings.InvertMouse;
        GameSettings.DifficultySetting = gameSettings.Difficulty;
        GameSettings.MouseSensitivitySetting = gameSettings.MouseSensitivity;

        DisplayServer.WindowSetMode(gameSettings.WindowMode == 0 ? DisplayServer.WindowMode.ExclusiveFullscreen : DisplayServer.WindowMode.Windowed);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && moveCamera)
            mousePosition = mouseMotion.Relative;

        if (Input.IsActionJustPressed("toggle_fullscreen"))
            HUD._ToggleFullscreen();
    }

    public override void _Process(double delta)
    {
        mousePosition *= 0.001f;
        var yaw = -mousePosition.X;
        var pitch = -mousePosition.Y;
        mousePosition = Vector2.Zero;

        camera.RotateY(Mathf.DegToRad(-yaw));
        camera.RotateObjectLocal(new Vector3(1, 0, 0), Mathf.DegToRad(-pitch));
    }

    private void StartGame()
    {
        GetTree().Paused = true;
        moveCamera = false;

        AddChild(loadingScene.Instantiate<Control>());
        var map = GD.Load<PackedScene>("res://scenes/map.tscn");

        var timer = new Timer() { OneShot = true };
        timer.Timeout += () =>
        {
            GetTree().Paused = false;
            GetTree().ChangeSceneToPacked(map);
        };
        AddChild(timer);
        timer.Start(0.7);
    }
}
