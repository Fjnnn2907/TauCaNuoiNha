using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationManager : Singleton<NotificationManager>
{
    [Header("UI Elements")]
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;

    private Queue<NotificationData> notificationQueue = new Queue<NotificationData>();
    private bool isShowing = false;
    private string currentMessage = null;

    private void Start()
    {
        notificationPanel.SetActive(false);
    }

    public void ShowNotification(string message, float duration = 2f)
    {
        // Nếu đang hiển thị cùng thông báo thì bỏ qua
        if (isShowing && message == currentMessage)
            return;

        notificationQueue.Enqueue(new NotificationData(message, duration));

        if (!isShowing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isShowing = true;

        while (notificationQueue.Count > 0)
        {
            NotificationData data = notificationQueue.Dequeue();
            currentMessage = data.message;

            notificationText.text = data.message;
            notificationPanel.SetActive(true);

            yield return new WaitForSeconds(data.duration);

            notificationPanel.SetActive(false);
            currentMessage = null;

            yield return new WaitForSeconds(0.2f);
        }

        isShowing = false;
    }

    private struct NotificationData
    {
        public string message;
        public float duration;

        public NotificationData(string message, float duration)
        {
            this.message = message;
            this.duration = duration;
        }
    }
}
