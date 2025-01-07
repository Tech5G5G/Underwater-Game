using Godot;
using System;
using System.Xml.Schema;

public partial class SettingsWindow : Window
{
	public TabBar Tabs;

	public Control RadioButtons;
	public ItemList Bindings;
	public Control Mouse;

	public HSlider Sensitivity;
	public TextEdit MouseSensitivity;
	public Label DifficultyExplainer;

	public override void _Ready()
	{
		(Tabs = GetNode<TabBar>("Tabs")).TabSelected += (index) =>
		{
			switch (index)
			{
				default:
				case 0:
					RadioButtons.Visible = true;
					Bindings.Visible = false;
					break;
				case 1:
					Bindings.Visible = true;
					RadioButtons.Visible = false;
					break;
				case 2:
					RadioButtons.Visible = Bindings.Visible = false;
					break;
				case 3:
					RadioButtons.Visible = Bindings.Visible = false;
					break;
			}
		};

		(RadioButtons = GetNode<Control>("View/RadioButtons")).GetNode<Button>("Easy").ButtonGroup.Pressed += (selected) =>
		{
			switch (selected.Name)
			{
				case "Easy":
					DifficultyExplainer.Text = "Enemy damage :  1:200\nFlashlight drainage: 1% every 0.5 sec\nPower recovery: 1% every 0.1 sec";
					break;
				case "Normal":
					DifficultyExplainer.Text = "Enemy damage :  1:100\nFlashlight drainage: 1% every 0.3 sec\nPower recovery: 1% every 0.2 sec";
					break;
				case "Hard":
					DifficultyExplainer.Text = "Enemy damage :  1:75\nFlashlight drainage: 1% every 0.2 sec\nPower recovery: 1% every 0.3 sec";
					break;
				case "Nightmare":
					DifficultyExplainer.Text = "Enemy damage :  1:50\nFlashlight drainage: 1% every 0.1 sec\nPower recovery: 1% every 0.5 sec";
					break;
			}
		};

		(Sensitivity = (Mouse = GetNode<Control>("View/Mouse")).GetNode<HSlider>("Sensitivity")).ValueChanged += (value) =>
		{
			if (GuiGetFocusOwner() is not TextEdit)
			{
				var sensitivity = 0.25f * (value / 100);
				MouseSensitivity.Text = value.ToString();
			}
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
		MouseSensitivity.FocusExited += () => MouseSensitivity.Text = Sensitivity.Value.ToString();

		Bindings = GetNode<ItemList>("View/Bindings");
		DifficultyExplainer = RadioButtons.GetNode<Label>("DifficultyExplainer");
		this.CloseRequested += () => Hide();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey input && input.Keycode == Key.Enter && GuiGetFocusOwner() is TextEdit)
			GetViewport().SetInputAsHandled();
	}
}
