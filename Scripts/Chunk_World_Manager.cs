using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public partial class Chunk_World_Manager : Node
{
	public static Chunk_World_Manager instance { get; private set; }
	List<(int, int)> _chunkPositions = new List<(int, int)>();
	private Dictionary<Vector2I, Chunk_Manager> _positionToChunk = new();

	private List<Chunk_Manager> _chunks = new List<Chunk_Manager>();

	[Export] public PackedScene ChunkScene;
	[Export] public Node3D chunkholderNode;

	public int renderDistance = 3;

	[Export] Player_Manager playerManager;
	private Vector3 playerPosition;
	private object playerPositionLock = new(); //This needs to be locked for the playerPosition to be changed

	int centerX = 0;
	int centerY = 0;

	// Flag to check if initial terrain generation is complete
	private bool isInitialGenerationComplete = false;

	public override void _Ready()
	{
		instance = this;
		// Generate only the center chunk at startup
		_ = SpawnInitialChunk();
	}

	public override void _Process(double delta)
	{
		// Wait until the initial terrain generation is complete
		if (!isInitialGenerationComplete)
			return;

		int newCenterX = playerManager.GetChunkPosition().X;
		int newCenterY = playerManager.GetChunkPosition().Y;

		if (newCenterX != centerX || newCenterY != centerY)
		{
			centerX = newCenterX;
			centerY = newCenterY;

			lock (playerPositionLock)
			{
				_ = UpdateChunks();
			}
		}
	}

	private async Task SpawnInitialChunk()
	{
		// Add the initial center chunk
		_chunkPositions.Add((centerX, centerY));
		CallDeferred(nameof(SpawnChunk), centerX, centerY);
		await ToSignal(GetTree().CreateTimer(0.002f), "timeout");

		// After initial chunk, start generating the rest
		await GenerateAndSpawnChunks();

		// Set the flag to true after the initial generation is complete
		isInitialGenerationComplete = true;
	}

	private async Task GenerateAndSpawnChunks()
	{
		await UpdateChunks();
	}

	private async Task UpdateChunks()
	{
		HashSet<(int, int)> newChunkPositions = new HashSet<(int, int)>();

		for (int r = 0; r <= renderDistance; r++)
		{
			for (int x = -r; x <= r; x++)
			{
				newChunkPositions.Add((centerX + x, centerY + r)); // Top row
				if (r != 0) newChunkPositions.Add((centerX + x, centerY - r)); // Bottom row
			}

			for (int y = -r + 1; y <= r - 1; y++)
			{
				newChunkPositions.Add((centerX + r, centerY + y)); // Right column
				if (r != 0) newChunkPositions.Add((centerX - r, centerY + y)); // Left column
			}
		}

		List<(int, int)> chunksToRemove = _chunkPositions.Except(newChunkPositions).ToList();
		List<(int, int)> chunksToAdd = newChunkPositions.Except(_chunkPositions).ToList();

		foreach (var pos in chunksToRemove)
		{
			RemoveChunk(pos.Item1, pos.Item2);
		}

		foreach (var pos in chunksToAdd)
		{
			_chunkPositions.Add(pos);
			CallDeferred(nameof(SpawnChunk), pos.Item1, pos.Item2);
			await ToSignal(GetTree().CreateTimer(0.002f), "timeout");
		}

		_chunkPositions = newChunkPositions.ToList();
	}

	private async void SpawnChunk(int x, int y)
	{
		var chunk = ChunkScene.Instantiate<Chunk_Manager>();
		chunkholderNode.AddChild(chunk);

		await ToSignal(GetTree().CreateTimer(0.002f), "timeout");
		chunk.SetChunkPosition(new Vector2I(x, y));
		chunk.GenerateAndUpdate();

		_chunks.Add(chunk);
		_positionToChunk[new Vector2I(x, y)] = chunk;
	}

	private void RemoveChunk(int x, int y)
	{
		var chunk = _chunks.FirstOrDefault(c => c.GetChunkPosition() == new Vector2I(x, y));
		if (chunk != null)
		{
			chunk.QueueFree();
			_chunks.Remove(chunk);
			_positionToChunk.Remove(new Vector2I(x, y));
		}
	}

	public void SetBlock(Vector3I globalPosition, Block block)
	{
		var chunkTilePosition = new Vector2I(Mathf.FloorToInt(globalPosition.X / 16f), Mathf.FloorToInt(globalPosition.Z / 16f));
		lock (_positionToChunk)
		{
			if (_positionToChunk.TryGetValue(chunkTilePosition, out var chunk))
			{
				chunk.SetBlock((Vector3I)(globalPosition - chunk.GlobalPosition), block);
			}
		}
	}

	public Block GetBlock(Vector3I globalPosition)
	{
		var chunkTilePosition = new Vector2I(Mathf.FloorToInt(globalPosition.X / 16f), Mathf.FloorToInt(globalPosition.Z / 16f));
		lock (_positionToChunk)
		{
			if (_positionToChunk.TryGetValue(chunkTilePosition, out var chunk))
			{
				return chunk.GetBlock((Vector3I)(globalPosition - chunk.GlobalPosition));
			}
			return null;
		}
	}

	public Chunk_Manager GetChunkManagerFromDictionary(Vector2I chunkTilePosition)
	{
		lock (_positionToChunk)
		{
			if (_positionToChunk.TryGetValue(chunkTilePosition, out var chunk))
			{
				return chunk;
			}
			return null;
		}
	}

}

