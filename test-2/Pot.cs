using Godot;
using System;

public partial class Pot : RigidBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//this.Rotation = Mathf.Clamp(this.Rotation, Mathf.DegToRad(-45), Mathf.DegToRad(45));
	}
}
