using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseRespond : MonoBehaviour
{
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

    private void OnEnable()
    {
        Events.Instance.HouseClicked += OnHouseClicked;
    }

    private void OnDisable()
    {
        Events.Instance.HouseClicked -= OnHouseClicked;
    }

    private void OnHouseClicked()
    {
        StartCoroutine(ShowIcon());
    }

    private IEnumerator ShowIcon()
    {
        Sprite s = emotionSprites[Random.Range(0, emotionSprites.Count)];
        uiImage.sprite = s;
        uiImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        uiImage.gameObject.SetActive(false);
    }
}