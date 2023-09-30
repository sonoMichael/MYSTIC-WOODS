using Godot;
using static Godot.GD;
using System;
using System.ComponentModel;

public partial class player : CharacterBody2D
{
	bool isPlayerAlive = true;
	bool canMove = true;

	float health = 20;
	float attackDamage = 3;
	public const float Speed = 150.0f;
	public const float swordCooldown = 10;

	Vector2 movementInput = Vector2.Zero;
	public string movementDirection = "front";

	AnimatedSprite2D animatedSprite;
	Area2D swordAttackHitbox;
	Timer swordAttackCooldown;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		swordAttackHitbox = GetNode<Area2D>("SwordAttackHitbox");
		swordAttackCooldown = GetNode<Timer>("SwordAttackCooldown");
    }
    public override void _PhysicsProcess(double delta)
	{
		movementInput = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Velocity = movementInput * Speed;

		if (Input.IsActionJustPressed("mouse_left_click") && canMove) { SwordAttack(); canMove = false; }
		if (canMove) {
			MovementAnimation(movementInput);
			MoveAndSlide();
		}
	}
	private void MovementAnimation(Vector2 input)
	{
		if (input != Vector2.Zero)
		{
			if (input.Y > 0)
			{
				swordAttackHitbox.Position = new Vector2(0, 0);
				swordAttackHitbox.RotationDegrees = 90;
				animatedSprite.Play("walk_front");
				movementDirection = "down";
			}
			if (input.Y < 0)
			{
				swordAttackHitbox.Position = new Vector2(0, -20);
				swordAttackHitbox.RotationDegrees = 90;
				animatedSprite.Play("walk_back");
				movementDirection = "up";
			}
			if (input.X > 0)
			{
				swordAttackHitbox.Position = new Vector2(10, -9);
				swordAttackHitbox.Rotation = 0;
				animatedSprite.Play("walk_side");
				animatedSprite.FlipH = false;
				movementDirection = "right";
			}
			if (input.X < 0)
			{
				swordAttackHitbox.Position = new Vector2(-10, -9);
				swordAttackHitbox.Rotation = 0;
				animatedSprite.Play("walk_side");
				animatedSprite.FlipH = true;
				movementDirection = "left";
			}
		}
		else
		{
			switch (movementDirection)
			{
				case "up":
					animatedSprite.Play("idle_back"); break;
				case "down":
					animatedSprite.Play("idle_front"); break;
				case "right":
					animatedSprite.Play("idle_side");
					animatedSprite.FlipH = false; break;
				case "left":
					animatedSprite.Play("idle_side");
					animatedSprite.FlipH = true; break;
			}
		}
	}
	private void _on_sword_attack_hitbox_body_entered(Node2D body)
	{
		body.Call("TakeDamage", attackDamage);
	}
	public void SwordAttack()
	{
		swordAttackCooldown.Start();
        switch (movementDirection)
        {
            case "up":
                animatedSprite.Play("sword_attack_back"); break;
            case "down":
                animatedSprite.Play("sword_attack_front"); break;
            case "right":
                animatedSprite.Play("sword_attack_side");
                animatedSprite.FlipH = false; break;
            case "left":
                animatedSprite.Play("sword_attack_side");
                animatedSprite.FlipH = true; break;
        }
		swordAttackHitbox.Monitoring = true;
	}
	public void TakeDamage(float damage) {
		health -= damage;
		if (health <= 0) QueueFree();
	}
	public void _on_sword_attack_cooldown_timeout() {
        if (swordAttackHitbox.Monitoring == true)
        {
            swordAttackHitbox.Monitoring = false;
        }
        canMove = true;
		swordAttackCooldown.Stop();
	}
}
