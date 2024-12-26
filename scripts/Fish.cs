using Godot;
using System;

public partial class Fish : RigidBody3D
{
    private const float _speed = 0.1f;

    private Node3D playerWindscreen;

    public Vector3 forwardAxis = new(0, 0, -1);
    public int forwardAxisCooldown = 0;

	public override void _Ready()
	{
		jet = GetParent().GetNode<Node3D>("Jet");

		GravityScale = 0;
		Mass = 0.3f;
	}

    private void LookFollow(PhysicsDirectBodyState3D state, Transform3D currentTransform, Vector3 targetPosition)
    {
        Vector3 forwardLocalAxis = new(1, 0, 0);
        Vector3 forwardDir = (currentTransform.Basis * forwardLocalAxis).Normalized();
        Vector3 targetDir = (targetPosition - currentTransform.Origin).Normalized();
        float localSpeed = Mathf.Clamp(_speed, 0.0f, Mathf.Acos(forwardDir.Dot(targetDir)));

        AngularVelocity = forwardDir.Cross(targetDir) * localSpeed / state.Step;

        // if (forwardDir.Dot(targetDir) > 1e-4)
        // {
        // }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
		LookFollow(state, GlobalTransform, jet.GlobalTransform.Origin);
   	}

    public override void _Process(double delta)
	{
		LinearVelocity = GlobalTransform.Basis * new Vector3(0, 0, -20);
	}
}
