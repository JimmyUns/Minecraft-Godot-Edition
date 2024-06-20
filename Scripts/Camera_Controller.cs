using Godot;
using System;

public partial class Camera_Controller : Node
{
	[Export] public float mouseSensitivity;
	[Export] public Camera3D mainCamera, subviewportCamera;
	[Export] public Node3D camHolder, playerHead;
	[Export] public Node3D mainBody;
	[Export] public Node3D headPivot;
	[Export] private Player_Manager playerManager;
	[Export] private SpringArm3D cameraSpringArm;
	public bool lockRotation;

	public override void _Ready()
	{
		TogglePerspective(0);
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && lockRotation == false)
		{
			float current_mouseSensitivity = mouseSensitivity * 0.005f;
			InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;

			camHolder.GlobalRotationDegrees = new Vector3(camHolder.GlobalRotationDegrees.X, camHolder.GlobalRotationDegrees.Y + -mouseMotion.Relative.X * current_mouseSensitivity * 50f, camHolder.GlobalRotationDegrees.Z);
			mainBody.GlobalRotationDegrees = camHolder.GlobalRotationDegrees;

			playerHead.RotateX(-mouseMotion.Relative.Y * current_mouseSensitivity);
			Vector3 cameraRot = playerHead.Rotation;
			cameraRot.X = Mathf.Clamp(cameraRot.X, Mathf.DegToRad(-90f), Mathf.DegToRad(90f));
			cameraRot.Y = 0;
			cameraRot.Z = 0;
			playerHead.Rotation = cameraRot;
			headPivot.Rotation = cameraRot;
		}
	}
	public override void _Process(double delta)
	{

		camHolder.GlobalPosition = mainBody.GlobalPosition + new Vector3(0f, 1.72f, 0f);
		subviewportCamera.GlobalPosition = camHolder.GlobalPosition;
		subviewportCamera.GlobalRotation = mainCamera.GlobalRotation;
	}

	public void TogglePerspective(int perspectiveIndex)
	{
		switch (perspectiveIndex)
		{
			case 0:
				cameraSpringArm.SpringLength = 0f;
				mainCamera.Rotation = Vector3.Zero;
				playerManager.lowerbodyMesh.Visible = false;
				playerManager.bodyMeshMaker.head.Visible = false;
				break;
			case 1:
				cameraSpringArm.SpringLength = 4f;
				mainCamera.Rotation = new Vector3(0f, 0f, 0f);
				playerManager.lowerbodyMesh.Visible = true;
				playerManager.bodyMeshMaker.head.Visible = true;
				break;
			case 2:
				cameraSpringArm.SpringLength = -4f;
				mainCamera.RotationDegrees = new Vector3(0f, 180f, 0f);
				playerManager.lowerbodyMesh.Visible = true;
				playerManager.bodyMeshMaker.head.Visible = true;
				break;
		}
	}
}
