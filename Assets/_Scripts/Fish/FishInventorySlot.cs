using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FishInventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;

    private FishData fishData;
    private int quantity;

    public void Setup(FishData data, int qty)
    {
        fishData = data;
        quantity = qty;

        icon.sprite = data.sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FishDetailPanel.Instance.ShowFishInfo(fishData, quantity);
    }
}
