using System.Collections;
using UnityEngine;
using TMPro;

public class NotificationManager : Singleton<NotificationManager>
{
    [Header("UI Elements")]
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;

    private Coroutine hideCoroutine;

    private void Start()
    {
        notificationPanel.SetActive(false);
    }
    public void ShowNotification(string message, float duration = 2)
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        notificationText.text = message;
        notificationPanel.SetActive(true);
        hideCoroutine = StartCoroutine(HideAfterSeconds(duration));
    }

    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        notificationPanel.SetActive(false);
    }
}
