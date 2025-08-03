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

    private bool hasSaveData;

    private void Start()
    {
        hasSaveData = SaveManager.Instance.HasSaveData();

        btnContinue.gameObject.SetActive(hasSaveData);

        btnNewGame.onClick.AddListener(OnNewGameClicked);
        btnContinue.onClick.AddListener(OnContinueClicked);
        btnQuit.onClick.AddListener(QuitGame);

        btnConfirmOK.onClick.AddListener(OnConfirmNewGame);
        btnConfirmCancel.onClick.AddListener(() => confirmNewGamePanel.SetActive(false));
    }

    private void OnNewGameClicked()
    {
        if (!hasSaveData)
        {
            Debug.Log("🆕 Lần đầu chơi -> Bắt đầu luôn không hỏi");
            SaveManager.Instance.DeleteSave();
            StartCoroutine(LoadSceneAsync("Audition"));
        }
        else
        {
            Debug.Log("⚠️ Đã từng chơi -> Hiện hộp thoại xác nhận");
            confirmNewGamePanel.SetActive(true);
        }
    }

    private void OnConfirmNewGame()
    {
        confirmNewGamePanel.SetActive(false);
        SaveManager.Instance.DeleteSave();
        StartCoroutine(LoadSceneAsync("Audition"));
    }

    private void OnContinueClicked()
    {
        Debug.Log("▶️ Chơi tiếp");
        StartCoroutine(LoadSceneAsync(SaveManager.Instance?.GetGameData().currentSceneName));
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
