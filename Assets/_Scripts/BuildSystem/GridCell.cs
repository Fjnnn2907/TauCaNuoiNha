using UnityEngine;

public class GridCell
{
    public Vector2Int position;
    public GameObject occupyingObject;

    public GridCell(Vector2Int pos)
    {
        position = pos;
    }

    public bool IsOccupied => occupyingObject != null;
}
