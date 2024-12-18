using Godot;
using System;

public partial class Fish : CharacterBody3D
{
	private Node3D jet;

	public override void _Ready()
	{
		jet = GetParent().GetNode<Node3D>("Jet");
	}

	public override void _Process(double delta)
	{
		LookAt(jet.GlobalPosition);

		var offset = GlobalTransform.Basis * new Vector3(0, 0, -0.5f) * (float)delta;
		this.Velocity += offset;
		this.MoveAndSlide();
	}
}
