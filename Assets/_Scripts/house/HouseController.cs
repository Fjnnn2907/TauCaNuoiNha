using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseController : MonoBehaviour
{
    [SerializeField] private string houseID;
    [SerializeField] private List<Sprite> emotionSprites;
    private Image uiImage;

    private void Start()
    {
        uiImage = GetComponentInChildren<Image>();
        if (uiImage == null)
        {
            Debug.LogError("Image component not found on the GameObject.");
        }
        uiImage.gameObject.SetActive(false);
    }

    private IEnumerator ShowIcon()
    {
        Sprite s = emotionSprites[Random.Range(0, emotionSprites.Count)];
        uiImage.sprite = s;
        uiImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        uiImage.gameObject.SetActive(false);
    }

    public void OnClicked()
    {
        Debug.Log($"House {houseID} được click!");
        // Xử lý logic riêng cho house này
        // Ví dụ: mở UI, chuyển scene, etc.
        StartCoroutine(ShowIcon());
    }
}
