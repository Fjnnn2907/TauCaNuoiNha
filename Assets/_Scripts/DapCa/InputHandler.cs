using UnityEngine;
using TMPro;

public class InputHandler : MonoBehaviour
{
    public KeySpawner keySpawner;
    public EnergyManager energyManager;
    public TextMeshProUGUI debugText;
    public Combo comboManager;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(keySpawner.currentKey.ToLower()))
            {
                energyManager.GainEnergy();
                comboManager.AddCombo();
                debugText.text = "Correct!";

                // Gọi zoom camera
                //float zoomFactor = Mathf.Clamp(1f - (comboManager.combo * 0.02f), 0.7f, 0.95f);
                //CamZoomEffect.Instance.ZoomTemporarily(zoomFactor, 0.3f);
            }
            else
            {
                comboManager.ResetCombo();
                debugText.text = "Wrong!";
            }
        }
    }
}
