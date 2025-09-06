using UnityEngine;
using UnityEngine.UI;

public class TutorialTester : MonoBehaviour
{
    [Header("Test Tutorial")]
    [SerializeField] private Button testButton;
    [SerializeField] private Button testButton2;
    [SerializeField] private Button testButton3;

    private void Start()
    {
        // Tạo các nút test nếu chưa có
        if (testButton == null)
        {
            CreateTestButtons();
        }
    }

    private void CreateTestButtons()
    {
        // Tạo nút test 1
        GameObject btn1 = new GameObject("Test Button 1");
        btn1.transform.SetParent(transform);
        testButton = btn1.AddComponent<Button>();
        btn1.AddComponent<Image>();
        
        // Tạo nút test 2
        GameObject btn2 = new GameObject("Test Button 2");
        btn2.transform.SetParent(transform);
        testButton2 = btn2.AddComponent<Button>();
        btn2.AddComponent<Image>();
        
        // Tạo nút test 3
        GameObject btn3 = new GameObject("Test Button 3");
        btn3.transform.SetParent(transform);
        testButton3 = btn3.AddComponent<Button>();
        btn3.AddComponent<Image>();
    }

    [ContextMenu("Test Tutorial")]
    public void TestTutorial()
    {
        GameSettings.EnableTutorial = true;
        Debug.Log("🧪 Testing tutorial...");
    }
}
