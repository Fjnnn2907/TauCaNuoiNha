using UnityEngine;

public class FurnitureItem : MonoBehaviour
{
    public SpriteRenderer highlightRenderer;
    public Vector2Int GridPosition { get; set; }
    public bool IsBeingPlaced { get; set; } = false;

    public void SetValid()
    {
        highlightRenderer.color = new Color(0, 1, 0, 0.3f); // xanh
    }

    public void SetInvalid()
    {
        highlightRenderer.color = new Color(1, 0, 0, 0.3f); // đỏ
    }

    public void SetNormal()
    {
        highlightRenderer.color = Color.white; // về bình thường
    }
}
