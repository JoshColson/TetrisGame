using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HoldTetromino : MonoBehaviour
{

	public Vector3Int[] cells { get; private set; }
	private TetrominoData data;
	public Vector3Int spawnPosition;
	public Tilemap tilemap;



	public void UpdateHeldTetrominoVisuals(TetrominoData? tetronimo)
    {
		Clear();

		if (tetronimo is not null)
		{
			data = (TetrominoData)tetronimo;
			InitializePiece();
			Set();
		}
	}

	private void InitializePiece()
	{
		if (cells == null)
		{
			cells = new Vector3Int[data.cells.Length];
		}

		for (int i = 0; i < data.cells.Length; i++)
		{
			cells[i] = (Vector3Int)data.cells[i];
		}
	}


	private void Set()
	{
		for (int i = 0; i < cells.Length; i++)
		{
			Vector3Int tilePosition = cells[i] + spawnPosition;
			tilemap.SetTile(tilePosition, data.tile);
		}
	}

	private void Clear()
	{
		if (cells!= null)
		{
			for (int i = 0; i < cells.Length; i++)
			{
				Vector3Int tilePosition = cells[i] + spawnPosition;
				tilemap.SetTile(tilePosition, null);
			}
		}

	}
}
