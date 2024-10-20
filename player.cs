using Godot;
using System;
using static Godot.TextServer;

public partial class player : CharacterBody3D
{
	const float shiftMultiplier = 2f;
	const float sensitivity = 0.25f;

	Vector2 mousePosition = Vector2.Zero;
	float totalPitch = 0f;

	Vector3 direction = Vector3.Zero;
	Vector3 velocity = Vector3.Zero;
	float acceleration = 30f;
	float deceleration = -10f;
	float velMultiplier = 4f;

	public Node3D Jet;
	public Camera3D Cam;

	public override void _Ready()
	{
		Jet = GetNode<Node3D>("Neck/Camera3D/Jet");
		Cam = GetNode<Camera3D>("Neck/Camera3D");

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
			mousePosition = (@event as InputEventMouseMotion).Relative;
	}

	public override void _Process(double delta)
	{
		UpdateMouseLook();
		UpdateMovement((float)delta);
	}

	public void UpdateMovement(float delta)
	{
		direction = new Vector3(0, 0, -BoolToFloat(Input.IsActionPressed("up")));

		var offset = direction.Normalized() * acceleration * velMultiplier * delta + velocity.Normalized() * deceleration * velMultiplier * delta;

		if (direction == Vector3.Zero && offset.LengthSquared() > velocity.LengthSquared())
		{
			//If velocity less than one, set it to zero (ex. 0.5 => 0)

			var x = velocity.X == 0 ? 0 : velocity.X - 1;
            var y = velocity.Y == 0 ? 0 : velocity.Y - 1;
            var z = velocity.Z == 0 ? 0 : velocity.Z - 1;

            velocity = new Vector3(x, y, z);
        }
        else
		{
			velocity.X = Mathf.Clamp(velocity.X + offset.X, -velMultiplier, velMultiplier);
			velocity.Y = Mathf.Clamp(velocity.Y + offset.Y, -velMultiplier, velMultiplier);
			velocity.Z = Mathf.Clamp(velocity.Z + offset.Z, -velMultiplier, velMultiplier);
		}

		Translate(velocity * delta * (Input.IsActionPressed("sprint") ? 4f * shiftMultiplier : 4f));
	}

	public void UpdateMouseLook()
	{
		mousePosition *= sensitivity;
		var yaw = mousePosition.X;
		var pitch = mousePosition.Y;
		mousePosition = Vector2.Zero;

		pitch = Mathf.Clamp(pitch, -90 - totalPitch, 90 - totalPitch);
		totalPitch = pitch;

        RotateY(Mathf.DegToRad(-yaw));
        RotateObjectLocal(new Vector3(1, 0, 0), Mathf.DegToRad(-pitch));
	}

	private float BoolToFloat(bool input)
	{
		return input ? 1f : 0f;
	}
}
