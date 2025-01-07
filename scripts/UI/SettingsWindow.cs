using Godot;
using System;
using System.Text.Json;

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Nightmare
}

public class GameSettings
{
    public bool InvertMouse { get; set; }
    public float MouseSensitivity { get; set; }
    public int WindowMode { get; set; }
    public int Difficulty { get; set; }

    public static bool InvertMouseSetting { get; set; }
    public static float MouseSensitivitySetting { get; set; }
    public static int WindowModeSetting { get; set; }
    public static int DifficultySetting { get; set; }

    public static GameSettings FromStaticSettings() => new()
    {
        InvertMouse = InvertMouseSetting,
        MouseSensitivity = MouseSensitivitySetting,
        WindowMode = WindowModeSetting,
        Difficulty = DifficultySetting
    };

    public static void SaveSettings(GameSettings settings)
    {
		var file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Write);
		file.StorePascalString(JsonSerializer.Serialize(settings));
		file.Close();
    }

    public static void LoadSettings()
    {
		var file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.ReadWrite);
		GameSettings gameSettings;
		if (!FileAccess.FileExists("user://settings.json"))
		{
			gameSettings = new GameSettings() { Difficulty = (int)global::Difficulty.Normal, InvertMouse = false, MouseSensitivity = 0.25f, WindowMode = 0 };
			file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.WriteRead);
			file.StorePascalString(JsonSerializer.Serialize(gameSettings));
			file.Close();
		}
		else
		{
			gameSettings = JsonSerializer.Deserialize<GameSettings>(file.GetPascalString());
			file.Close();
		}
		GameSettings.WindowModeSetting = gameSettings.WindowMode;
		GameSettings.InvertMouseSetting = gameSettings.InvertMouse;
		GameSettings.DifficultySetting = gameSettings.Difficulty;
		GameSettings.MouseSensitivitySetting = gameSettings.MouseSensitivity;
    }
}

public partial class SettingsWindow : Window
{
    public TabBar Tabs;

    public Control RadioButtons;
    public ItemList Bindings;
    public Control Mouse;
    public Control Video;

    public HSlider Sensitivity;
    public TextEdit MouseSensitivity;
    public CheckBox InvertMouse;
    public Label DifficultyExplainer;
    public OptionButton WindowMode;

    public override void _Ready()
    {
        (Tabs = GetNode<TabBar>("Tabs")).TabSelected += (index) =>
        {
            switch (index)
            {
                default:
                case 0:
                    RadioButtons.Visible = true;
                    Bindings.Visible = Mouse.Visible = Video.Visible = false;
                    break;
                case 1:
                    Bindings.Visible = true;
                    RadioButtons.Visible = Mouse.Visible = Video.Visible = false;
                    break;
                case 2:
                    Mouse.Visible = true;
                    RadioButtons.Visible = Bindings.Visible = Video.Visible = false;
                    break;
                case 3:
                    Video.Visible = true;
                    RadioButtons.Visible = Bindings.Visible = Mouse.Visible = false;
                    break;
            }
        };

        (RadioButtons = GetNode<Control>("View/RadioButtons")).GetNode<Button>("Easy").ButtonGroup.Pressed += (selected) =>
        {
            DifficultyExplainer.Text = (string)selected.Name switch
            {
                "Easy" => "Enemy damage: 1:200\nFlashlight drainage: 1% every 0.5 sec\nPower recovery: 1% every 0.1 sec",
                "Hard" => "Enemy damage: 1:75\nFlashlight drainage: 1% every 0.2 sec\nPower recovery: 1% every 0.3 sec",
                "Nightmare" => "Enemy damage: 1:50\nFlashlight drainage: 1% every 0.1 sec\nPower recovery: 1% every 0.5 sec",
                _ => "Enemy damage: 1:100\nFlashlight drainage: 1% every 0.3 sec\nPower recovery: 1% every 0.2 sec"
            };

            var file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Write);
            var settings = GameSettings.FromStaticSettings();
            settings.Difficulty = (string)selected.Name switch
            {
                "Easy" => (int)Difficulty.Easy,
                "Hard" => (int)Difficulty.Hard,
                "Nightmare" => (int)Difficulty.Nightmare,
                _ => (int)Difficulty.Normal
            }; ;
            file.StorePascalString(JsonSerializer.Serialize(settings));
            file.Close();

            GameSettings.DifficultySetting = settings.Difficulty;
        };

