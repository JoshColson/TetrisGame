using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Button = UnityEngine.UI.Button;
using Unity.VisualScripting;

public class PlayerController_RuleBasedAI : MonoBehaviour
{
	public Button activateAi;
	private Tilemap tileMap { get; set; }
	private Board board { get; set; }
	private Piece piece { get; set; }
	private List<PossibleMove> possibleMoves { get; set; } = new List<PossibleMove>();
	private PossibleMove bestMove { get; set; }
	private TetrominoData nextTetromino { get; set; }
	private TetrominoData currentTetromino { get; set; }
	private TetrominoData? heldTetromino { get; set; }
	private bool heldTetrominoCheck { get; set; } = false;
	private bool nextTetrominoCheck { get; set; } = false;
	private RectInt bounds { get; set; }

	private void Awake()
	{
		var mainBoard = GameObject.FindWithTag(Tags.Board.ToString());
        tileMap = mainBoard.GetComponentInChildren<Tilemap>();
        board = mainBoard.GetComponent<Board>();
		bounds = board.Bounds;
	}

	private void Start()
	{
		activateAi.onClick.AddListener(OnButtonClick);
	}

	private void OnButtonClick()
	{
		activateAi.enabled = false; // Disable button while AI is running
		Go();
		activateAi.enabled = true; // Reactivate button
	}

	public void Go()
	{
		//Set up variables
		piece = board.activePiece;
		nextTetromino = board.sceneController.nextTetronimo;
		heldTetromino = board.sceneController.heldTetromino;
		currentTetromino = piece.data;
		//Clear board
		possibleMoves.Clear();
		//Collect all possible moves
		CollectPossibleMoves();
		//Find best move
		bestMove = possibleMoves.OrderByDescending(move => move.score).FirstOrDefault();
		//Move into best move
		MoveIntoBestMove();
		//Drop piece
		piece.HardDrop();
	}

	private void MoveIntoBestMove()
	{
		//Swap to correct piece
		SwapToCorrectPiece();

		//Rotate into the best move's rotational position
		MultipleRotate(bestMove.rotationIndex);

		//Move into the best move's x position
		MultipleMoveXPos(bestMove.position.x);
	}

	private void MultipleMoveXPos(int desiredXPos)
	{
		while (piece.position.x != desiredXPos) //While the piece is not in the correct x position
		{
			if (piece.position.x > desiredXPos) //If the piece is to the right of the desired x position
			{
				piece.Move(Vector2Int.left); //Move left
			}
			else //Otherwise the piece is to the left of the desired x position
			{
				piece.Move(Vector2Int.right); //Move right
			}
		}
	}

	private void MultipleMoveMax(Vector2Int direction)
	{
		// Move in the desired direction until it can no longer move
		while (piece.Move(direction)) 
		{ 
			if (direction == Vector2Int.up && piece.position.y == board.spawnPosition.y) // If the piece is at the top of the board
			{
				return; // Break out of the loop and method
			}
		}
	}

	private void MultipleRotate(int desiredRotationIndex)
	{
		while (piece.currentRotationIndex != desiredRotationIndex) //While the piece is not in the correct rotation
		{
			piece.Rotate(1); //Rotate clockwise
		}
	}

	private void SwapToCorrectPiece() 
	{
		if (!bestMove.heldTetromino && !bestMove.nextTetromino) //If the best move is the current active piece
		{ 
			piece.ReplacePiece(currentTetromino); //Ensure the active piece is the current tetromino
		}
		else //Otherwise swap to the correct piece
		{
			piece.SwapTetromino(); //Spawn the correct piece, and update the held/next tetromino correctly
		}
	}

	private void CollectPossibleMoves()
	{
		MoveGetScore(); //Gather possible move data from current piece

		//If there is a held piece
		if (heldTetromino != null)
		{
			//Switch to held piece and gather possible move data
			heldTetrominoCheck = true;
			board.Clear(piece);
			piece.Initialize(board, piece.position, (TetrominoData)heldTetromino);
			MoveGetScore();// gather data from held piece
			heldTetrominoCheck = false;
		}
		//If there's no held piece, gather the next piece move data instead
		else
		{
			//Switch to next piece and gather possible move data
			nextTetrominoCheck = true;
			board.Clear(piece);
			piece.Initialize(board, piece.position, nextTetromino);
			MoveGetScore();// gather data from next piece
			nextTetrominoCheck = false;
		}
	}

	private void MoveGetScore()
	{
		//Move to the left most position
		MultipleMoveMax(Vector2Int.left);
		do //while the piece is not the original rotation
		{
			do //While the piece is not the maximum to the right it can be
			{
				//Move down as far as possible
				MultipleMoveMax(Vector2Int.down);
				//Add the current position and rotation, along with it's move score, to the list of possible moves
				SetPossibleMove();
				//Reset piece Y position
				MultipleMoveMax(Vector2Int.up);
			}
			while (piece.Move(Vector2Int.right)); //Break if piece is all the way to the right

			piece.Rotate(1); //Rotate clockwise once
			MultipleMoveMax(Vector2Int.left); //Reset X position
		}
		while (piece.currentRotationIndex != 0); // Break if piece is same as original rotation
	}

