using Godot;
using System;
using static Godot.TextServer;

public partial class player : CharacterBody3D
{
	const float shiftMultiplier = 2f;
	const float sensitivity = 0.25f;

    Vector2 mousePosition = Vector2.Zero;
	float totalPitch = 0f;

    const float velMultiplier = 4f;
    const float acceleration = 30f;
    const float deceleration = -10f;

    Vector3 direction = Vector3.Zero;
	Vector3 velocity = Vector3.Zero;

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
        UpdateMovement((float)delta);
        UpdateMouseLook();
	}

	public void UpdateMovement(float delta)
	{
		direction = (GlobalTransform.Basis * new Vector3(0, 0, -BoolToFloat(Input.IsActionPressed("up")))).Normalized();
		var offset = direction * acceleration * velMultiplier * delta + velocity * deceleration * velMultiplier * delta;
		if (direction == Vector3.Zero && offset.LengthSquared() > velocity.LengthSquared())
			velocity = Vector3.Zero;
		else
		{
			velocity.X = Mathf.Clamp(velocity.X + offset.X, -velMultiplier, velMultiplier);
			velocity.Y = Mathf.Clamp(velocity.Y + offset.Y, -velMultiplier, velMultiplier);
			velocity.Z = Mathf.Clamp(velocity.Z + offset.Z, -velMultiplier, velMultiplier);
		}

		Velocity = velocity * (Input.IsActionPressed("sprint") ? 7f * shiftMultiplier : 7f);
		MoveAndSlide();
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
