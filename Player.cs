using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public int Speed {get; set;} = 200;
	
	public static int coins {get; set; } = 0;

	//stores the vertical velocity modifier for underwater gravity
	private static float underwaterGravityVelocity {get; set; } = 0.5F;

	//not sure if this should be private
	public int hp;

	//if true, player can't get hit
	private Boolean invulnerable;

	//player flashing animation (when hit)
	private Boolean flash;
	
	//determines player sitting position
	private Boolean facingRight;

	//stores velocity modifiers such as wind/tube coral pull
	private Vector2 velocityModifier;

	//stores possible gravity options
	enum Gravity
	{
		Underwater, Air, None
	}

	//stores the current gravity type
	Gravity gravity;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//ScreenSize = GetViewportRect().Size;
		velocityModifier = Vector2.Zero;
		hp = 2;
		invulnerable = false;
		flash = false;
		gravity = Gravity.Underwater; //todo - change to update based on the player's room
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		var velocity = Vector2.Zero; //(0, 0)
		if (Input.IsActionPressed("move_right")) {
			velocity.X += 1;
		}
		if (Input.IsActionPressed("move_left")) {
			velocity.X -= 1;
		}
		if (Input.IsActionPressed("move_down")) {
			velocity.Y += 1;
		}
		if (Input.IsActionPressed("move_up")) {
			velocity.Y -= 1;
		}

		//gravity and velocity modifier
		if (gravity == Gravity.Underwater)
		{
			velocity.Y += underwaterGravityVelocity;
		}
		velocity = velocity.Normalized() + velocityModifier;
	

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (velocity.Length() > 0) {
			velocity = velocity * Speed;
			
			animatedSprite2D.Play();
		}
		else {
			animatedSprite2D.Stop();
		}

		//setting the animation
		if (velocity.X < 0)
		{
			animatedSprite2D.Animation = "swim-left";
			facingRight = false;
		}
		else if (velocity.X > 0)
		{
			animatedSprite2D.Animation = "swim-right";
			facingRight = true;
		}
		else
		{
			if (facingRight) {
				animatedSprite2D.Animation = "sit-helmet";
			}
			else {
				animatedSprite2D.Animation = "left_sit";
			}
			
		}

		//flashing
		var hurtTimer = GetNode<Godot.Timer>("HurtTimer");
		if (flash)
		{
			var modulate = animatedSprite2D.Modulate;
			if (hurtTimer.TimeLeft % 0.2 < 0.1)
			{
				animatedSprite2D.Modulate = new Color(modulate.R, modulate.G, modulate.B, (float) 0.5);
			}
			else
			{
				animatedSprite2D.Modulate = new Color(modulate.R, modulate.G, modulate.B, (float) 0);
			}
		}

		//Position += velocity * (float)delta;
		MoveAndCollide(velocity * (float)delta); //character2d movement
	}

	//player enteres hitbox
	private void OnHurtboxAreaEntered(Node2D area)
	{
		if (!invulnerable)
		{
			GetHit();
		}
		//
		else
		{
			GD.Print("oops");
		}
	}

	//get hit
	private void GetHit()
	{
		hp --;
		GD.Print("HP: " + hp); //
		if (hp <= 0)
		{
			//todo - add death code
			GD.Print("You died! HP resetting to 2."); //
			hp = 2;
		}

		//i-frames
		var hurtTimer = GetNode<Godot.Timer>("HurtTimer");
		invulnerable = true;
		flash = true;
		hurtTimer.Start();
	}

	//when invulnerablility ends
	private void OnHurtTimerTimeout()
	{
		invulnerable = false;
		flash = false;

		var insideHurtbox =  GetNode<Area2D>("Hurtbox").GetOverlappingBodies();

		//if player is in hitbox when invulnerability ends
		if (insideHurtbox.Count > 0)
		{
			GetHit();
		}
	}

	private void OnTubeCoralPull(Vector2 tubeVelocity)
	{
		velocityModifier = tubeVelocity;
	}

	//stop pulling the character when it leaves the AOE
	private void OnTubeCoralUnpull()
	{
		velocityModifier = Vector2.Zero;
	}
}
