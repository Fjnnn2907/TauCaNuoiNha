using UnityEngine;

public class Events
{
    public static Events Instance { get; set; } = new();

    public delegate void OnHouseClicked();
    public event OnHouseClicked HouseClicked;

    public void TriggerHouseClicked()
    {
        HouseClicked?.Invoke();
    }
}