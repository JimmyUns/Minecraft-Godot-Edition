using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Block_Manager : Node
{
	[Export] public Block Air, Dirt, Grass, Stone;
	
	private readonly Dictionary<Texture2D, Vector2I> _atlasLookup = new Dictionary<Texture2D, Vector2I>();
	private int _gridWidth = 4;
	private int _gridHeight;
	
	public Vector2I blockTextureSize {get;} = new(16, 16);
	public Vector2 textureAtlasSize {get; private set; }
	public static Block_Manager Instance {get; private set;}
	public StandardMaterial3D chunkMaterial {get; private set;}

	public override void _Ready()
	{
		Instance = this;
		
		//Set The textures of the block into one Sheet
		//So that we can reuse some of its sprites
		//for example the bottom side of a grass block is the same as a dirt block	
		Texture2D[] blockTextures = new Block[] {Air, Dirt, Grass, Stone}.SelectMany(block => block.Textures).Where(texture => texture != null).Distinct().ToArray();
		
		for (int i = 0; i < blockTextures.Length; i++)
		{
			Texture2D texture = blockTextures[i];
			_atlasLookup.Add(texture, new Vector2I(i % _gridWidth, Mathf.FloorToInt(i / _gridWidth)));
		}	
		
		_gridHeight = Mathf.CeilToInt(blockTextures.Length / (float)_gridWidth); //if we have an image and grid is 4, we create a second row with only 1 cell
		
		Image image = Image.Create(_gridWidth * blockTextureSize.X, _gridHeight * blockTextureSize.Y, false, Image.Format.Rgb8); //Create the actual image
		
		for (int x = 0; x < _gridWidth; x++)
		{
			for (int y = 0; y < _gridHeight; y++)
			{
				int imgIndex = x + y * _gridWidth;
				
				if(imgIndex >= blockTextures.Length) continue;
				
				Image currentImage = blockTextures[imgIndex].GetImage();
				currentImage.Convert(Image.Format.Rgb8);
				
				image.BlitRect(currentImage, new Rect2I(Vector2I.Zero, blockTextureSize), new Vector2I(x, y) * blockTextureSize);
			}
		}
		
		ImageTexture textureAtlas = ImageTexture.CreateFromImage(image);
		
		chunkMaterial = new StandardMaterial3D()
		{
			AlbedoTexture = textureAtlas,
			TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
		};
		
		textureAtlasSize = new Vector2(_gridWidth, _gridHeight);
	}
	
	//Find the image is in the texturesheet
	public Vector2I GetTextureAtlastPosition(Texture2D texture)
	{
		if(texture == null) return Vector2I.Zero;
		
		return _atlasLookup[texture];
	}
}
