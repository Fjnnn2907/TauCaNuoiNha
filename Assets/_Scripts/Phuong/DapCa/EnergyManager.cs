using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergyManager : MonoBehaviour
{
    public float energy = 0f;
    public float maxEnergy = 100f;
    public float decayRate = 5f;
    public float gainAmount = 10f;
    public Slider energySlider;

    public GameObject winPanel;
    public TextMeshProUGUI winText;

    private bool hasWon = false;

    void Start()
    {
        energySlider.maxValue = maxEnergy;
        energySlider.value = energy;
        winPanel.SetActive(false);
    }

    void Update()
    {
        if (hasWon) return;

        energy -= decayRate * Time.deltaTime;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
        energySlider.value = energy;

        if (energy >= 99)
        {
            WinGame();
        }
    }

    public void GainEnergy()
    {
        energy += gainAmount;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
        energySlider.value = energy;
    }

    private void WinGame()
    {
        hasWon = true;
        winPanel.SetActive(true);
        winText.text = "You Win!";
        Time.timeScale = 0f;
    }
}
