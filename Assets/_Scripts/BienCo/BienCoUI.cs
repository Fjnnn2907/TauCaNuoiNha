using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BienCoUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI tenBienCoText;
    public TextMeshProUGUI moTaText;
    public Image iconImage;

    private BienCoSO bienCoDangHien;

    public void ShowBienCo(BienCoSO bienCo)
    {
        bienCoDangHien = bienCo;

        tenBienCoText.text = bienCo.tenBienCo;
        moTaText.text = bienCo.moTaBienCo;
        iconImage.sprite = bienCo.iconBienCo;

        panel.SetActive(true);
    }

    public void OnAcceptBienCo()
    {
        BienCoManager.Instance.XuLyBienCo(bienCoDangHien);
        Close();
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}
