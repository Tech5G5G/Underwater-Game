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

        (playButtonCollision = (playButton = this.GetNode<MeshInstance3D>("PlayButton")).GetNode<Area3D>("Area3D")).InputEvent += (camera, @event, position, normal, shapeIdx) =>
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
        playButtonMaterial = (playButton.Mesh as BoxMesh).Material as StandardMaterial3D;
        playButtonCollision.MouseEntered += () => playButtonMaterial.AlbedoColor = Colors.Gray;
        playButtonCollision.MouseExited += () => playButtonMaterial.AlbedoColor = Colors.White;

        (settingsButtonCollision = (settingsButton = this.GetNode<MeshInstance3D>("SettingsButton")).GetNode<Area3D>("Area3D")).InputEvent += (camera, @event, position, normal, shapeIdx) =>
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
        settingsButtonMaterial = (settingsButton.Mesh as BoxMesh).Material as StandardMaterial3D;
        settingsButtonCollision.MouseEntered += () => settingsButtonMaterial.AlbedoColor = Colors.Gray;
        settingsButtonCollision.MouseExited += () => settingsButtonMaterial.AlbedoColor = Colors.White;

        (exitButtonCollision = (exitButton = this.GetNode<MeshInstance3D>("ExitButton")).GetNode<Area3D>("Area3D")).InputEvent += (camera, @event, position, normal, shapeIdx) =>
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
        exitButtonMaterial = (exitButton.Mesh as BoxMesh).Material as StandardMaterial3D;
        exitButtonCollision.MouseEntered += () => exitButtonMaterial.AlbedoColor = Colors.Gray;
        exitButtonCollision.MouseExited += () => exitButtonMaterial.AlbedoColor = Colors.White;

        GameSettings.LoadSettings();
        DisplayServer.WindowSetMode(GameSettings.WindowModeSetting == 0 ? DisplayServer.WindowMode.ExclusiveFullscreen : DisplayServer.WindowMode.Windowed);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && moveCamera)
            mousePosition = GameSettings.InvertMouseSetting ? -mouseMotion.Relative : mouseMotion.Relative;
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
        this.ProcessMode = ProcessModeEnum.Always;
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
