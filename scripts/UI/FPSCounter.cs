using Godot;
using System;

public partial class FPSCounter : Label
{
	public override void _Ready()
	{
		SetTextTheme();
		GameSettings.FPSModeSettingChanged += SetTextTheme;

		this.ProcessMode = ProcessModeEnum.Always;
	}

	private void SetTextTheme()
	{
		if (IsInstanceValid(this))
		{
            Visible = GameSettings.FPSModeSetting != 0;
            this.AddThemeColorOverride("font_color", GameSettings.FPSModeSetting == 2 ? new Color(0x00FF00FF) : Colors.White);
        }
    }

	public override void _Process(double delta)
	{
        if (Visible)
            this.Text = $"FPS: {Engine.GetFramesPerSecond()}";
    }
}
