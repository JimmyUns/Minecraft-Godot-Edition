using System.Diagnostics;
using Godot;

public partial class Body_Mesh_Controller : Node
{
	[Export] private Node3D head;
	[Export] private Node3D body;
	[Export] private Player_Manager playerManager;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		body.GlobalPosition = head.GlobalPosition + new Vector3(0f, -0.65f, 0f);

		if (playerManager.playerBody.inputDir.Y == -1f)
		{
			body.RotationDegrees = new Vector3(body.RotationDegrees.X, Mathf.Lerp(body.RotationDegrees.Y, head.RotationDegrees.Y, (float) delta * 15f), body.RotationDegrees.Z);
		}
		RotateBodyTowardsHead();
		body.Scale = new Vector3(0.06f, 0.06f, 0.06f);
	}

	private void RotateBodyTowardsHead()
	{
		Vector3 headForward = head.GlobalTransform.Basis.Z;
		headForward.Y = 0;
		headForward = headForward.Normalized();

		Vector3 bodyForward = body.GlobalTransform.Basis.Z;
		bodyForward.Y = 0;
		bodyForward = bodyForward.Normalized();

		float angleBetween = Mathf.RadToDeg(Mathf.Acos(bodyForward.Dot(headForward)));

		if (Mathf.Abs(angleBetween) > 45f)
		{
			float dirSign = Mathf.Sign(bodyForward.Cross(headForward).Y);

			//calculate Rotation based on angle
			Vector3 targetDirection = headForward.Rotated(Vector3.Up, Mathf.DegToRad(-dirSign * 45f)).Normalized();
			Quaternion targetRotation = new Quaternion(Vector3.Up, Mathf.Atan2(targetDirection.X, targetDirection.Z));

			Transform3D bodyTransform = body.GlobalTransform;
			bodyTransform.Basis = new Basis(targetRotation);
			body.GlobalTransform = bodyTransform;
		}
	}
}
