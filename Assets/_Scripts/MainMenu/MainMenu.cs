using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button btnNewGame;
    [SerializeField] private Button btnContinue;
    [SerializeField] private Button btnQuit;

    [Header("Loading")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject buttonGroup;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Confirm Panel")]
    [SerializeField] private GameObject confirmNewGamePanel; 
    [SerializeField] private Button btnConfirmOK;
    [SerializeField] private Button btnConfirmCancel;

    [Header("Tutorial Panel")]
    [SerializeField] private GameObject askTutorialPanel;     // hỏi Tutorial
    [SerializeField] private Button btnTutorialYes;
    [SerializeField] private Button btnTutorialNo;

    private void Awake()
    {
        askTutorialPanel.SetActive(false);
        confirmNewGamePanel.SetActive(false);
        loadingScreen.SetActive(false);
    }

    private IEnumerator Start()
    {
        // Đợi GameData load xong
        while (SaveManager.Instance.GetGameData() == null)
            yield return null;

        CheckSaveData();

        btnNewGame.onClick.AddListener(OnNewGameClicked);
        btnContinue.onClick.AddListener(OnContinueClicked);
        btnQuit.onClick.AddListener(QuitGame);

        btnConfirmOK.onClick.AddListener(OnConfirmNewGame);
        btnConfirmCancel.onClick.AddListener(() => confirmNewGamePanel.SetActive(false));

        btnTutorialYes.onClick.AddListener(() =>
        {
            GameSettings.EnableTutorial = true;
            askTutorialPanel.SetActive(false);
            StartCoroutine(LoadSceneAsync("CutScene"));
        });

        btnTutorialNo.onClick.AddListener(() =>
        {
            GameSettings.EnableTutorial = false;
            askTutorialPanel.SetActive(false);
            StartCoroutine(LoadSceneAsync("CutScene"));
        });
    }

    private void CheckSaveData()
    {
        var gameData = SaveManager.Instance.GetGameData();
        bool hasValidScene = SaveManager.Instance.HasSaveData() && !string.IsNullOrEmpty(gameData?.currentSceneName);
        btnContinue.gameObject.SetActive(hasValidScene);
    }

    private void OnNewGameClicked()
    {
        var gameData = SaveManager.Instance.GetGameData();
        bool hasValidScene = SaveManager.Instance.HasSaveData() && !string.IsNullOrEmpty(gameData?.currentSceneName);

        if (!hasValidScene)
        {
            SaveManager.Instance.DeleteSave();
            AskTutorial(); // hỏi Tutorial ngay
        }
        else
        {
            confirmNewGamePanel.SetActive(true);
        }
    }

    private void OnConfirmNewGame()
    {
        confirmNewGamePanel.SetActive(false);
        SaveManager.Instance.DeleteSave();
        AskTutorial(); // sau khi confirm thì hỏi Tutorial
    }

    private void AskTutorial()
    {
        askTutorialPanel.SetActive(true);
    }

    private void OnContinueClicked()
    {
        string sceneToLoad = SaveManager.Instance?.GetGameData()?.currentSceneName;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            GameSettings.EnableTutorial = false;
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }
        else
        {
            Debug.LogWarning("❌ Không có scene để tiếp tục!");
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        buttonGroup.SetActive(false);
        loadingScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float progress = 0;
        while (!operation.isDone)
        {
            progress = Mathf.MoveTowards(progress, operation.progress, Time.deltaTime);
            float displayProgress = Mathf.Clamp01(progress / 0.9f);

            progressSlider.value = displayProgress;
            progressText.text = $"Đang tải... {Mathf.RoundToInt(displayProgress * 100)}%";

            if (progress >= 0.9f)
            {
                progressSlider.value = 1f;
                progressText.text = "Đang tải... 100%";
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
