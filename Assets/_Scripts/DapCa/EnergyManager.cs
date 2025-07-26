using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergyManager : MonoBehaviour
{
    public float energy = 0f;
    public float maxEnergy = 100f;
    public float decayRate = 5f;
    public float gainAmount = 10f;

    [Header("Sliders")]
    public Slider energySlider;
    public Slider timeSlider;

    [Header("Minigame Time")]
    public float timeLimit = 10f;
    private float currentTimer;

    private bool hasEnded = false;

    private void Start()
    {
        energySlider.maxValue = maxEnergy;
        energySlider.value = energy;

        timeSlider.maxValue = timeLimit;
        timeSlider.value = timeLimit;
    }

    private void OnEnable()
    {
        hasEnded = false;
        energy = 10f;

        currentTimer = timeLimit;
        energySlider.value = energy;

        timeSlider.maxValue = timeLimit;
        timeSlider.value = timeLimit;
    }

    void Update()
    {
        if (hasEnded) return;

        // Thời gian giảm
        currentTimer -= Time.deltaTime;
        timeSlider.value = currentTimer;

        // Thua nếu hết thời gian
        if (currentTimer <= 0f)
        {
            LoseGame();
            return;
        }

        // Năng lượng giảm dần
        energy -= decayRate * Time.deltaTime;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
        energySlider.value = energy;

        // Thắng nếu đầy năng lượng
        if (energy >= 99f)
        {
            WinGame();
        }
    }

    public void GainEnergy()
    {
        if (hasEnded) return;

        energy += gainAmount;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
        energySlider.value = energy;
    }

    private void WinGame()
    {
        hasEnded = true;
        FishingManager.Instance.OnMinigameWin();
    }

    private void LoseGame()
    {
        hasEnded = true;
        FishingManager.Instance.OnMinigameLose();
    }
}