        (Sensitivity = (Mouse = GetNode<Control>("View/Mouse")).GetNode<HSlider>("Sensitivity")).ValueChanged += (value) =>
        {
            float sensitivity = 0.25f * ((float)value / 100f);

            if (GuiGetFocusOwner() is not TextEdit)
                MouseSensitivity.Text = value.ToString();

            var file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Write);
            var settings = GameSettings.FromStaticSettings();
            settings.MouseSensitivity = sensitivity;
            file.StorePascalString(JsonSerializer.Serialize(settings));
            file.Close();

            GameSettings.MouseSensitivitySetting = sensitivity;
        };
        (MouseSensitivity = Mouse.GetNode<TextEdit>("MouseSensitivity")).TextChanged += () =>
        {
            if (!int.TryParse(MouseSensitivity.Text, out int value))
            {
                MouseSensitivity.Text = string.Empty;
                return;
            }

            value = Mathf.Clamp(value, 0, 200);
            value = (int)Mathf.Round(value / 5) * 5;
            Sensitivity.Value = value;
        };
        (InvertMouse = Mouse.GetNode<CheckBox>("InvertMouse")).ButtonUp += () =>
        {
            var file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Write);
            var settings = GameSettings.FromStaticSettings();
            settings.InvertMouse = InvertMouse.ButtonPressed;
            file.StorePascalString(JsonSerializer.Serialize(settings));
            file.Close();

            GameSettings.InvertMouseSetting = InvertMouse.ButtonPressed;
        };
        MouseSensitivity.FocusExited += () => MouseSensitivity.Text = Sensitivity.Value.ToString();

        (WindowMode = (Video = GetNode<Control>("View/Video")).GetNode<OptionButton>("WindowMode")).ItemSelected += (index) =>
        {
            var file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Write);
            var settings = GameSettings.FromStaticSettings();
            settings.WindowMode = (int)index;
            file.StorePascalString(JsonSerializer.Serialize(settings));
            file.Close();

            DisplayServer.WindowSetMode(index == 0 ? DisplayServer.WindowMode.ExclusiveFullscreen : DisplayServer.WindowMode.Windowed);
            GameSettings.WindowModeSetting = settings.WindowMode;
        };

        Bindings = GetNode<ItemList>("View/Bindings");
        DifficultyExplainer = RadioButtons.GetNode<Label>("DifficultyExplainer");

        InvertMouse.SetPressedNoSignal(GameSettings.InvertMouseSetting);
        Sensitivity.SetValueNoSignal(GameSettings.MouseSensitivitySetting / 0.25f * 100);
        MouseSensitivity.Text = (GameSettings.MouseSensitivitySetting / 0.25f * 100).ToString();
        WindowMode.Selected = GameSettings.WindowModeSetting;

        switch ((Difficulty)GameSettings.DifficultySetting)
        {
            case Difficulty.Easy:
                RadioButtons.GetNode<Button>("Easy").ButtonPressed = true;
                DifficultyExplainer.Text = "Enemy damage: 1:200\nFlashlight drainage: 1% every 0.5 sec\nPower recovery: 1% every 0.1 sec";
                break;
            case Difficulty.Normal:
                RadioButtons.GetNode<Button>("Normal").ButtonPressed = true;
                DifficultyExplainer.Text = "Enemy damage:  1:100\nFlashlight drainage: 1% every 0.3 sec\nPower recovery: 1% every 0.2 sec";
                break;
            case Difficulty.Hard:
                RadioButtons.GetNode<Button>("Hard").ButtonPressed = true;
                DifficultyExplainer.Text = "Enemy damage: 1:75\nFlashlight drainage: 1% every 0.2 sec\nPower recovery: 1% every 0.3 sec";
                break;
            case Difficulty.Nightmare:
                RadioButtons.GetNode<Button>("Nightmare").ButtonPressed = true;
                DifficultyExplainer.Text = "Enemy damage: 1:50\nFlashlight drainage: 1% every 0.1 sec\nPower recovery: 1% every 0.5 sec";
                break;
        }

        this.CloseRequested += () => Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey input && input.Keycode == Key.Enter && GuiGetFocusOwner() is TextEdit)
            GetViewport().SetInputAsHandled();
    }
}
