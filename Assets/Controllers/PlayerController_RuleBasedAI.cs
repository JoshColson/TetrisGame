using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Button = UnityEngine.UI.Button;

public class PlayerController_RuleBasedAI : MonoBehaviour
{
    public GameObject mainBoard;
	public GameObject ghostBoard;

    private Tilemap mainTileMap;
	private Board board;
    private Piece piece;

	private List<PossibleMove> possibleMoves = new List<PossibleMove>();
	private PossibleMove bestMove;
	public Button activateAi;

	private TetrominoData nextTetromino { get; set; }
	private TetrominoData currentTetromino { get; set; }
	private TetrominoData? heldTetromino { get; set; }

	private bool heldTetrominoCheck { get; set; } = false;

	private bool nextTetrominoCheck { get; set; } = false;

	private void Awake()
	{
        mainTileMap = mainBoard.GetComponentInChildren<Tilemap>();
        board = mainBoard.GetComponent<Board>();
	}

	private void Start()
	{
		activateAi.onClick.AddListener(OnButtonClick);
	}

	private void OnButtonClick()
	{
		activateAi.enabled = false;
		Go();
		activateAi.enabled = true;
	}

	public void Go()
	{
		piece = board.activePiece;

		nextTetromino = board.sceneController.nextTetronimo;
		heldTetromino = board.sceneController.heldTetromino;
		currentTetromino = piece.data;

		possibleMoves.Clear();
		CollectPossibleMoves();
		bestMove = possibleMoves.OrderByDescending(move => move.score).FirstOrDefault();
		MoveIntoBestMove();
		piece.HardDrop();
	}



	//private IEnumerator Wait(float duration)
	//{
	//	yield return new WaitForSeconds(duration);
	//}

	private void MoveIntoBestMove()
	{
		if (!bestMove.heldTetromino && !bestMove.nextTetromino)
		{
			piece.ReplacePiece(currentTetromino);
		}
		else
		{
			piece.SwapTetromino();
		}


		while (piece.rotationIndex != bestMove.rotationIndex)
		{
			piece.Rotate(1);
			//StartCoroutine(Wait(1.0f));
		}
		while (piece.position.x != bestMove.position.x)
		{
			if (piece.position.x > bestMove.position.x)
			{
				piece.Move(Vector2Int.left);
				//StartCoroutine(Wait(1.0f));
			}
			else
			{
				piece.Move(Vector2Int.right);
				//StartCoroutine(Wait(1.0f));
			}
		}
	}

	private void CollectPossibleMoves()
	{
		MoveGetScore(); //gather data from current piece

		//Switch to held piece
		if (heldTetromino != null)
		{
			heldTetrominoCheck = true;
			board.Clear(piece);
			piece.Initialize(board, piece.position, (TetrominoData)heldTetromino);
			MoveGetScore();// gather data from held piece
			heldTetrominoCheck = false;
		}
		else
		{
			//Switch to next piece
			nextTetrominoCheck = true;
			board.Clear(piece);
			piece.Initialize(board, piece.position, nextTetromino);
			MoveGetScore();// gather data from next piece
			nextTetrominoCheck = false;
		}

	}

	private void MoveGetScore()
	{
		while (piece.Move(Vector2Int.left)) { }

		do //while the piece is not the original rotation
		{
			do //While the piece is not the maximum to the right it can be
			{
				while (piece.Move(Vector2Int.down)) { };
				board.Set(piece, possiblePlacement: true);
				var possibleMove = new PossibleMove
				{
					position = piece.position,
					rotationIndex = piece.rotationIndex,
					score = CalculateMoveScore()
				};
				if (nextTetrominoCheck)
				{
					possibleMove.nextTetromino = true;
				}
				else if (heldTetrominoCheck)
				{
					possibleMove.heldTetromino = true;
				}
				possibleMoves.Add(possibleMove);
				while (piece.position.y != board.spawnPosition.y) { piece.Move(Vector2Int.up); }; //Reset Y position

			}
			while (piece.Move(Vector2Int.right)); //Break if piece is all the way to the right

			piece.Rotate(1);
			while (piece.Move(Vector2Int.left)) { }
		}
		while (piece.rotationIndex != 0); // Break if piece is same as original rotation
	}



