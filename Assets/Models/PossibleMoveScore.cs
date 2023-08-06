using System.Collections.Generic;

public class PossibleMoveScore
{
    public int maxHeight { get; set; }
	public int totalTiles { get; set; }
	public int tilesInTopRow { get; set; }
	public int trappedTileSpaces { get; set; } = 0;

	public int completeLines { get; set; } = 0;

}
