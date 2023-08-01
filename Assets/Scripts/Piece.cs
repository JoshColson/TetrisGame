using System.Collections;
using Assets.Controllers;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
	public Board board { get; private set; }

	private SceneController sceneController;
	public TetrominoData data { get; private set; }
	public Vector3Int position { get; set; }
	public Vector3Int[] cells { get; set; }
	public int rotationIndex { get; set; }

	public float lockDelay = 0.5f;
	public bool increaseSpeed = false;
	public int difficulty = StartingData.difficultyLevel;
	private float lockTime;

	private float maximumSpeed = 0.4f; // Maximum time in seconds between moves
	public float timeTakes = 600f; // Time in seconds to reach the maximum speed
	private float startingSpeed = 2.0f; // Time in seconds before the first move down event
	private float maxSpeedHardCeiling = 0.05f;
	private float rampTimeHardCeiling = 8f;

	public float speedMultiplier { get; private set; } // Multiplier for speed
	public float currentSpeed; // Current speed
							   //public float elapsedTime = 0.0f; // Elapsed time since the start
	private float stepTimer = 0.0f; // Timer for move down interval
	public float decrementCurrentSpeed;
	public bool IsProposedPiece { get; set; } = false;
	public bool setCollide { private get; set; } = true;

	private void Awake()
	{
		SetDifficultySettings();
		decrementCurrentSpeed = (maximumSpeed - startingSpeed) / timeTakes;
	}


	private void ReturnToBounds(Vector3Int newPosition)
	{
		var validData = board.IsValidPosition(this, newPosition);

		while (validData.outOfBounds)
		{
			if (validData.outOfBoundsLeft)
			{
				newPosition.x++;
			}
			else if (validData.outOfBoundsRight)
			{
				newPosition.x--;
			}
			else if (validData.outOfBoundsBottom)
			{
				newPosition.y++;
			}
			else if (validData.outOfBoundsTop)
			{
				newPosition.y--;
			}
			else
			{
				break;
			}
			validData = board.IsValidPosition(this, newPosition);
		}
		position = newPosition;
	}

	private void ReplacePiece(TetrominoData newPiece)
	{
		Initialize(board, position, newPiece);
		sceneController.NewTetronimo(newPiece);
		ReturnToBounds(position);
	}

	private void SetDifficultySettings()
	{
		speedMultiplier = difficulty - 1;
		currentSpeed = (startingSpeed - (speedMultiplier) * 0.2f);
		timeTakes -= timeTakes * (speedMultiplier / 15);
		maximumSpeed = (maximumSpeed - (speedMultiplier) * 0.2f);
		if (!increaseSpeed)
		{
			maximumSpeed = startingSpeed;
		}
		else if (maximumSpeed < maxSpeedHardCeiling)
		{
			maximumSpeed = maxSpeedHardCeiling;
		}
		if (timeTakes < rampTimeHardCeiling)
		{
			timeTakes = rampTimeHardCeiling;
		}
	}

	public void Initialize(Board board, Vector3Int position, TetrominoData data)
	{
		this.board = board;
		sceneController = board.sceneController;
		this.position = position;
		this.data = data;
		rotationIndex = 0;
		lockTime = 0f;

		if (cells == null)
		{
			cells = new Vector3Int[data.cells.Length];
		}

		for (int i = 0; i < data.cells.Length; i++)
		{
			cells[i] = (Vector3Int)data.cells[i];
		}
	}

	private void Update()
	{
		board.Clear(this);

		//activate input from user
		if (Input.anyKeyDown)
		{
			//return if human input
			PlayerMovement();
		}

		StepController();
		board.Set(this, false);
	}

	private void StepController()
	{
		stepTimer += Time.deltaTime;
		lockTime += Time.deltaTime;

		if (currentSpeed > (maximumSpeed + decrementCurrentSpeed))
		{
			//elapsedTime += Time.deltaTime;
			currentSpeed += decrementCurrentSpeed * Time.deltaTime;
		}

		if (stepTimer >= currentSpeed)
		{
			stepTimer = 0.0f;
			Step();
		}
	}

	private void PlayerMovement()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			if (sceneController.heldTetromino is null)
			{
				sceneController.StoreHeldTetromino();
				ReplacePiece(sceneController.currentTetronimo);
			}
			else
			{
				var held = sceneController.heldTetromino;
				sceneController.StoreHeldTetromino();
				ReplacePiece((TetrominoData)held);
			}
		}
		else if (Input.GetKeyDown(KeyCode.Q))
		{
			Rotate(-1);
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			Rotate(1);
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			Move(Vector2Int.left);
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			Move(Vector2Int.right);
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			Move(Vector2Int.down);
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			HardDrop();
		}
	}



	private void Step()
	{
		if (IsProposedPiece)
		{
			return;
		}

		if((!Move(Vector2Int.down)) && lockTime >= lockDelay)
		{
			Lock() ;
		}
	}

	public void Lock()
	{
		board.Set(this, true);
		board.ClearLines();
		board.SpawnPiece();
	}

	public void HardDrop()
	{
		while (Move(Vector2Int.down))
		{
			continue;
		}
		Lock();
	}

	public bool Move(Vector2Int translation)
	{
		Vector3Int newPosition = position;
		newPosition.x += translation.x;
		newPosition.y += translation.y;

		var validData = board.IsValidPosition(this, newPosition);
		if (!validData.colliding && !validData.outOfBounds)
		{
			board.Clear(this);
			SetPositionAndWait(newPosition);

		}
		return (!validData.colliding) && !validData.outOfBounds;
	}

	public int GetRowNumber()
	{
		// Shift the y position by 10 units so that the range becomes 0 to 20
		float shiftedYPosition = position.y + 10f;

		// Calculate the row number by dividing the shifted y position by the row height
		// The row height is the total range (20 units) divided by the number of rows (20 rows)
		int rowNumber = Mathf.FloorToInt(shiftedYPosition);

		// Clamp the row number to ensure it is within the valid range (0 to 20)
		rowNumber = Mathf.Clamp(rowNumber, 0, 20);

		return rowNumber;
	}

	public void SetPositionAndWait(Vector3Int newPosition)
	{
		position = newPosition;
		lockTime = 0f;
		//wait for 2 seconds without waitforsecondsrealtime or waitforsen
	}

	public void Rotate(int direction)
	{
		int originalRotation = rotationIndex;
		rotationIndex = Wrap(rotationIndex + direction, 0 ,4);

		ApplyRotationMatrix(direction);

		if (!TestWallKicks(rotationIndex, direction))
		{
			rotationIndex = originalRotation;
			ApplyRotationMatrix(-direction);
		}
	}

	private void ApplyRotationMatrix(int direction)
	{
		for (int i = 0; i < data.cells.Length; i++)
		{
			Vector3 cell = cells[i];

			int x, y;

			switch (data.tetromino)
			{
				case Tetromino.I:
				case Tetromino.O:
					cell.x -= 0.5f;
					cell.y -= 0.5f;
					x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
					y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
					break;
				default:
					x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
					y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
					break;

			}

			cells[i] = new Vector3Int(x, y, 0);
		}
	}

	private bool TestWallKicks(int rotationIndex, int rotationDirection)
	{
		int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

		for (int i = 0; i < data.wallKicks.GetLength(1); i++)
		{
			Vector2Int translation = data.wallKicks[wallKickIndex, i];

			if (Move(translation))
			{
				return true;
			}
		}
		return false;
	}

	private int GetWallKickIndex(int rotationIndex, int rotationDirection)
	{
		int wallKickIndex = rotationIndex * 2;

		if (rotationDirection < 0)
		{
			wallKickIndex--;
		}
		return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
	}

	private int Wrap(int input, int min, int max)
	{
		if (input < min)
		{
			return max - (min - input) % (max - min);
		}
		else
		{
			return min + (input - min) % (max - min);
		}
	}

	public bool TileCheck(TileBase tile)
	{
		if (tile == null || 
			tile.name == TileNames.NoCollideTile.ToString() ||
			tile.name == TileNames.PossiblePlacement.ToString())
		{
			return false;
		}
		return true;
	}


}
