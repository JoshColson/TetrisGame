
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
	const string linesClearedWriting = "Lines Cleared: ";
	const string gamesWriting = "Games: ";
	const string scoreWriting = "Score: ";
	const int pointsOneLine = 40;
	const int pointsTwoLine = 100;
	const int pointsThreeLine = 300;
	const int pointsFourLine = 1200;
	const int perfectClearMultiplyer = 10;

	public Tilemap tilemap { private get; set; }
	public Piece activePiece { get; private set; }
	public Text linesClearedText; 
	public Text gamesText;
	public Text scoreText;
    public TetrominoData[] tetrominoes;
	public Vector3Int spawnPosition;
	public Vector2Int boardSize = new Vector2Int(10, 20);
	private int linesCleared = 0;
	private int games = 0;
	private int score = 0;

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
		gamesText.text = gamesWriting+games.ToString();
		scoreText.text = scoreWriting+score.ToString();

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
		games++;
		gamesText.text = gamesWriting + games.ToString();
		linesCleared = 0;
		linesClearedText.text = linesClearedWriting + linesCleared.ToString();
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
		var row = bounds.yMin;
		var perfectClear = true;
		var cleared = linesCleared;

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
				perfectClear = false;
				row++;
			}
		}
		score += CalculateScore(perfectClear, (linesCleared - cleared));
		scoreText.text = scoreWriting + score.ToString();
	}

	private int CalculateScore(bool perfectClear, int linesCleared)
	{
		var score = 0;
		switch (linesCleared)
		{
			case 1:
				score = pointsOneLine;
				break;
			case 2:
				score = pointsTwoLine;
				break;
			case 3:
				score = pointsThreeLine;
				break;
			case 4:
				score = pointsFourLine;
				break;
			default:
				return 0;
		}
		if (perfectClear)
		{
			score = score * perfectClearMultiplyer;
		}
		return score;
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
