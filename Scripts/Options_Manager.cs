using Godot;
using System;

public partial class Options_Manager : Node
{
	public static Options_Manager instance { get; private set; }

	[Export] private Control options;
	[Export] private Button changeskinButton;
	[Export] private FileDialog skinFileDialogue;

	public override void _Ready()
	{
		instance = this;
	}

	public void ShowOptions(bool state)
	{
		if (state)
		{
			options.Visible = true;
			options.MouseFilter = Control.MouseFilterEnum.Pass;
			changeskinButton.Text = "Change Skin";
		}
		else
		{
			options.Visible = false;
			options.MouseFilter = Control.MouseFilterEnum.Ignore;
		}
	}

	public void _on_back_button_pressed()
	{
		options.Visible = false;
		options.MouseFilter = Control.MouseFilterEnum.Ignore;
	}

	public void _on_change_skin_button_pressed()
	{
		skinFileDialogue.Popup();
	}

	public void _on_file_dialog_file_selected(String path)
	{
		var image = new Image();
		image.Load(path);
		image.Convert(Image.Format.Rgba8);
		Game_manager.instance.skin = ImageTexture.CreateFromImage(image);
		changeskinButton.Text = "Skin Changed!";

	}
}