	private void SetPossibleMove()
	{
		//Create new possible move
		var possibleMove = new PossibleMove
		{
			position = piece.position,
			rotationIndex = piece.currentRotationIndex,
			score = CalculateMoveScore() //Collect Possible move score
		};
		//If the possible move piece is the next tetromino, set bool to true
		if (nextTetrominoCheck)
		{
			possibleMove.nextTetromino = true;
		}
		//If the possible move piece is the held tetromino, set bool to true
		else if (heldTetrominoCheck)
		{
			possibleMove.heldTetromino = true;
		}
		//Add the possible move to the list of possible moves
		possibleMoves.Add(possibleMove);
	}

	private int CalculateMoveScore()
	{
		//Set current piece into proposed position and rotation on TileMap
		board.Set(piece, true);

		//Get New state of TileMap
		var newGameState = GetGameState();

		//Remove proposed piece from TileMap
		board.Clear(piece);

		//Calculate and return the proposed move score from new game state
		return ReturnGameStateScore(newGameState);
	}



	private bool GameStateCheckLongGap(int row, int col)
	{
		// Check if the border is to the left
		var foundLeftTile = col - 1 <= bounds.xMin;
		// Check if the border is to the right
		var foundRightTile = col + 1 >= bounds.xMax;
		// If the border is not to the left, check if there is a tile to the left
		if (!foundLeftTile)
		{
			//Check for tile to the left
			foundLeftTile = board.CheckForTile(new Vector2Int(col - 1, row), piece); 
		}
		// If the border is not to the right, check if there is a tile to the right
		if (!foundRightTile && foundLeftTile)
		{
			//Check for tile to the right
			foundRightTile = board.CheckForTile(new Vector2Int(col + 1, row), piece); 
		}
		//return true if there is a tile or border to the left and the right
		return foundRightTile && foundLeftTile;
	}

	private void GameStateSearchTilesAbove(GameState gameState, int currentRow, int col)
	{
		var longGapFound = false;
		//Loop over every row directly above currentRow
		for (int row = currentRow + 1; row < bounds.yMax; row++)
		{
			// If there is a tile, break out of the method and count a trapped space
			if (board.CheckForTile(new Vector2Int(col, row), piece))
			{
				gameState.trappedTileSpaces++;
				return;
			}
			// If there is not a tile 2 rows above, and a long gap hasn't already been found, check if there is a long gap
			else if (row >= currentRow + 2 && !longGapFound)
			{
				if (GameStateCheckLongGap(row, col))
				{
					gameState.longGaps++;
				}
			}
		}
	}

	private void GameStateSearchTilesInRow(GameState gameState, int row)
	{
		//Loop over every column in row
		for (int col = bounds.xMin; col < bounds.xMax; col++)
		{
			//If a tile is found, row is not empty
			if (board.CheckForTile(new Vector2Int(col, row), piece))
			{
				gameState.totalHeight = board.GetRowNumber(row);
			}
			//If a tile is not found, check the tiles above for trapped spaces and long gaps
			else
			{
				GameStateSearchTilesAbove(gameState, row, col);
			}
		}
		//If the row is full, add to completeLines
		if (board.IsLineFull(row))
		{
			gameState.completeLines++;
		}
	}

	private GameState GetGameState()
	{
		var gameState = new GameState();

		//Get the height of the piece in the proposed move
		gameState.pieceHeight = piece.GetRowNumber();
		//Loop over every row in the board; bottup to top
		for (int row = bounds.yMin; row < bounds.yMax; row++)
		{
			//Get game state data for current row
			GameStateSearchTilesInRow(gameState, row);
			//breaj if total height has been found, as there are no more game pieces above
			if (gameState.totalHeight == board.GetRowNumber(row - 1))
			{
				break;
			}
		}
		return gameState;
	}

	private int ReturnGameStateScore(GameState gameState)
	{
		//Initiate score
		var score = 0;

		//Calculate score based on the row height of the proposed piece, encouraging placing pieces lower down, and discouraging placing pieces higher up
		float pieceExponent = 0.1f + 0.1f * (float)gameState.pieceHeight;
		int pieceHeightMaxScore = gameState.pieceHeight <= 10 ? 0 : 20; // Maximum score is 0 if pieceHeight <= 10, otherwise 20
		score += Mathf.RoundToInt(pieceHeightMaxScore - Mathf.Pow((float)gameState.pieceHeight, pieceExponent));

		//Calculate score based on the number of trapped spaces in the game state, encouraging placing pieces in positions that create less trapped spaces
		score += gameState.trappedTileSpaces * (int)AiPositionScoreSheet.TrappedGapsCreated;

		//Calculate score based on the number of complete lines in the game state, encouraging placing pieces in positions that create more complete lines
		score += gameState.completeLines * (int)AiPositionScoreSheet.LineClear;

		//Calculate score based on the number of long gaps in the game state, encouraging placing pieces in positions that create less long gaps
		score += gameState.longGaps <= 1 ? 0 : gameState.longGaps * (int)AiPositionScoreSheet.LongGapsCreated;

		//Return score
		return score;
	}

}
