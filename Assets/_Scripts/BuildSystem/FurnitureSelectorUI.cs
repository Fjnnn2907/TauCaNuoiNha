using UnityEngine;
using UnityEngine.UI;

public class FurnitureSelectorUI : MonoBehaviour
{
    public string itemID;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            BuildManager.Instance.StartPlacing(itemID);
        });
    }
}
