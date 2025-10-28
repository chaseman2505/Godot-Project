using Godot;
using System;
using System.Numerics;

public partial class CharacterController : RigidBody2D
{
	//Notes: the root node will always get saved as a scene when you eventually hit save
	//scene or save all scenes. This scene which is based on the root node cannot be instanced.


	CollisionShape2D collisionShape;
	Sprite2D sprite2D;

	//The default scale and position for both the CollisionShape and Sprite2D
	Godot.Vector2 defaultScale = new Godot.Vector2(0.2f, 1.2f);
	Godot.Vector2 defaultPosition = new Godot.Vector2(0, 76.8f);

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collisionShape = GetChild<CollisionShape2D>(0);
		sprite2D = GetChild<Sprite2D>(1);

		//Engine.TimeScale = 2f;
		ContinuousCd = RigidBody2D.CcdMode.CastShape;

		//Disables any linear or angular damping
		this.LinearDamp = 0;
		this.AngularDamp = 0;

		//Sets the center of mass to be exactly where the RigidBody2D instead of being the center of mass of the collisionShape2D
		this.CenterOfMassMode = RigidBody2D.CenterOfMassModeEnum.Custom;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsPhysicalKeyPressed(Key.W))
		{
			//Generates a scalar value to apply to the default scale and default position of the collision shape and sprite
			float scalar = (GetGlobalMousePosition() - this.GlobalPosition).Length() / (128 * defaultScale.Y);

			//The max and min that the scalar value can be
			float maxScalarValue = 2f;
			float minScalarValue = 0.8f;


			//Weird bugs happen when the min value is too low

			//Makes sure the scalar doesn't exceed its max or min value
			if (scalar >= maxScalarValue)
			{
				scalar = maxScalarValue;
			}
			else if (scalar <= minScalarValue)
			{
				scalar = minScalarValue;
			}

			//Adjusts the scale and position of the collision shape and sprite using the scalar value
			collisionShape.Scale = new Godot.Vector2(collisionShape.Scale.X, defaultScale.Y * scalar);
			collisionShape.Position = new Godot.Vector2(collisionShape.Position.X, defaultPosition.Y * scalar);
			sprite2D.Scale = new Godot.Vector2(sprite2D.Scale.X, defaultScale.Y * scalar);
			sprite2D.Position = new Godot.Vector2(sprite2D.Position.X, defaultPosition.Y * scalar);
			
			
		}

		//Vector2 direction = GetGlobalMousePosition() - this.GlobalPosition;
		//this.Rotation = direction.Angle() + Mathf.Pi/2;
		//GD.Print(this.Rotation);
		
	}

	//
	public override void _PhysicsProcess(double delta)
	{

		this.LinearVelocity = new Godot.Vector2(0, 0);
		this.AngularVelocity = 0;

		//ApplyCentralImpulse(10 * (GetGlobalMousePosition() - (this.GlobalPosition + new Godot.Vector2(0, 150))) * this.Mass);
		ApplyTorqueImpulse((Mathf.Wrap((GetGlobalMousePosition() - this.GlobalPosition).Angle() - this.Rotation, -Mathf.Pi, Mathf.Pi) - Mathf.Pi / 2) * 2000000);
		//ApplyTorqueImpulse(10f);
		
		
		//GD.Print(this.LinearVelocity.Length());
		//GD.Print(this.AngularVelocity);


		//Maybe I should apply a force or impulse to not the center of mass instead of changing the center of mass
		//What is the difference between Impulse and Force?
	}


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			Godot.Vector2 relative = mouseMotion.Relative; // Movement since last event
			Godot.Vector2 position = mouseMotion.Position; // Current mouse position

			//GD.Print($"Mouse moved: {relative}, Current position: {position}");
		}

		//When a mouse button is released
		if (@event is InputEventMouseButton mouseEvent && !mouseEvent.Pressed)
		{
			//ApplyCentralImpulse(10 * (GetGlobalMousePosition() - this.GlobalPosition) * this.Mass);
			this.Freeze = true;
			this.GlobalPosition = GetGlobalMousePosition();
			this.Freeze = false;
		}
	}
}
