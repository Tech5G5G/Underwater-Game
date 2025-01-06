using Godot;
using System;

public partial class SettingsWindow : Window
{
	public override void _Ready()
	{
		this.CloseRequested += () => Hide();
	}
}
