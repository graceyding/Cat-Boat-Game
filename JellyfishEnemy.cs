using Godot;
using System;

public partial class JellyfishEnemy : Node2D
{
	[Export]
	public int Speed{get; set;} = 60;

	private static int[] Velocities = {1, 0, -1, 0};

	private int idx;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		idx = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero;
		velocity.Y = Velocities[idx];
		velocity *= Speed;

		Position += velocity * (float)delta;

		//animation (add later)
	}

	private void OnTimerTimeout()
	{
		idx ++;
		if (idx > 3)
		{
			idx = 0;
		}
	}
}
