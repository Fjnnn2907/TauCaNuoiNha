using System.Collections.Generic;
using UnityEngine;

public class BienCoManager : Singleton<BienCoManager>
{
    [Header("Cài đặt biến cố")]
    public List<BienCoSO> danhSachBienCo;
    public float minDelay = 30f;
    public float maxDelay = 120f;
    private float timer;

    [Header("Tham chiếu hệ thống")]
    public BienCoUI bienCoUI;

    public List<(FishingBaitData bait, int quantity)> lastLostBaits = new();
    public List<(FishData fish, int quantity)> lastAffectedFish = new();
    public List<(FishingBaitData bait, int quantity)> lastAddedBaits = new();

    public int lastCoinChange = 0;
    public int lastGoldEarnedFromFish = 0;
    public float currentRandomFactor = 1f;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (FishingManager.Instance.isPlayMiniGame || GameSettings.EnableTutorial)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            KichHoatBienCoNgauNhien();
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        timer = Random.Range(minDelay, maxDelay);
    }

    public void KichHoatBienCoNgauNhien()
    {
        var danhSachHopLe = new List<BienCoSO>();

        foreach (var bienCo in danhSachBienCo)
        {
            if (BienCoLogic.IsValid(bienCo) && Random.value <= bienCo.xacSuatXuatHien)
            {
                danhSachHopLe.Add(bienCo);
            }
        }

        if (danhSachHopLe.Count == 0)
        {
            Debug.Log("Không có biến cố nào hợp lệ lúc này.");
            return;
        }

        BienCoSO bienCoChon = danhSachHopLe[Random.Range(0, danhSachHopLe.Count)];

        lastLostBaits.Clear();
        lastAffectedFish.Clear();
        lastAddedBaits.Clear();
        lastCoinChange = 0;

        BienCoLogic.PrepareBienCoData(bienCoChon);
        bienCoUI.ShowBienCo(bienCoChon);
    }


    public void XuLyBienCo(BienCoSO bienCo)
    {
        BienCoLogic.XuLyBienCo(bienCo);
    }
}
