using UnityEngine.Tilemaps;
using UnityEngine;

public enum Tetromino
{
	I,
	O,
	T,
	J,
	L,
	S,
	Z,
}

[System.Serializable]
public struct TetrominoData
{
	public Tetromino tetromino;
	public Tile tile;
	public Tile noCollideTile;
	public Tile possiblePlacementTile;
	public Vector2Int[] cells { get; private set; }
	public Vector2Int[,] wallKicks { get; private set; }

	public void Initialize()
	{
		cells = Data.Cells[tetromino];
		wallKicks = Data.WallKicks[tetromino];

		
		noCollideTile = ScriptableObject.CreateInstance<Tile>(); ;
		noCollideTile.name = TileNames.NoCollideTile.ToString();
		noCollideTile.sprite = tile.sprite;
		noCollideTile.colliderType = tile.colliderType;

		possiblePlacementTile = ScriptableObject.CreateInstance<Tile>();
		possiblePlacementTile.name = TileNames.PossiblePlacement.ToString();
		possiblePlacementTile.sprite = tile.sprite;
		possiblePlacementTile.colliderType = tile.colliderType;
	}

}