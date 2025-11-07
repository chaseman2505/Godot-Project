using Godot;
using System;

public partial class CursorController : Sprite2D
{
	private double timeSinceLastMouseMove = 0;
	private const double mouseStopDelay = 1;

	private RigidBody2D hammerRigidBody;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hammerRigidBody = GetNode<RigidBody2D>("../Pot/Hammer");
		
		this.GlobalPosition = hammerRigidBody.GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timeSinceLastMouseMove += delta;
		//Checks if the mouse stopped moving for a certain amount of time
		if (timeSinceLastMouseMove > mouseStopDelay)
		{
			//this.GlobalPosition = hammerRigidBody.GlobalPosition + (new Godot.Vector2(0, 150) * );
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			Godot.Vector2 relative = mouseMotion.Relative; // Movement since last event
			Godot.Vector2 position = mouseMotion.Position; // Current mouse position

			//GD.Print($"Mouse moved: {relative}, Current position: {position}");


			this.Translate(relative);
			GD.Print("moving");
			timeSinceLastMouseMove = 0;
		}

		//When a mouse button is released
		if (@event is InputEventMouseButton mouseEvent && !mouseEvent.Pressed)
		{
			
		}
	}
}
