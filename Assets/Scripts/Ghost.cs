using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    // Start is called before the first frame update
    public Tile tile;
    public Board board;
    public Piece trackingPiece;
    public Tilemap tilemap {  get; private set; }
	public Vector3Int[] cells { get; private set; }
	public Vector3Int position { get; private set; }

	private void Awake()
	{
		tilemap = GetComponentInChildren<Tilemap>();
		cells = new Vector3Int[4];
	}

	private void LateUpdate()
	{
		Clear();
		Copy();
		Drop();
		Set();
	}

	private void Clear()
	{
		for (int i = 0; i < cells.Length; i++)
		{
			Vector3Int tilePosition = cells[i] + position;
			tilemap.SetTile(tilePosition, null);
		}
	}

	private void Copy()
	{
		for (int i = 0; i < cells.Length; i++)
		{
			cells[i] = trackingPiece.cells[i];
		}
	}

	private void Drop()
	{
		board.Clear(trackingPiece);
		Vector3Int position = trackingPiece.position;
		var bounds = board.Bounds;
		var row = trackingPiece.position.y;

		while (row >= bounds.yMin)
		{
			position.y = row;

			var validData = board.IsValidPosition(trackingPiece, position);
			if (!validData.colliding && !validData.outOfBounds)
			{
				this.position = position;
			}
			else
			{
				break;
			}
			row--;
		}
		board.Set(trackingPiece);
	}

	private void Set()
	{
		for (int i = 0; i < cells.Length; i++)
		{
			Vector3Int tilePosition = cells[i] + position;
			tilemap.SetTile(tilePosition, tile);
		}
	}
}
