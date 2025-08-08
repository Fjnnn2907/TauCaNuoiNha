using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("Grid Settings")]
    public int gridWidth = 20;
    public int gridHeight = 20;
    public float cellSize = 1f;
    private Dictionary<Vector2Int, GridCell> grid = new();

    [Header("Prefabs")]
    public List<GameObject> furniturePrefabs;

    [Header("Current Building")]
    private GameObject currentGhost;
    private FurnitureItem currentItem;
    private bool isBuilding = false;

    private void Awake()
    {
        Instance = this;
        InitGrid();
    }

    void InitGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var pos = new Vector2Int(x, y);
                grid[pos] = new GridCell(pos);
            }
        }
    }

    void Update()
    {
        if (isBuilding && currentGhost != null)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = WorldToGrid(mouseWorldPos);

            currentGhost.transform.position = GridToWorld(gridPos);

            bool canPlace = CanPlaceAt(gridPos, currentItem.size);
            SetGhostColor(canPlace ? Color.green : Color.red);

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceFurniture(gridPos);
            }
            if (Input.GetMouseButtonDown(1)) // Chuột phải
            {
                CancelBuildMode();
            }
        }
    }

    public void StartPlacing(string itemID)
    {
        GameObject prefab = furniturePrefabs.Find(p => p.GetComponent<FurnitureItem>().itemID == itemID);
        if (prefab == null)
        {
            Debug.LogError("Item not found: " + itemID);
            return;
        }

        if (currentGhost) Destroy(currentGhost);

        currentGhost = Instantiate(prefab);
        currentItem = currentGhost.GetComponent<FurnitureItem>();
        currentGhost.GetComponent<Collider2D>().enabled = false;
        SetGhostAlpha(0.5f);
        isBuilding = true;
    }
    public void CancelBuildMode()
    {
        if (currentGhost != null)
        {
            Destroy(currentGhost);
            currentGhost = null;
        }

        currentItem = null;
        isBuilding = false;
    }


    void PlaceFurniture(Vector2Int pos)
    {
        GameObject placed = Instantiate(currentGhost);
        placed.GetComponent<SpriteRenderer>().color = Color.white;
        placed.GetComponent<Collider2D>().enabled = true;
        placed.transform.position = GridToWorld(pos);

        // Đánh dấu các ô bị chiếm
        for (int x = 0; x < currentItem.size.x; x++)
        {
            for (int y = 0; y < currentItem.size.y; y++)
            {
                Vector2Int offset = (pos + new Vector2Int(x, y));
                if (grid.ContainsKey(offset))
                {
                    grid[offset].occupyingObject = placed;
                }
            }
        }

        Destroy(currentGhost);
        currentGhost = null;
        isBuilding = false;
    }

    void SetGhostAlpha(float a)
    {
        var sr = currentGhost.GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = a;
        sr.color = c;
    }

    void SetGhostColor(Color color)
    {
        var sr = currentGhost.GetComponent<SpriteRenderer>();
        sr.color = color;
    }

    Vector2Int WorldToGrid(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0);
    }

    bool CanPlaceAt(Vector2Int pos, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int checkPos = pos + new Vector2Int(x, y);
                if (!grid.ContainsKey(checkPos)) return false;
                if (grid[checkPos].IsOccupied) return false;
            }
        }
        return true;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, 0);
            Vector3 end = new Vector3(x * cellSize, gridHeight * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = new Vector3(0, y * cellSize, 0);
            Vector3 end = new Vector3(gridWidth * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }
#endif
}
