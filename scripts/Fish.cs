using Godot;
using System;

public partial class Fish : RigidBody3D
{
	private Node3D jet;

	public override void _Ready()
	{
		jet = GetParent().GetNode<Node3D>("Jet");
	}

	public override void _Process(double delta)
	{
		LinearVelocity = GlobalTransform.Basis * new Vector3(0, 0, -20);
	}
}
