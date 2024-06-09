using Godot;
using System;

public partial class PlayerBody : CharacterBody3D
{
	[Export] Player_Manager playerManager;
	public const float Speed = 5.0f, flySpeed = 15f;
	private float currSpeed;
	public const float JumpVelocity = 4.5f;
	public Vector2 inputDir;
	public Vector3 direction;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

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
				velocity.Y = JumpVelocity;
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
			velocity.X = direction.X * currSpeed;
			velocity.Z = direction.Z * currSpeed;
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
