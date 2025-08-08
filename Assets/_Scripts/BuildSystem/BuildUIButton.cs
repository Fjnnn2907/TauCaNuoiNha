using UnityEngine;
using UnityEngine.UI;

public class BuildUIButton : MonoBehaviour
{
    public int prefabIndex;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => BuildManager.Instance.StartPlacing(prefabIndex));
    }
}
