using System.Collections.Generic;
using UnityEngine;

public class PossibleMove
{
	public Vector3Int position { get; set; }
	public int rotationIndex { get; set; }

	public int score { get; set; }

	public bool heldTetromino { get; set; } = false;

	public bool nextTetromino { get; set; } = false;

}