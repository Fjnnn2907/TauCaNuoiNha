using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BikeTravelController : MonoBehaviour
{
    public Transform bikeTransform;
    public float speed = 3f;
    public Transform endPoint;
    public GameObject BienCoPanel;

    private int bienCoToXayRa = 0;
    private List<float> bienCoPositions = new();
    private bool isStoppedForBienCo = false;
    private bool hasReachedEnd = false;

    private void Start()
    {
        // Tạo danh sách ngẫu nhiên 2–4 vị trí xảy ra biến cố dọc đường
        bienCoToXayRa = Random.Range(2, 5);
        float totalDistance = endPoint.position.x - bikeTransform.position.x;
        for (int i = 0; i < bienCoToXayRa; i++)
        {
            float randomOffset = Random.Range(0.2f, 0.8f) * totalDistance;
            bienCoPositions.Add(bikeTransform.position.x + randomOffset);
        }

        bienCoPositions.Sort();
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (isStoppedForBienCo || hasReachedEnd) return;

        // Di chuyển xe
        bikeTransform.Translate(Vector3.right * speed * Time.deltaTime);

        // Kiểm tra biến cố
        if (bienCoPositions.Count > 0 && bikeTransform.position.x >= bienCoPositions[0])
        {
            bienCoPositions.RemoveAt(0);
            StartCoroutine(TriggerBienCo());
        }

        // Kiểm tra đến điểm cuối
        if (bikeTransform.position.x >= endPoint.position.x)
        {
            hasReachedEnd = true; // Ngăn việc gọi nhiều lần
            StartCoroutine(SaveAndLoadNextScene());
        }
    }

    private IEnumerator TriggerBienCo()
    {
        isStoppedForBienCo = true;
        BienCoManager.Instance.KichHoatBienCoNgauNhien();

        // Đợi người chơi xác nhận biến cố
        while (BienCoPanel.activeSelf)
        {
            yield return null;
        }

        isStoppedForBienCo = false;
    }

    private IEnumerator SaveAndLoadNextScene()
    {
        Debug.Log("💾 Đang lưu dữ liệu trước khi chuyển scene...");
        var saveTask = SaveManager.Instance.SaveGameAsync();

        while (!saveTask.IsCompleted)
        {
            yield return null;
        }

        Debug.Log("✅ Đã lưu xong. Đang chuyển scene...");

        string targetScene = SaveManager.Instance.GetGameData().targetSceneName;
        SceneManager.LoadScene(targetScene);
    }
}
