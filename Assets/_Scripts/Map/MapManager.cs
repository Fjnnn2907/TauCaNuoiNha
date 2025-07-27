using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [System.Serializable]
    public class LocationData
    {
        public string locationName;
        [TextArea]
        public string description;
        public string sceneName; // Nếu bạn muốn load scene
    }

    public LocationData[] locations;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI currentLocationText; // ✅ Thêm text hiện khu vực hiện tại
    public GameObject infoPanel;
    public Button goButton;
    public TextMeshProUGUI goButtonText; // ✅ Thêm text để thay đổi dòng chữ nút

    private int currentLocationIndex = -1;

    private string currentSceneName;

    void Start()
    {
        infoPanel.SetActive(false);
        goButton.onClick.AddListener(GoToLocation);

        // ✅ Lấy scene hiện tại
        currentSceneName = SceneManager.GetActiveScene().name;

        if (currentLocationText != null)
        {
            // Tìm xem scene hiện tại tương ứng khu vực nào
            foreach (var loc in locations)
            {
                if (loc.sceneName == currentSceneName)
                {
                    currentLocationText.text = "Bạn đang ở: " + loc.locationName;
                    break;
                }
            }
        }
    }

    public void ShowLocationInfo(int index)
    {
        currentLocationIndex = index;
        LocationData data = locations[index];

        titleText.text = data.locationName;
        descriptionText.text = data.description;
        infoPanel.SetActive(true);

        // ✅ Nếu đang ở scene này
        if (data.sceneName == currentSceneName)
        {
            goButton.interactable = false;
            goButtonText.text = "Đã tới";
        }
        else
        {
            goButton.interactable = true;
            goButtonText.text = "Đi tới";
        }
    }

    public async void GoToLocation()
    {
        if (currentLocationIndex < 0) return;

        string sceneName = locations[currentLocationIndex].sceneName;
        Debug.Log("Di chuyển đến: " + sceneName);

        GameData data = SaveManager.Instance.GetGameData();
        data.currentSceneName = sceneName;

        await SaveManager.Instance.SaveGameAsync();
        SceneManager.LoadScene(sceneName);
    }
}
