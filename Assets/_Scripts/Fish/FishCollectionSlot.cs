using UnityEngine;
using UnityEngine.UI;

public class FishCollectionSlot : MonoBehaviour
{
    public Image icon;

    private FishData fishData;

    public void Setup(FishData data, bool isDiscovered)
    {
        fishData = data;

        if (isDiscovered)
            icon.sprite = fishData.sprite;
        else
            icon.sprite = fishData.shadowSprite;
    }

    public FishData GetFishData() => fishData;
}
