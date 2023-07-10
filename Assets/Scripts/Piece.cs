﻿using System;
using UnityEngine;

public class Piece : MonoBehaviour
{
	public Board board { get; private set; }
	public TetrominoData data { get; private set; }
	public Vector3Int position { get; private set; }
	public Vector3Int[] cells { get; private set; }
	public int rotationIndex { get; private set; }

	public float lockDelay = 0.5f;
	public bool increaseSpeed = false;
	public int difficulty;
	private float lockTime;

	private float maximumSpeed = 0.4f; // Maximum time in seconds between moves
	private float timeTakes = 600f; // Time in seconds to reach the maximum speed
	private float startingSpeed = 2.0f; // Time in seconds before the first move down event
	private float maxSpeedHardCeiling = 0.05f;
	private float rampTimeHardCeiling = 8f;

	public float speedMultiplier { get; private set; } // Multiplier for speed
	private float currentSpeed; // Current speed
	private float elapsedTime = 0.0f; // Elapsed time since the start
	private float moveDownTimer = 0.0f; // Timer for move down interval


	private void Awake()
	{
		SetDifficultySettings();
	}

	private void SetDifficultySettings()
	{
		speedMultiplier = difficulty - 1;
		currentSpeed = (startingSpeed - (speedMultiplier) * 0.1f);
		timeTakes -= timeTakes * (speedMultiplier / 15);
		maximumSpeed = (maximumSpeed - (speedMultiplier) * 0.1f);
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
		difficulty = StartingData.difficultyLevel;
		this.board = board;
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

		if (Input.anyKeyDown)
		{
			PlayerMovement();
		}

		StepController();
		board.Set(this);
	}

	private void StepController()
	{
		moveDownTimer += Time.deltaTime;
		lockTime += Time.deltaTime;

		if (!(currentSpeed <= maximumSpeed))
		{
			elapsedTime += Time.deltaTime;
			float timeRemaining = timeTakes - elapsedTime;
			currentSpeed = maximumSpeed - (timeRemaining / timeTakes) * (maximumSpeed - startingSpeed);
			currentSpeed = Mathf.Clamp(currentSpeed, maximumSpeed, startingSpeed);
		}

		if (moveDownTimer >= currentSpeed)
		{
			moveDownTimer = 0.0f;
			Step();
		}
	}

	private void PlayerMovement()
	{
		if (Input.GetKeyDown(KeyCode.Q))
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
		Move(Vector2Int.down);

		if(lockTime >= lockDelay)
		{
			Lock() ;
		}
	}

	private void Lock()
	{
		board.Set(this);
		board.ClearLines();
		board.SpawnPiece();
	}

	private void HardDrop()
	{
		while (Move(Vector2Int.down))
		{
			continue;
		}
		Lock();
	}

	private bool Move(Vector2Int translation)
	{
		Vector3Int newPosition = position;
		newPosition.x += translation.x;
		newPosition.y += translation.y;

		var validData = board.IsValidPosition(this, newPosition);
		if ((!validData.colliding) && !validData.outOfBounds)
		{
			position = newPosition;
			lockTime = 0f;
		}
		return (!validData.colliding) && !validData.outOfBounds;
	}

	private void Rotate(int direction)
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


}
