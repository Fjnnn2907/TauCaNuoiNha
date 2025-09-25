using System.Collections.Generic;
using UnityEngine;
using System;

public class ButterflyController : MonoBehaviour
{
    private List<Vector3> path;
    private int currentIndex = 0;
    private Action onPathComplete;

    [Header("Movement")]
    public float speed = 2f;
    public float arrivalThreshold = 0.05f;

    [Header("Rotation")]
    public float rotationOffset = -90f;

    public void SetPath(List<Vector3> newPath, Action onComplete, float newSpeed, float newScale)
    {
        path = newPath;
        currentIndex = 0;
        onPathComplete = onComplete;

        speed = newSpeed;
        transform.localScale = Vector3.one * newScale;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (path == null || path.Count == 0) return;
        if (currentIndex >= path.Count) return;

        Vector3 target = path[currentIndex];

        // Di chuyển
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Xoay theo hướng bay
        Vector2 dir = (target - transform.position).normalized;
        if (dir.sqrMagnitude > 0.0001f)
        {
            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotZ + rotationOffset);
        }

        // Check tới waypoint
        if (Vector3.Distance(transform.position, target) <= arrivalThreshold)
        {
            currentIndex++;
            if (currentIndex >= path.Count)
            {
                gameObject.SetActive(false);
                onPathComplete?.Invoke();
            }
        }
    }
}