	private GameState GetGameState(Tilemap tilemap, bool newGameState=false)
	{
		var gameState = new GameState();
		RectInt bounds = board.Bounds;

		var row = bounds.yMin;
		var tilesInRow = 0;

		if (newGameState)
		{
			gameState.pieceHeight = piece.GetRowNumber();
		}

		while (row < bounds.yMax) //from bottom row
		{
			var emptyTilesInRow = 0;
			for (int col = bounds.xMin; col < bounds.xMax; col++) //Each Column in row
			{
				
				var position = new Vector3Int(col, row, 0);

				TileBase tile = tilemap.GetTile(position);
				if (piece.TileCheck(tile))//Does tile exist
				{
					gameState.totalTiles++;
				}
				else //If not:
				{

					emptyTilesInRow++;
					var foundTile = false;
					var foundLeftTile = false;
					var foundRightTile = false;

					for (int rowChecked = row+1; rowChecked < bounds.yMax; rowChecked++) //loop over every tile directly above
					{
						Vector3Int tilePosition = new Vector3Int(col, rowChecked, 0);//New tile position
						TileBase tileAbove = tilemap.GetTile(tilePosition);
						var tileCheck = piece.TileCheck(tileAbove);

						if (col - 1 > bounds.xMin)
						{
							if (rowChecked >= row + 2 && !foundLeftTile && !tileCheck)
							{
								Vector3Int leftTilePosition = new Vector3Int(col - 1, rowChecked, 0);
								if (piece.TileCheck(tilemap.GetTile(leftTilePosition)))
								{
									foundLeftTile = true;
								}
							}
						}
						else
						{
							foundLeftTile = true;
						}
						if (foundLeftTile)
						{
							if (col + 1 < bounds.xMax)
							{
								if (rowChecked >= row+2 && !foundRightTile && !tileCheck)
								{
									Vector3Int rightTilePosition = new Vector3Int(col + 1, rowChecked, 0);//New tile position to the right
									if(piece.TileCheck(tilemap.GetTile(rightTilePosition)))
									{
										foundRightTile = true;
									}
								}
							}
                            else
                            {
								foundRightTile = true;
                            }
                        }

						if (col-1 > bounds.xMin && rowChecked >= row+2 && !foundLeftTile && !tileCheck)
						{
						}
						if (piece.TileCheck(tileAbove))// If tile above is not null, count as trapped tile
						{
							foundTile = true;
							break;
						}
					}
					if (foundTile)// If we did find a tile at any point above, count the original one as trapped, if not, its not trapped
					{
						gameState.trappedTileSpaces++;
					}
					else if (foundLeftTile && foundRightTile)
					{
						gameState.longGaps++;
					}
				}
			}
			if (emptyTilesInRow == bounds.xMax - bounds.xMin) //If all of the tiles in this row is empty
			{
				if (row == bounds.yMin)
				{
					gameState.maxHeight = 0;
				}
				else
				{
					gameState.maxHeight = board.GetRowNumber(row-1); //then the row below this was the maximum height
				}
				gameState.tilesInTopRow = tilesInRow;
				break; // stop counting
			}
			else //If not, the tiles in the row is the maximum tiles - the empty ones
			{
				tilesInRow = (bounds.xMax - bounds.xMin) - emptyTilesInRow;
			}
			if (tilesInRow == bounds.xMax - bounds.xMin) // If the number of tiles in the row is maximum
			{
				gameState.completeLines++; //Then a complete line is recorded
			}
			row++;
		}
		return gameState;
	}



	private int CalculateMoveScore()
    {
		//Get current state of main TileMap
		var oldGameState = GetGameState(mainTileMap);

		//Set current piece into proposed position and rotation on TileMap
		for (int i = 0; i < piece.cells.Length; i++)
		{
			Vector3Int tilePosition = piece.cells[i] + piece.position;
			TileBase tile = piece.data.tile;
			mainTileMap.SetTile(tilePosition, tile);
		}

		//Get New state of TileMap
		var newGameState = GetGameState(mainTileMap, true);

		//Remove possible game piece
		board.Clear(piece);

		//Compare old main TileMap state and new TileMap State and add decide score
		var score = 0;

		float pieceExponent = 0.1f + 0.1f * (float)newGameState.pieceHeight;
		int pieceHeightMaxScore = newGameState.pieceHeight <= 10 ? 0 : 20; // Maximum score is 0 if pieceHeight <= 10, otherwise 20
		score += Mathf.RoundToInt(pieceHeightMaxScore - Mathf.Pow((float)newGameState.pieceHeight, pieceExponent));

		float heightExponent = 0.1f + 0.1f * (float)newGameState.maxHeight;
		int heightMaxScore = newGameState.maxHeight <= 10 ? 0 : 20; // Maximum score is 0 if pieceHeight <= 10, otherwise 20
		score += Mathf.RoundToInt(heightMaxScore - Mathf.Pow((float)newGameState.maxHeight, heightExponent));

		score += newGameState.tilesInTopRow > oldGameState.tilesInTopRow ? (int)AiPositionScoreSheet.FlatterTop : 0;
		score += (newGameState.trappedTileSpaces - oldGameState.trappedTileSpaces) * (int)AiPositionScoreSheet.TrappedGapsCreated;
		score += newGameState.completeLines * (int)AiPositionScoreSheet.LineClear;
		score += newGameState.longGaps <=1 ? 0 : newGameState.longGaps * (int)AiPositionScoreSheet.LongGapsCreated;
		//Return score
		return score;
	}

}
