using Godot;
using System;
using System.Collections.Generic;

public partial class Seed_Generator : Node2D
{
	[Export]
	public int width = 250;
	[Export]
	public int height = 250;
	[Export] private TileMap tileM;
	
	[Export] public int octaves, gain;
	[Export] public float Lacunarity;
	public FastNoiseLite fastNoiseLite = new();
	public override void _Ready()
	{
		generate_world();
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("action_0")) generate_world();
	}
	
	public void generate_world()
	{
		// A random number generator which we will use for the noise seed
		RandomNumberGenerator rng = new();
		// The list of tiles we want to use with the noise. Order matters !
		List<Vector2I> tilesList = new();

		tilesList.Add(TileDictionary.WATER);
		tilesList.Add(TileDictionary.SAND);
		tilesList.Add(TileDictionary.GRASSIER_GRASS);
		tilesList.Add(TileDictionary.GRASS);
		tilesList.Add(TileDictionary.ROCK);
		tilesList.Add(TileDictionary.ROCKIER_ROCK);

		rng.Randomize();
		fastNoiseLite.Seed = rng.RandiRange(0, 500);

		// Try out other parameters from [NoiseTypeEnum] for cool variants !
		fastNoiseLite.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
		// The number of layers we want to generate on the noise. Each tile will have its own layer.
		fastNoiseLite.FractalOctaves = octaves;
		fastNoiseLite.FractalGain = gain;
		fastNoiseLite.FractalLacunarity = Lacunarity;
		
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				// We get the noise coordinate as an absolute value (which represents the gradient - or layer) .
				float newNoise = 0f;
				if(fastNoiseLite.GetNoise2D(x, y) > 0)
				{
					newNoise = fastNoiseLite.GetNoise2D(x, y) - 3;

				}
				
				float absNoise = Math.Abs(fastNoiseLite.GetNoise2D(x, y));
				// We determine which tile our value corresponds to.
				int tileToPlace = (int)Math.Floor((absNoise * octaves));
				tileM.SetCell(0, new(x, y), 0, tilesList[tileToPlace]);
			}
		}
	}
}
public class TileDictionary
{
	public static Vector2I WATER = new(0, 0);
	public static Vector2I SAND = new(2, 0);
	public static Vector2I GRASS = new(0, 2);
	public static Vector2I GRASSIER_GRASS = new(2, 4);
	public static Vector2I ROCK = new(2, 2);
	public static Vector2I ROCKIER_ROCK = new(0, 4);
}
