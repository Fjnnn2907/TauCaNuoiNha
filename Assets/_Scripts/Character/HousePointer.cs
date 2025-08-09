using UnityEngine;

public class HousePointer : MonoBehaviour
{
	[SerializeField] private LayerMask factoryLayer;
    [SerializeField] private LayerMask houseLayer;

	void Update()
	{
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, factoryLayer);
            RaycastHit2D hit2 = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, houseLayer);
            if (hit.collider != null)
            {
                // Lấy đối tượng được nhấp chuột
                Debug.Log("May click vao factory");
            }
            if (hit2.collider != null)
            {
                // Chỉ xử lý object cụ thể được click
                GameObject clickedHouse = hit2.collider.gameObject;
                Debug.Log("May click vao house: " + clickedHouse.name);
                
                // Tìm component HouseController trên object được click
                var houseController = clickedHouse.GetComponent("HouseController");
                if (houseController != null)
                {
                    // Gọi method OnClicked() bằng reflection
                    houseController.SendMessage("OnClicked", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    Debug.Log("Không tìm thấy HouseController trên " + clickedHouse.name);
                    // Fallback - chỉ xử lý object này thôi, không trigger event global
                }
            }
		}
	}
}