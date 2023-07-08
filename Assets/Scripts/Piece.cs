using System;
using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
	public Board board { get; private set; }
	public TetrominoData data { get; private set; }
	public Vector3Int position { get; private set; }
	public Vector3Int[] cells { get; private set; }
	public int rotationIndex { get; private set; }

	public float stepDelay = 1f;
	public float lockDelay = 0.5f;
	public Double moveDelay = 0.15;
	public bool increaseSpeed = false;
	public int difficulty = 1;
	private float moveTime;
	private float stepTime;
	private float lockTime;
	public Double fallTime=0;
	public Double fallLock=0.97;

	public void Initialize(Board board, Vector3Int position, TetrominoData data)
	{
		this.board = board;
		this.position = position;
		this.data = data;
		rotationIndex = 0;
		stepTime = Time.time + stepDelay;
		moveTime = Time.time + (float)moveDelay;
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

		if (increaseSpeed && fallTime < fallLock)
		{
			fallTime += (difficulty * 0.00003);
			if (moveDelay < 0.05)
			{
				moveDelay-= (difficulty * 0.0000001);
			}
		}

		lockTime += Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Q))
		{
			Rotate(-1);
		}
		else if (Input.GetKeyDown(KeyCode.E)) 
		{
			Rotate(1);
		}

		if (Input.GetKey(KeyCode.A))
		{
			if (Time.time >=  moveTime)
			{
				Move(Vector2Int.left);
			}
		}
		else if (Input.GetKey(KeyCode.D))
		{
			if (Time.time >= moveTime)
			{
				Move(Vector2Int.right);
			}
		}

		if (Input.GetKey(KeyCode.S))
		{
			if (Time.time >= moveTime)
			{
				Move(Vector2Int.down);
			}
		}

		if (Input.GetKeyDown(KeyCode.Space)) 
		{
			HardDrop();
		}

		if ((Time.time+fallTime) >= stepTime){
			Step();
		}
		board.Set(this);
	}


	private void Step()
	{
		stepTime = Time.time + stepDelay;

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

		bool valid = board.IsValidPosition(this, newPosition);
		if (valid)
		{
			position = newPosition;
			lockTime = 0f;
			moveTime = Time.time + (float)moveDelay;
		}
		return valid;
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
