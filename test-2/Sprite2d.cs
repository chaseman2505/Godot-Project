using Godot;
using System;

public partial class Sprite2d : Sprite2D
{
	//Notes: the root node will always get saved as a scene when you eventually hit save
	//scene or save all scenes. This scene which is based on the root node cannot be instanced.
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Rotate(3);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsPhysicalKeyPressed(Key.W))
		{
			this.Rotate(0.001f);
		}
	}
}
