using Godot;
using System;
using ExtensionsMethods;

namespace ExtensionsMethods
{
	public static class Extensions
	{
		/// <summary>Convert a boolean value to a float</summary>
		/// <param name="input">bool to convert</param>
		/// <returns>1f for true, 0f for false</returns>
        public static float ToFloat(this bool input)
        {
            return input ? 1f : 0f;
        }
    }
}

public partial class player : CharacterBody3D
{
	const float shiftMultiplier = 2f;
	const float sensitivity = 0.25f;

    Vector2 mousePosition = Vector2.Zero;
	float totalPitch = 0f;
    float totalYaw = 0f;

    const float velMultiplier = 4f;
    const float acceleration = 30f;
    const float deceleration = -10f;

    Vector3 direction = Vector3.Zero;
	Vector3 velocity = Vector3.Zero;
    float previousSlowdown = 0f;

    public CharacterBody3D Jet;
	public Camera3D Cam;
    public Camera3D ThirdCam;

    public override void _Ready()
	{
		Jet = GetParent<CharacterBody3D>();
		Cam = GetNode<Camera3D>("Neck/Camera3D");
        ThirdCam = GetNode<Camera3D>("Neck/ThirdPersonCamera");

        Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
			mousePosition = (@event as InputEventMouseMotion).Relative;

        if (Input.IsActionJustPressed("toggle_camera"))
            ToggleCamera();

        if (Input.IsActionJustPressed("close"))
            GetTree().Quit();
    }

	public override void _Process(double delta)
	{
        UpdateMovement((float)delta);
		RotateJet();
        UpdateMouseLook();
    }

	public void ToggleCamera()
	{
		bool toggle = Cam.Current;
		Cam.Current = !toggle;
		ThirdCam.Current = toggle;
	}

	public void RotateJet()
	{
		var pitch = Input.IsActionPressed("up").ToFloat() - Input.IsActionPressed("down").ToFloat();
        pitch = Mathf.Clamp(pitch, -90 - totalPitch, 90 - totalPitch);
        Jet.RotateObjectLocal(new Vector3(1, 0, 0), Mathf.DegToRad(-pitch));

        var yaw = Input.IsActionPressed("turn_right").ToFloat() - Input.IsActionPressed("turn_left").ToFloat();
		Jet.RotateY(Mathf.DegToRad(-yaw));
    }

    public void UpdateMovement(float delta)
	{
		direction = GlobalTransform.Basis * new Vector3(0, 0, -Input.IsActionPressed("accelerate").ToFloat());
		var offset = direction * acceleration * velMultiplier * delta + velocity * deceleration * velMultiplier * delta;
		if (direction == Vector3.Zero)
		{
			if (previousSlowdown > 0)
			{
				previousSlowdown -= 0.03f / (Input.IsActionPressed("speed_up") ? shiftMultiplier : 1);

                direction = GlobalTransform.Basis * new Vector3(0, 0, -previousSlowdown);
                offset = direction * acceleration * velMultiplier * delta + velocity * deceleration * velMultiplier * delta;

                velocity.X += offset.X;
                velocity.Y += offset.Y;
                velocity.Z += offset.Z;
            }
			else
				velocity = Vector3.Zero;
        }
        else
		{
			velocity.X = Mathf.Clamp(velocity.X + offset.X, -velMultiplier, velMultiplier);
			velocity.Y = Mathf.Clamp(velocity.Y + offset.Y, -velMultiplier, velMultiplier);
			velocity.Z = Mathf.Clamp(velocity.Z + offset.Z, -velMultiplier, velMultiplier);

			previousSlowdown = 1f;
		}

		Jet.Velocity = velocity * (Input.IsActionPressed("speed_up") ? 7f * shiftMultiplier : 7f);
		Jet.MoveAndSlide();
    }

	public void UpdateMouseLook()
	{
		mousePosition *= sensitivity;
		var yaw = mousePosition.X;
		var pitch = mousePosition.Y;
		mousePosition = Vector2.Zero;

		pitch = Mathf.Clamp(pitch, -90 - totalPitch, 90 - totalPitch);
		totalPitch += pitch;

        yaw = Mathf.Clamp(yaw, -90 - totalYaw, 90 - totalYaw);
        totalYaw += yaw;

        Cam.RotateY(Mathf.DegToRad(-yaw));
		Cam.RotateObjectLocal(new Vector3(1, 0, 0), Mathf.DegToRad(-pitch));
    }
}
