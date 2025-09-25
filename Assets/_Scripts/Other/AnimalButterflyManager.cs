using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalButterflyManager : MonoBehaviour
{
    [Header("Butterfly Settings")]
    public List<GameObject> butterflyPrefabs;
    public int minSpawnCount = 1;
    public int maxSpawnCount = 2;

    [Header("Spawn Area")]
    public Collider2D spawnArea;

    [Header("Path Settings")]
    public int minMidPoints = 2;
    public int maxMidPoints = 3;
    public int smoothness = 15;

    [Header("Randomize Settings")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 3.5f;
    public float minScale = 0.8f;
    public float maxScale = 1.3f;

    [Header("Delay Settings")]
    public float minDelay = 1f;
    public float maxDelay = 3f;

    private List<List<Vector3>> activePaths = new List<List<Vector3>>();
    private int activeButterflies = 0;

    private void Start()
    {
        foreach (var b in butterflyPrefabs)
            b.SetActive(false);

        // Lần đầu cũng delay
        float firstDelay = Random.Range(minDelay, maxDelay);
        StartCoroutine(SpawnAfterDelay(firstDelay));
    }

    private void SpawnBatch()
    {
        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            if (SpawnButterfly())
                activeButterflies++;
        }
    }

    private bool SpawnButterfly()
    {
        if (butterflyPrefabs.Count == 0 || spawnArea == null) return false;

        // Lấy danh sách prefab chưa active
        List<GameObject> inactiveList = butterflyPrefabs.FindAll(b => !b.activeInHierarchy);
        if (inactiveList.Count == 0) return false;

        // Chọn ngẫu nhiên 1 prefab trong số chưa active
        GameObject butterfly = inactiveList[Random.Range(0, inactiveList.Count)];

        Vector3 startPos = GetRandomPointOnEdge(spawnArea);
        butterfly.transform.position = startPos;

        List<Vector3> waypoints = GenerateWaypoints(startPos);
        List<Vector3> smoothPath = GenerateSmoothPath(waypoints, smoothness);
        activePaths.Add(smoothPath);

        float randSpeed = Random.Range(minSpeed, maxSpeed);
        float randScale = Random.Range(minScale, maxScale);

        ButterflyController controller = butterfly.GetComponent<ButterflyController>();
        controller.SetPath(smoothPath, () =>
        {
            activePaths.Remove(smoothPath);
            activeButterflies--;

            if (activeButterflies <= 0)
            {
                float delay = Random.Range(minDelay, maxDelay);
                StartCoroutine(SpawnAfterDelay(delay));
            }
        }, randSpeed, randScale);

        butterfly.SetActive(true); // đừng quên bật nó lên

        return true;
    }


    private IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnBatch();
    }

    private Vector3 GetRandomPointOnEdge(Collider2D col)
    {
        Bounds bounds = col.bounds;
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: return new Vector3(bounds.min.x, Random.Range(bounds.min.y, bounds.max.y), 0);
            case 1: return new Vector3(bounds.max.x, Random.Range(bounds.min.y, bounds.max.y), 0);
            case 2: return new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.min.y, 0);
            default: return new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y, 0);
        }
    }

    private List<Vector3> GenerateWaypoints(Vector3 start)
    {
        List<Vector3> points = new List<Vector3>();
        Bounds bounds = spawnArea.bounds;

        points.Add(start);

        int midPointCount = Random.Range(minMidPoints, maxMidPoints + 1);
        for (int i = 0; i < midPointCount; i++)
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            points.Add(new Vector3(x, y, 0));
        }

        points.Add(GetRandomPointOnEdge(spawnArea));
        return points;
    }

    private List<Vector3> GenerateSmoothPath(List<Vector3> waypoints, int smoothness)
    {
        List<Vector3> smoothPath = new List<Vector3>();

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Vector3 p0 = i > 0 ? waypoints[i - 1] : waypoints[i];
            Vector3 p1 = waypoints[i];
            Vector3 p2 = waypoints[i + 1];
            Vector3 p3 = (i + 2 < waypoints.Count) ? waypoints[i + 2] : p2;

            for (int j = 0; j < smoothness; j++)
            {
                float t = j / (float)smoothness;
                Vector3 pos = 0.5f *
                    ((2f * p1) +
                    (-p0 + p2) * t +
                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * (t * t) +
                    (-p0 + 3f * p1 - 3f * p2 + p3) * (t * t * t));
                smoothPath.Add(pos);
            }
        }

        smoothPath.Add(waypoints[waypoints.Count - 1]);
        return smoothPath;
    }

    private void OnDrawGizmos()
    {
        if (spawnArea != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(spawnArea.bounds.center, spawnArea.bounds.size);
        }

        Gizmos.color = Color.cyan;
        foreach (var path in activePaths)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }
}
