using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : Singleton<MapManager>
{
    [System.Serializable]
    public class LocationData
    {
        public string nameKey;           // 🔑 key đa ngôn ngữ (vd: bac, trung, nam)
        public string descriptionKey;    // 🔑 key mô tả
        public string sceneName;
        public int regionIndex; // Bắc = 0, Trung = 1, Nam = 2
    }

    public LocationData[] locations;

    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI currentLocationText;
    public GameObject infoPanel;
    public Button goButton;
    public TextMeshProUGUI goButtonText;

    [Header("Transport UI")]
    public GameObject transportPanel;
    public TextMeshProUGUI bikeButtonText;
    public TextMeshProUGUI planeButtonText;
    public Button bikeButton;
    public Button planeButton;

    [Header("Travel Cost")]
    public int costPerDistanceBike = 100;
    public int costPerDistancePlane = 300;

    private int currentLocationIndex = -1;
    private string currentSceneName;

    private void Start()
    {
        infoPanel.SetActive(false);
        goButton.onClick.AddListener(OnGoButtonClick);

        currentSceneName = SceneManager.GetActiveScene().name;

        // Hiển thị tên khu vực hiện tại
        foreach (var loc in locations)
        {
            if (loc.sceneName == currentSceneName)
            {
                string locName = LanguageManager.Instance.GetText(loc.nameKey);
                string prefix = LanguageManager.Instance.GetText("ban_dang_o"); // "Bạn đang ở:" / "You are at:"
                currentLocationText.text = $"{prefix} {locName}";
                break;
            }
        }
    }

    public void ShowLocationInfo(int index)
    {
        currentLocationIndex = index;
        LocationData data = locations[index];

        // 🔑 Lấy text theo ngôn ngữ
        titleText.text = LanguageManager.Instance.GetText(data.nameKey);
        descriptionText.text = LanguageManager.Instance.GetText(data.descriptionKey);
        infoPanel.SetActive(true);

        bool isCurrentLocation = (data.sceneName == currentSceneName);
        goButton.interactable = !isCurrentLocation;
        goButtonText.text = isCurrentLocation
            ? LanguageManager.Instance.GetText("da_toi")  // "Đã tới" / "Arrived"
            : LanguageManager.Instance.GetText("di_toi"); // "Đi tới" / "Go To"

        // Nếu đang mở transportPanel thì reset
        if (transportPanel.activeSelf)
        {
            transportPanel.SetActive(false);
            goButtonText.text = LanguageManager.Instance.GetText("di_toi");
        }
    }

    public void OnGoButtonClick()
    {
        if (currentLocationIndex < 0) return;

        var target = locations[currentLocationIndex];
        bool isCurrentLocation = (target.sceneName == currentSceneName);

        if (isCurrentLocation) return;

        // Nếu panel đang bật → tắt
        if (transportPanel.activeSelf)
        {
            transportPanel.SetActive(false);
            goButtonText.text = LanguageManager.Instance.GetText("di_toi");
            return;
        }

        // Hiển thị UI chọn phương tiện
        transportPanel.SetActive(true);
        goButtonText.text = LanguageManager.Instance.GetText("dong"); // "Đóng" / "Close"

        var current = GetCurrentLocationData();
        int bikeCost = CalculateCost(current.regionIndex, target.regionIndex, "Bike");
        int planeCost = CalculateCost(current.regionIndex, target.regionIndex, "Plane");

        // 🔑 Nút xe máy + giá
        //bikeButtonText.text = $"{LanguageManager.Instance.GetText("bike")} ({bikeCost})";
        //planeButtonText.text = $"{LanguageManager.Instance.GetText("plane")} ({planeCost})";

        bikeButtonText.text = $"{bikeCost}";
        planeButtonText.text = $"{planeCost}";

        bikeButton.onClick.RemoveAllListeners();
        planeButton.onClick.RemoveAllListeners();

        bikeButton.onClick.AddListener(() => ConfirmTravel("Bike", bikeCost));
        planeButton.onClick.AddListener(() => ConfirmTravel("Plane", planeCost));
    }

    private LocationData GetCurrentLocationData()
    {
        foreach (var loc in locations)
        {
            if (loc.sceneName == currentSceneName)
                return loc;
        }
        return null;
    }

    private int CalculateDistance(int from, int to)
    {
        return Mathf.Abs(from - to);
    }

    private int CalculateCost(int from, int to, string transport)
    {
        int distance = CalculateDistance(from, to);
        int costPerDistance = (transport == "Plane") ? costPerDistancePlane : costPerDistanceBike;
        return distance * costPerDistance;
    }

    private async void ConfirmTravel(string transport, int cost)
    {
        if (!CoinManager.Instance.SpendCoins(cost))
        {
            Debug.Log("❌ Không đủ tiền để di chuyển!");
            transportPanel.SetActive(false);
            goButtonText.text = LanguageManager.Instance.GetText("di_toi");
            return;
        }

        GameData data = SaveManager.Instance.GetGameData();
        data.targetSceneName = locations[currentLocationIndex].sceneName;

        await SaveManager.Instance.SaveGameAsync();

        transportPanel.SetActive(false);

        if (transport == "Bike")
        {
            SceneManager.LoadScene("BikeTravelScene"); // chuyển sang scene trung gian
        }
        else
        {
            SceneManager.LoadScene(locations[currentLocationIndex].sceneName);
        }
    }

    private void OnApplicationQuit()
    {
        SaveCurrentSceneName();
    }

    private void SaveCurrentSceneName()
    {
        GameData data = SaveManager.Instance.GetGameData();
        data.currentSceneName = SceneManager.GetActiveScene().name;
    }
}
