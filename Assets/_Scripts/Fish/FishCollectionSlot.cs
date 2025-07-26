using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishCollectionSlot : MonoBehaviour
{
    public Image icon;
    //public TextMeshProUGUI nameText;

    private FishData fishData;

    public void Setup(FishData data, bool isDiscovered)
    {
        fishData = data;

        if (isDiscovered)
        {
            icon.sprite = fishData.sprite;
            //nameText.text = fishData.fishName;
        }
        else
        {
            icon.sprite = fishData.shadowSprite;
            //nameText.text = "???";
        }
    }

    public FishData GetFishData() => fishData;
}
