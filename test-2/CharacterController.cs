using Godot;
using System;
using System.Numerics;

public partial class CharacterController : RigidBody2D
{
	//Notes: the root node will always get saved as a scene when you eventually hit save
	//scene or save all scenes. This scene which is based on the root node cannot be instanced.


	//Pogostick?
	private CollisionShape2D collisionShape;
	private Sprite2D sprite2D;
	private Sprite2D mouse;
	private float sensitivity = 0.3f;

	//The default scale and position for both the CollisionShape and Sprite2D
	private Godot.Vector2 defaultScale = new Godot.Vector2(0.2f, 1.2f);
	private Godot.Vector2 defaultPosition = new Godot.Vector2(0, 76.8f);

	private double timeSinceLastMouseMove = 0;
	private Godot.Vector2 mouseOffset = new Godot.Vector2(0, 0);

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collisionShape = GetChild<CollisionShape2D>(0);
		sprite2D = GetChild<Sprite2D>(1);
		mouse = GetNode<Sprite2D>("../../Cursor");

		//Engine.TimeScale = 2f;
		ContinuousCd = RigidBody2D.CcdMode.CastShape;

		//Disables any linear or angular damping
		this.LinearDamp = 0;
		this.AngularDamp = 0;

		//Sets the center of mass to be exactly where the RigidBody2D instead of being the center of mass of the collisionShape2D
		this.CenterOfMassMode = RigidBody2D.CenterOfMassModeEnum.Custom;

		//Hides the cursor and locks it to the center of the screen
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	//
	public override void _PhysicsProcess(double delta)
	{
		//this.LinearVelocity = new Godot.Vector2(0, 0);
		this.AngularVelocity = 0;

		mouse.GlobalPosition = this.GlobalPosition + new Godot.Vector2(0, 128 * collisionShape.Scale.Y).Rotated(this.GlobalRotation) + (mouseOffset * sensitivity);
		Godot.Vector2 desiredVector = (this.GlobalPosition + new Godot.Vector2(0, 128 * collisionShape.Scale.Y).Rotated(this.GlobalRotation) + (mouseOffset * sensitivity)) - this.GlobalPosition;

		//Caculates the moment of interia depending on the Y scale (this assumes mass = 1, default height and width is 128, and that X scale is always 0.2)
		float momentOfIntertia = (655.36f + ((128 * collisionShape.Scale.Y) * (128 * collisionShape.Scale.Y))) / 12 + (64 * collisionShape.Scale.Y);

		//(Desired angle - current rotation - 90 degrees) * Moment of Inertia * a big number
		//Multipying by moment of interia should make angular velocity the same regardless of Y scale
		//ApplyTorqueImpulse(Mathf.Wrap((mouse.GlobalPosition - this.GlobalPosition).Angle() - this.GlobalRotation - Mathf.Pi / 2, -Mathf.Pi, Mathf.Pi) * momentOfIntertia * 500);
		ApplyTorqueImpulse(Mathf.Wrap((desiredVector).Angle() - this.GlobalRotation - Mathf.Pi / 2, -Mathf.Pi, Mathf.Pi) * momentOfIntertia * 700);

		//Generates a scalar value to apply to the default scale and default position of the collision shape and sprite
		//float scalar = (mouse.GlobalPosition - this.GlobalPosition).Length() / (128 * defaultScale.Y);
		float scalar = (desiredVector).Length() / (128 * defaultScale.Y);

		//The max and min that the scalar value can be
		float maxScalarValue = 1f;
		float minScalarValue = 0.001f;

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

		mouseOffset = new Godot.Vector2(0, 0);

		//Caps linear velocity
		int maxVelocity = 500;
		if(this.LinearVelocity.Length() > maxVelocity)
		{
			this.LinearVelocity = this.LinearVelocity.Normalized() * maxVelocity;
		}

		/*mouse.GlobalPosition = this.GlobalPosition + new Godot.Vector2(0, 128 * collisionShape.Scale.Y).Rotated(this.GlobalRotation);
		mouse.Translate(mouseOffset);
		mouseOffset = new Godot.Vector2(0, 0);*/

		/*timeSinceLastMouseMove += delta;
		//Checks if the mouse stopped moving for a certain amount of time
		if (timeSinceLastMouseMove > mouseStopDelay)
		{
			mouse.GlobalPosition = this.GlobalPosition + new Godot.Vector2(0, 128 * collisionShape.Scale.Y).Rotated(this.GlobalRotation);
			//GD.Print("still");
		}
		else
		{
			mouse.Translate(mouseOffset);
			mouseOffset = new Godot.Vector2(0, 0);
			//GD.Print("moving");
		}*/

		//GD.Print(this.LinearVelocity.Length());
		//GD.Print(this.AngularVelocity);
		//GD.Print("Rotation: " + Mathf.RadToDeg(this.GlobalRotation));
		//GD.Print("Angle: " + Mathf.RadToDeg((GetGlobalMousePosition() - this.GlobalPosition).Angle()));
		//GD.Print("Torque: " + Mathf.RadToDeg(Mathf.Wrap((GetGlobalMousePosition() - this.GlobalPosition).Angle() - this.GlobalRotation - Mathf.Pi / 2, -Mathf.Pi, Mathf.Pi)));

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

			mouseOffset += relative;
		}

		//When a mouse button is released
		if (@event is InputEventMouseButton mouseEvent && !mouseEvent.Pressed)
		{

		}
		
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}
}
