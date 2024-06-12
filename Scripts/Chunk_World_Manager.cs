using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public partial class Chunk_World_Manager : Node
{
	public static Chunk_World_Manager instance { get; private set; }

	private Dictionary<Chunk_Manager, Vector2I> _chunkToPosition = new();
	private Dictionary<Vector2I, Chunk_Manager> _positionToChunk = new();

	private List<Chunk_Manager> _chunks;

	[Export] public PackedScene ChunkScene;

	public int renderDistance = 4;

	[Export] Player_Manager playerManager;
	private Vector3 playerPosition;
	private object playerPositionLock = new(); //This needs to be locked for the playerPosition to be changed


	public override void _Ready()
	{
		instance = this;

		_chunks = GetParent().GetChildren().Where(child => child is Chunk_Manager).Select(child => child as Chunk_Manager).ToList();

		for (int i = _chunks.Count; i < renderDistance * renderDistance; i++)
		{
			var chunk = ChunkScene.Instantiate<Chunk_Manager>();
			GetParent().CallDeferred(Node.MethodName.AddChild, chunk);
			_chunks.Add(chunk);
		}

		for (int x = 0; x < renderDistance; x++)
		{
			for (int y = 0; y < renderDistance; y++)
			{
				var index = (y * renderDistance) + x;
				var halfWidth = Mathf.FloorToInt(renderDistance / 2f);
				_chunks[index].SetChunkPosition(new Vector2I(x - halfWidth, y - halfWidth));
			}
		}
		
		new Thread(new ThreadStart(ThreadProcess)).Start();
	}

	public void UpdateChunkPosition(Chunk_Manager chunk, Vector2I currentPosition, Vector2I perviousPosition)
	{
		if (_positionToChunk.TryGetValue(perviousPosition, out var chunkAtPosition) && chunkAtPosition == chunk)
		{
			_positionToChunk.Remove(perviousPosition);
		}

		_chunkToPosition[chunk] = currentPosition;
		_positionToChunk[currentPosition] = chunk;
	}

	public void SetBlock(Vector3I globalPosition, Block block)
	{
		var chunkTilePosition = new Vector2I(Mathf.FloorToInt(globalPosition.X / (float)Chunk_Manager.dimensions.X), Mathf.FloorToInt(globalPosition.Z / (float)Chunk_Manager.dimensions.Z));
		lock (_positionToChunk)
		{
			if (_positionToChunk.TryGetValue(chunkTilePosition, out var chunk))
			{
				chunk.SetBlock((Vector3I)(globalPosition - chunk.GlobalPosition), block);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		lock (playerPositionLock)
		{
			playerPosition = playerManager.playerBody.GlobalPosition;
		}
	}


	//Seperate Thread
	private void ThreadProcess()
	{
		//Keep running as long as Chunk_World_manager exists
		while (IsInstanceValid(this))
		{
			int playerChunkX, playerChunkZ;
			lock (playerPositionLock)
			{
				playerChunkX = Mathf.FloorToInt(playerPosition.X / Chunk_Manager.dimensions.X);
				playerChunkZ = Mathf.FloorToInt(playerPosition.Z / Chunk_Manager.dimensions.Z);
			}

			foreach (var chunk in _chunks)
			{
				var chunkPosition = _chunkToPosition[chunk];
				var chunkX = chunkPosition.X;
				var chunkZ = chunkPosition.Y;

				var newChunkX = (int)(Mathf.PosMod(chunkX - playerChunkX + renderDistance / 2, renderDistance) + playerChunkX - renderDistance / 2);
				var newChunkZ = (int)(Mathf.PosMod(chunkZ - playerChunkZ + renderDistance / 2, renderDistance) + playerChunkZ - renderDistance / 2);

				if (newChunkX != chunkX || newChunkZ != chunkZ)
				{
					lock (_positionToChunk)
					{
						if (_positionToChunk.ContainsKey(chunkPosition))
						{
							_positionToChunk.Remove(chunkPosition);
						}

						var newPosition = new Vector2I(newChunkX, newChunkZ);

						_chunkToPosition[chunk] = newPosition;
						_positionToChunk[newPosition] = chunk;

						chunk.CallDeferred(nameof(chunk.SetChunkPosition), newPosition);
					}

					Thread.Sleep(100);
				}
			}
			
			Thread.Sleep(100);
		}
	}

}
