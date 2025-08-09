using System.Collections;
using UnityEngine;

public class NPCController : Singleton<NPCController>
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    public IEnumerator MoveToPointB()
    {
        transform.position = pointA.position;

        while (Vector3.Distance(transform.position, pointB.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                pointB.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    public void EndGame()
    {
        transform.position = pointA.position;
    }
}
