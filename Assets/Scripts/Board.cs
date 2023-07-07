
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
	public Tilemap tilemap { private get; set; }
	public Piece activePiece { get; private set; }
	public Text linesClearedText; 
    public TetrominoData[] tetrominoes;
	public Vector3Int spawnPosition;
	public Vector2Int boardSize = new Vector2Int(10, 20);
	private int linesCleared = 0;
	private string linesClearedWriting = "Lines Cleared: ";

	public RectInt Bounds
	{
		get
		{
			Vector2Int position = new Vector2Int(-boardSize.x/2, -boardSize.y/2);
			return new RectInt(position, boardSize);
		}
	}
	private void Awake()
	{
		linesClearedText.text = linesClearedWriting+linesCleared.ToString();
		tilemap = GetComponentInChildren<Tilemap>();
		activePiece = GetComponentInChildren<Piece>();
		for (int i = 0; i < tetrominoes.Length; i++) 
		{
			tetrominoes[i].Initialize();
		}
	}

	private void Start()
	{
		SpawnPiece();
	}

	public void SpawnPiece()
	{
		int random = Random.Range(0, tetrominoes.Length);

		TetrominoData data = tetrominoes[random];

		activePiece.Initialize(this, spawnPosition, data);

		if (IsValidPosition(activePiece, spawnPosition))
		{
			Set(activePiece);
		}
		else
		{
			GameOver();
		}
	}

	private void GameOver()
	{
		tilemap.ClearAllTiles();
	}

	public void Set(Piece piece)
	{
		for (int i = 0; i<piece.cells.Length; i++)
		{
			Vector3Int tilePosition = piece.cells[i] + piece.position;
			tilemap.SetTile(tilePosition, piece.data.tile);
		}
	}
	public void Clear(Piece piece)
	{
		for (int i = 0; i < piece.cells.Length; i++)
		{
			Vector3Int tilePosition = piece.cells[i] + piece.position;
			tilemap.SetTile(tilePosition, null);
		}
	}

	public bool IsValidPosition(Piece piece, Vector3Int position)
	{
		RectInt bounds = Bounds;

		for (int i = 0;i < piece.cells.Length; i++)
		{
			Vector3Int tilePosition = piece.cells[i] + position;

			if (!bounds.Contains((Vector2Int)tilePosition))
			{
				return false;
			}
			if (tilemap.HasTile(tilePosition))
			{
				return false;
			}

		}
		return true;
	}

	public void ClearLines()
	{
		RectInt bounds = Bounds;
		int row = bounds.yMin;

		while (row< bounds.yMax)
		{
			if (IsLineFull(row))
			{
				LineClear(row);
				linesCleared++;
				linesClearedText.text = linesClearedWriting+linesCleared.ToString();
			}
			else
			{
				row++;
			}
		}
	}

	private bool IsLineFull(int row)
	{
		RectInt bounds = Bounds;

		for (int col = bounds.xMin; col< bounds.xMax; col++)
		{
			Vector3Int position = new Vector3Int(col, row, 0);

			if (!tilemap.HasTile(position))
			{
				return false;
			}
		}
		return true;
	}

	private void LineClear(int row)
	{
		RectInt bounds = Bounds;

		for (int col = bounds.xMin; col < bounds.xMax; col++)
		{
			Vector3Int position = new Vector3Int(col, row, 0);
			tilemap.SetTile(position, null);
		}

		while (row < bounds.yMax)
		{
			for (int col = bounds.xMin; col < bounds.xMax; col++)
			{
				Vector3Int position = new Vector3Int(col, row + 1, 0);
				TileBase above = tilemap.GetTile(position);

				position = new Vector3Int(col, row, 0);
				tilemap.SetTile(position, above);
			}
			row++;
		}
	}
}
