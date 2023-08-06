using System.Collections.Generic;

public class GameState
{
	public int trappedTileSpaces { get; set; } = 0;

	public int completeLines { get; set; } = 0;

	public int? pieceHeight { get; set; } = null;

	public int longGaps { get; set; } = 0;

	public int? totalHeight { get; set; } = null;
}
