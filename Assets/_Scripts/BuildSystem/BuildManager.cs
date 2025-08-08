using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public GameObject[] buildPrefabs;

    private FurnitureItem currentGhost;
    private bool isPlacing = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (isPlacing && currentGhost != null)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            Vector2Int gridPos = GridManager.Instance.WorldToGrid(mouseWorld);
            Vector3 snapPos = GridManager.Instance.GridToWorld(gridPos);

            // Mượt mà hơn
            currentGhost.transform.position = Vector3.Lerp(currentGhost.transform.position, snapPos, Time.deltaTime * 20f);

            if (GridManager.Instance.CanPlace(gridPos))
                currentGhost.SetValid();
            else
                currentGhost.SetInvalid();

            if (Input.GetMouseButtonDown(0))
            {
                if (GridManager.Instance.CanPlace(gridPos))
                {
                    PlaceObject(gridPos);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                CancelBuild();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0f;

                RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);
                if (hit.collider != null)
                {
                    FurnitureItem item = hit.collider.GetComponent<FurnitureItem>();
                    if (item != null && !item.IsBeingPlaced)
                    {
                        // Bắt đầu di chuyển lại
                        GridManager.Instance.ClearGrid(item.GridPosition);
                        currentGhost = item;
                        isPlacing = true;
                        item.IsBeingPlaced = true;
                        item.highlightRenderer.color = new Color(1, 1, 1, 0.3f);
                    }
                }
            }
        }
    }

    public void StartPlacing(int index)
    {
        CancelBuild();

        GameObject prefab = buildPrefabs[index];
        GameObject ghost = Instantiate(prefab);
        currentGhost = ghost.GetComponent<FurnitureItem>();
        currentGhost.IsBeingPlaced = true;
        currentGhost.highlightRenderer.color = new Color(1, 1, 1, 0.3f);

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector2Int gridPos = GridManager.Instance.WorldToGrid(mouseWorld);
        Vector3 snapPos = GridManager.Instance.GridToWorld(gridPos);
        currentGhost.transform.position = snapPos;

        isPlacing = true;
    }

    private void PlaceObject(Vector2Int gridPos)
    {
        GridManager.Instance.OccupyGrid(gridPos, currentGhost);
        currentGhost.SetNormal();
        currentGhost.GridPosition = gridPos;
        currentGhost.IsBeingPlaced = false;
        currentGhost = null;
        isPlacing = false;
    }

    public void CancelBuild()
    {
        if (currentGhost != null && currentGhost.IsBeingPlaced)
        {
            Destroy(currentGhost.gameObject);
            currentGhost = null;
        }
        isPlacing = false;
    }
}
