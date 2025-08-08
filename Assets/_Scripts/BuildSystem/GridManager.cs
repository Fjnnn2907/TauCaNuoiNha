using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public float cellSize = 1f;

    private Dictionary<Vector2Int, FurnitureItem> grid = new Dictionary<Vector2Int, FurnitureItem>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize + cellSize / 2, gridPos.y * cellSize + cellSize / 2, 0);
    }

    public bool CanPlace(Vector2Int gridPos)
    {
        return !grid.ContainsKey(gridPos);
    }

    public void OccupyGrid(Vector2Int gridPos, FurnitureItem item)
    {
        grid[gridPos] = item;
    }

    public void ClearGrid(Vector2Int gridPos)
    {
        if (grid.ContainsKey(gridPos))
            grid.Remove(gridPos);
    }
}
