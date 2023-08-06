using System;
using Assets.Controllers;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Assets.Data;

public class Board : MonoBehaviour
{
	public TetrominoData[] tetrominos;
	public GameObject ruleBasedAi;
	public Tilemap tilemap { get; set; }
	public Piece activePiece { get; private set; }

	public Vector3Int spawnPosition;
	public Vector2Int boardSize = new Vector2Int(10, 20);
	private int linesCleared = 0;
	private int score = 0;
	public SceneController sceneController { get; private set; }
	public bool ruleBasedAIActive;
	private GameUiController gameUiController;

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
		gameUiController = GameObject.FindWithTag(Tags.SceneController.ToString()).GetComponent<GameUiController>();
		sceneController = new SceneController(tetrominos);
		SetCurrentScore(score);
		SetLinesCleared(linesCleared);
		tilemap = GetComponentInChildren<Tilemap>();
		activePiece = GetComponentInChildren<Piece>();
		//get noCollideTile from the same index tetrominos

	}

	private void SetCurrentScore(int score)
	{
		gameUiController.SetScoreText(score);
	}
	private void SetLinesCleared(int linesCleared)
	{
		gameUiController.SetLinesClearedText(linesCleared);
	}

	private void Start()
	{
		SpawnPiece();
	}

	public void SpawnPiece()
	{
		sceneController.MoveUpTetronimos();
		activePiece.Initialize(this, spawnPosition, sceneController.currentTetronimo);
		var validData = IsValidPosition(activePiece, spawnPosition);

		if (!validData.colliding)
		{
			Set(activePiece, false);
			if (ruleBasedAIActive)
			{
				//ruleBasedAiController.Go();
			}
		}
		else
		{
			GameOver();
		}
	}

	//Get row number from tile position
	public int GetRowNumber(float yPosition)
	{
		// Shift the y position by 10 units so that the range becomes 0 to 20
		float shiftedYPosition = yPosition + 10f;

		// Calculate the row number by dividing the shifted y position by the row height
		// The row height is the total range (20 units) divided by the number of rows (20 rows)
		int rowNumber = Mathf.FloorToInt(shiftedYPosition);

		return rowNumber;
	}

	public bool CheckForTile(Vector2Int position, Piece piece)
	{
		Vector3Int positionToCheck = new Vector3Int(position.x, position.y, 0);
		return piece.TileCheck(tilemap.GetTile(positionToCheck));
	}

	private void GameOver()
	{
		GameData.linesCleared = linesCleared;
		GameData.score = score;
		NavigationController.SceneNavigate(SceneNames.GameOver);
	}

	public void Set(Piece piece, bool collide=true, bool possiblePlacement = false)
	{
		Clear(piece);
		Tile tile;
		if (!collide)
		{
			tile = piece.data.noCollideTile;
			
		}
		else if (possiblePlacement)
		{
			tile = piece.data.possiblePlacementTile;
		}
		else
		{
			tile = piece.data.tile;
		}
		for (int i = 0; i<piece.cells.Length; i++)
		{
			Vector3Int tilePosition = piece.cells[i] + piece.position;
			if (tilePosition.y < Bounds.yMax)
			{
				tilemap.SetTile(tilePosition, tile);
			}
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

	public ValidPiece IsValidPosition(Piece piece, Vector3Int position)
	{
		ValidPiece validData = new ValidPiece();
		RectInt bounds = Bounds;
		for (int i = 0;i < piece.cells.Length; i++)
		{
			Vector3Int tilePosition = piece.cells[i] + position;

			switch (tilePosition)
			{
				case var _ when tilePosition.x < bounds.xMin:
					validData.outOfBoundsLeft = true;
					break;
				case var _ when tilePosition.x >= bounds.xMax:
					validData.outOfBoundsRight = true;
					break;
				case var _ when tilePosition.y > spawnPosition.y+10:
					validData.outOfBoundsTop = true;
					break;
				case var _ when tilePosition.y < bounds.yMin:
					validData.outOfBoundsBottom = true;
					break;
				default:
					break;
			}

			validData.outOfBounds =
				validData.outOfBoundsTop ||
				validData.outOfBoundsBottom ||
				validData.outOfBoundsLeft ||
				validData.outOfBoundsRight;

			if (validData.outOfBounds)
			{
				break;
			}
			TileBase tile = tilemap.GetTile(tilePosition);
			if (piece.TileCheck(tile))
			{
				validData.colliding = true;
			}
		}
		return validData;
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
				SetLinesCleared(++linesCleared);
			}
			else
			{
				perfectClear = false;
				row++;
			}
		}
		score += CalculateScore(perfectClear, (linesCleared - cleared));
		SetCurrentScore(score);
	}

	private int CalculateScore(bool perfectClear, int linesCleared)
	{
		var score = 0;
		switch (linesCleared)
		{
			case 1:
				score = (int)ScoreValues.pointsOneLine;
				break;
			case 2:
				score = (int)ScoreValues.pointsTwoLine;
				break;
			case 3:
				score = (int)ScoreValues.pointsThreeLine;
				break;
			case 4:
				score = (int)ScoreValues.pointsFourLine;
				break;
			default:
				return 0;
		}
		if (perfectClear)
		{
			score = (score * (int)ScoreValues.perfectClearMultiplyer);
		}
		if ((int) Math.Round(score *+ (activePiece.speedMultiplier * 0.5))==0)
		{
			return score;
		}

		return Mathf.RoundToInt(Mathf.Clamp((float)(score + score * (activePiece.speedMultiplier * 0.5)), score, float.MaxValue));
	}
	public bool IsLineFull(int row)
	{
		RectInt bounds = Bounds;

		for (int col = bounds.xMin; col< bounds.xMax; col++)
		{
			if (!CheckForTile(new Vector2Int(col, row), activePiece))
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
