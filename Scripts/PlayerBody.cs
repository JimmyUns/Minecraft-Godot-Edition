using Godot;
using System;
using System.Diagnostics;

public partial class PlayerBody : CharacterBody3D
{
	[Export] Player_Manager playerManager;
	[Export] public float Speed = 5.0f, flySpeed = 15f;
	private float sprintMultiplier =  1f;
	private float currSpeed;
	public float JumpVelocity = 8.8f;
	public Vector2 inputDir;
	public Vector3 direction;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = 35;
	private float gravityMultiplyer = 0.1f;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		if (playerManager.gameMode == 0)
		{
			currSpeed = Speed;

			// Add the gravity.
			if (!IsOnFloor())
				velocity.Y -= gravity * (float)delta;

			// Handle Jump.
			if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			{
				gravityMultiplyer = 0.1f;
				velocity.Y = JumpVelocity;
			}
				
			gravityMultiplyer += (float) delta / 3f;
			if(gravityMultiplyer > 1f) gravityMultiplyer = 1f;
			if(velocity.Y < -40 * gravityMultiplyer) velocity.Y = -40f * gravityMultiplyer;
		}
		else if (playerManager.gameMode == 1)
		{
			currSpeed = flySpeed;
			if (Input.IsActionPressed("ui_accept"))
				velocity.Y += JumpVelocity;

			else if (Input.IsActionPressed("sneak"))
				velocity.Y -= JumpVelocity;
				
			else velocity.Y = 0f;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		inputDir = Input.GetVector("left", "right", "forward", "backward");
		direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			if(Input.IsActionPressed("sprint") && inputDir.Y == -1) sprintMultiplier += (float)delta; else sprintMultiplier = 1;
			if(sprintMultiplier > 1.3f) sprintMultiplier = 1.3f;
			velocity.X = direction.X * currSpeed;
			velocity.Z = direction.Z * (currSpeed * sprintMultiplier);
			Debug.Print((currSpeed * sprintMultiplier).ToString());
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, currSpeed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, currSpeed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
