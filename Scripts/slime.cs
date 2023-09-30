using Godot;
using static Godot.GD;
using System;

public partial class slime : CharacterBody2D
{
	public float health = 10;
	public float attackDamage = 2;
	public float Speed = 50.0f;
	public string movementDirection = "right";
	public bool canMove = true;
	public bool hasAttacked = false;
	public bool attackPreparationDone = false;
	public bool isInAttackRange = false;
	public bool isTriggered = false;
	public bool isTimerOn = false;

	AnimatedSprite2D animatedSprite;
	Area2D slimeHitbox;
	Node2D target;
	Area2D attackArea;
	Timer normalAttackCooldown;

    Vector2 attackInitialPosition;
    Vector2 movementInput;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		slimeHitbox = GetNode<Area2D>("SlimeHitbox");
		normalAttackCooldown = GetNode<Timer>("NormalAttackCooldown");
		attackArea = GetNode<Area2D>("AttackArea");
	}
	public override void _PhysicsProcess(double delta)
	{
		if (isInAttackRange) {
			SlimeAttack(GlobalPosition, target.Position);
        }
		if (canMove && !isTriggered) {
			animatedSprite.Play("idle_side");
            if (movementDirection == "right") {
                animatedSprite.FlipH = false;
            }
			if(movementDirection == "left") {
                animatedSprite.FlipH = true;
            }
		}
		if (canMove && isTriggered) {
			SlimeMovement(GlobalPosition, target.Position);
        }
	}
	private void SlimeMovement(Vector2 currentPosition, Vector2 enemyPosition)
	{
        movementInput = (enemyPosition - currentPosition).Normalized();
		if (movementInput != Vector2.Zero) {
			if(movementInput.X > 0) {
				movementDirection = "right";
                animatedSprite.Play("walk_side");
                animatedSprite.FlipH = false;
            }
            if (movementInput.X < 0) {
				movementDirection = "left";
                animatedSprite.Play("walk_side");
                animatedSprite.FlipH = true;
            }
            Velocity = movementInput * Speed;
			MoveAndSlide();
        }
	}

	private void _on_detection_area_body_entered(Node2D body) {
		target = body;
		isTriggered = true;
	}
	private void _on_detection_area_body_exited(Node2D body) {
		target = null;
		isTriggered = false;
    }
	private void _on_attack_area_body_entered(Node2D body)
	{
		isInAttackRange = true;
	}
	private void _on_attack_area_body_exited(Node2D body)
	{
		isInAttackRange = false;
		isTimerOn = false;
        hasAttacked = false;
        canMove = true;
        Speed = 50;
    }
	private void _on_slime_hitbox_body_entered(Node2D body)
	{
		if (target.Name == body.Name)
		{
			//body.Call("TakeDamage", attackDamage);
			hasAttacked = true;
			isTimerOn = false;
		}
	}
	private void SlimeAttack(Vector2 myPosition, Vector2 enemyPosition)
	{
		if (!isTimerOn && !hasAttacked)
		{
			Print("start timer");
			Speed = 100;
			canMove = false;
			movementInput = Vector2.Zero;
			normalAttackCooldown.Start();
			isTimerOn = true;
            attackInitialPosition = myPosition;
        }
		if (normalAttackCooldown.TimeLeft == 0 && !hasAttacked) { attackPreparationDone = true; }
		if (normalAttackCooldown.TimeLeft == 0 && attackPreparationDone && !hasAttacked)
		{
			Print("attacking");
			switch (movementDirection)
			{
				case "right":
					animatedSprite.Play("basic_attack_side");
					animatedSprite.FlipH = false; break;
				case "left":
					animatedSprite.Play("basic_attack_side");
					animatedSprite.FlipH = true; break;
			}
			movementInput = (enemyPosition - myPosition).Normalized();
		}
		Print(hasAttacked && !isTimerOn);
		if (hasAttacked && !isTimerOn)
		{
			Print("attacked");
			movementInput = (attackInitialPosition - myPosition).Normalized();
			if (myPosition == attackInitialPosition || !isInAttackRange)
			{
				Print("can move");
				hasAttacked = false;
				attackPreparationDone = false;
			}
		}
		Velocity = movementInput * Speed;
		MoveAndSlide();
	}
	public void TakeDamage(float damage) {
        health -= damage;
		Print(health);
		if (health <= 0) { QueueFree(); }
    }
}