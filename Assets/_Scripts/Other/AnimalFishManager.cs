using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFishManager : MonoBehaviour
{
    [Header("Fish Settings")]
    public List<GameObject> fishObjects;
    public float minDelay = 2f;
    public float maxDelay = 5f;

    [Header("Spawn Area")]
    public Collider2D spawnArea; 

    [Header("Forbidden Area")]
    public Collider2D forbiddenArea;

    private void Start()
    {
        StartCoroutine(FishJumpRoutine());
    }

    private IEnumerator FishJumpRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            int index = Random.Range(0, fishObjects.Count);
            GameObject fish = fishObjects[index];

            if (!fish.activeInHierarchy)
            {
                Vector2 spawnPos = GetValidSpawnPoint();

                fish.SetActive(true);
                fish.transform.position = spawnPos;

                float randomScale = Random.Range(1f, 1.4f);

                int dir = Random.value < 0.5f ? 1 : -1;
                fish.transform.localScale = new Vector3(randomScale * dir, randomScale, randomScale);
            }
        }
    }

    private Vector2 GetValidSpawnPoint()
    {
        if (spawnArea == null) return Vector2.zero;

        Bounds bounds = spawnArea.bounds;
        Vector2 spawnPos;
        int attempts = 0;

        do
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            spawnPos = new Vector2(x, y);

            attempts++;
            if (attempts > 30) break;
        }
        while (forbiddenArea != null && forbiddenArea.OverlapPoint(spawnPos));

        return spawnPos;
    }
}
