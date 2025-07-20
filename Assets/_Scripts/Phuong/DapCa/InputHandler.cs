using UnityEngine;
using TMPro;

public class InputHandler : MonoBehaviour
{
    public KeySpawner keySpawner;
    public EnergyManager energyManager;
    public TextMeshProUGUI debugText;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(keySpawner.currentKey.ToLower()))
            {
                energyManager.GainEnergy();
                debugText.text = "Correct!";
            }
            else
            {
                debugText.text = "Wrong!";
            }
        }
    }
}
