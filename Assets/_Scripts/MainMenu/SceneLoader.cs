using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI progressText;

    private Button targetButton;
    private string targetSceneName;

    public void AssignButton(Button button, string sceneName)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => LoadSceneWithLoading(sceneName));
    }

    public void LoadSceneWithLoading(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingScreen != null) loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float progress = 0f;
        while (!operation.isDone)
        {
            progress = Mathf.MoveTowards(progress, operation.progress, Time.deltaTime);
            float displayProgress = Mathf.Clamp01(progress / 0.9f);

            if (progressSlider != null)
                progressSlider.value = displayProgress;

            string loadingText = LanguageManager.Instance.GetText("dang_tai");

            if (progressText != null)
                progressText.text = $"{loadingText}... {Mathf.RoundToInt(displayProgress * 100)}%";

            if (progress >= 0.9f)
            {
                if (progressSlider != null) progressSlider.value = 1f;
                if (progressText != null) progressText.text = $"{loadingText}... 100%";
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
