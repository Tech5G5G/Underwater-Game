using Godot;
using System;
using System.Text.Json;

public class GameSave
{
    public bool DefaultFound { get; set; }
    public bool GreenFound { get; set; }
    public bool EpikFound { get; set; }
    public bool BotFound { get; set; }

    public static bool DefaultFoundSave { get; set; }
    public static bool GreenFoundSave { get; set; }
    public static bool EpikFoundSave { get; set; }
    public static bool BotFoundSave { get; set; }

    public static GameSave FromStaticSave() => new()
    {
        DefaultFound = DefaultFoundSave,
        GreenFound = GreenFoundSave,
        EpikFound = EpikFoundSave,
        BotFound = BotFoundSave
    };

    public static void Save_Save(GameSave save)
    {
        var file = FileAccess.Open("user://save.json", FileAccess.ModeFlags.Write);
        file.StorePascalString(JsonSerializer.Serialize(save));
        file.Close();
    }

    public static void LoadSave()
    {
        var file = FileAccess.Open("user://save.json", FileAccess.ModeFlags.ReadWrite);
        GameSave gameSave;
        if (!FileAccess.FileExists("user://save.json"))
        {
            gameSave = new();
            file = FileAccess.Open("user://save.json", FileAccess.ModeFlags.WriteRead);
            file.StorePascalString(JsonSerializer.Serialize(gameSave));
            file.Close();
        }
        else
        {
            gameSave = JsonSerializer.Deserialize<GameSave>(file.GetPascalString());
            file.Close();
        }
        DefaultFoundSave = gameSave.DefaultFound;
        GreenFoundSave = gameSave.GreenFound;
        EpikFoundSave = gameSave.EpikFound;
        BotFoundSave = gameSave.BotFound;
    }
}

public partial class List : Window
{
    public ItemList Fishes;
    public Button Reset;

    public override void _Ready()
    {
        Fishes = GetNode<ItemList>("Fish");
        if (GameSave.DefaultFoundSave)
            Fishes.SetItemText(0, "  Default fish  ");
        if (GameSave.GreenFoundSave)
            Fishes.SetItemText(2, "  Green fish  ");
        if (GameSave.EpikFoundSave)
            Fishes.SetItemText(4, "  Epik fish  ");
        if (GameSave.BotFoundSave)
            Fishes.SetItemText(6, "  NextBot fish  ");

        (Reset = GetNode<Button>("Reset")).Pressed += () =>
        {
            GameSave.Save_Save(new());
            GameSave.DefaultFoundSave = GameSave.GreenFoundSave = GameSave.EpikFoundSave = GameSave.BotFoundSave = false;
            for (int i = 6; i >= 0; i -= 2)
                Fishes.SetItemText(i, "  Unknown  ");
        };

        this.CloseRequested += () => this.Hide();
    }
}
