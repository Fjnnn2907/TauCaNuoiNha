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
    public GameObject infoPanel;
    public Button goButton;

    private int currentLocationIndex = -1;

    void Start()
    {
        infoPanel.SetActive(false);

        goButton.onClick.AddListener(GoToLocation);
    }

    public void ShowLocationInfo(int index)
    {
        currentLocationIndex = index;
        LocationData data = locations[index];

        titleText.text = data.locationName;
        descriptionText.text = data.description;
        infoPanel.SetActive(true);
    }

    public void GoToLocation()
    {
        if (currentLocationIndex < 0) return;

        string sceneName = locations[currentLocationIndex].sceneName;
        Debug.Log("Di chuyển đến: " + sceneName);
        _ = SaveManager.Instance.SaveGameAsync();
        SceneManager.LoadScene(sceneName); // Nếu bạn dùng scene
    }
}
