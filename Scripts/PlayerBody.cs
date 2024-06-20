using Godot;
using System;
using System.Diagnostics;

public partial class PlayerBody : CharacterBody3D
{
	[Export] Player_Manager playerManager;
	[Export] public float Speed = 5.0f, flySpeed = 15f;
	private float sprintMultiplier = 1f;
	private float currSpeed;
	public float JumpVelocity = 8.4f;
	private float jumpCooldown = 0.2f;
	public Vector2 inputDir;
	public Vector3 direction;

	private float movementAnimationLerper;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = 35;
	private float gravityMultiplyer = 0.1f;
	public bool lockMovement;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		if (playerManager.gameMode == 0)
		{
			currSpeed = Speed;

			// Add the gravity.
			if (!IsOnFloor())
				velocity.Y -= gravity * (float)delta;
			else
			{
				if (jumpCooldown > 0) jumpCooldown -= (float)delta;
				if (Input.IsActionPressed("jump") && jumpCooldown <= 0)
				{
					gravityMultiplyer = 0.1f;
					velocity.Y = JumpVelocity;
					jumpCooldown = 0.15f;
				}
			}

			gravityMultiplyer += (float)delta / 3f;
			if (gravityMultiplyer > 1f) gravityMultiplyer = 1f;
			if (velocity.Y < -40 * gravityMultiplyer) velocity.Y = -40f * gravityMultiplyer;
		}
		else if (playerManager.gameMode == 1)
		{
			currSpeed = flySpeed;
			if (Input.IsActionPressed("jump"))
				velocity.Y += JumpVelocity;


			else if (Input.IsActionPressed("sneak"))
				velocity.Y -= JumpVelocity;

			else velocity.Y = 0f;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		if (lockMovement == false)
		{
			inputDir = Input.GetVector("left", "right", "forward", "backward");
		} else 
		{
			inputDir = Vector2.Zero;
		}
		direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			if (Input.IsActionPressed("sprint") && inputDir.Y == -1) sprintMultiplier += (float)delta; else sprintMultiplier = 1;
			if (sprintMultiplier > 1.45f) sprintMultiplier = 1.45f;
			velocity.X = direction.X * currSpeed * sprintMultiplier;
			velocity.Z = direction.Z * (currSpeed * sprintMultiplier);
			movementAnimationLerper = Mathf.Clamp(movementAnimationLerper += ((float)delta * 4), 0, 1);
			playerManager.bodyAnimTree.Set("parameters/idle_walk_blend/blend_amount", movementAnimationLerper);

		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, currSpeed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, currSpeed);
			movementAnimationLerper = Mathf.Clamp(movementAnimationLerper -= ((float)delta * 4), 0, 1);
			playerManager.bodyAnimTree.Set("parameters/idle_walk_blend/blend_amount", movementAnimationLerper);

		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
