using System.Collections.Generic;

public class GameState
{
    public int maxHeight { get; set; }
	public int totalTiles { get; set; } = 0;
	public int tilesInTopRow { get; set; }
	public int trappedTileSpaces { get; set; } = 0;

	public int completeLines { get; set; } = 0;

	public int? pieceHeight { get; set; } = null;

	public int longGaps { get; set; } = 0;

}
