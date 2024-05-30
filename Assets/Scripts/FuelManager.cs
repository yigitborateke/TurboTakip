using UnityEngine;
using UnityEngine.UI; // UI bileşenleri için gerekli
using TMPro; // Eğer TextMeshPro kullanıyorsanız gerekli

public class FuelManager : MonoBehaviour
{
    public Slider fuelSlider;
    public TextMeshProUGUI fuelText;
    public int initialFuel = 50;

    void Start()
    {
        InitializeFuel(initialFuel);
    }

    public void InitializeFuel(int fuelAmount)
    {
        fuelSlider.maxValue = fuelAmount;
        fuelSlider.value = fuelAmount;
        UpdateFuelText();
    }

    public bool ConsumeFuel(int amount)
    {
        if (fuelSlider.value >= amount)
        {
            fuelSlider.value -= amount;
            UpdateFuelText();
            return true;
        }
        else
        {
            Debug.Log("Not enough fuel");
            return false;
        }
    }

    public int GetCurrentFuel()
    {
        return (int)fuelSlider.value;
    }

    void UpdateFuelText()
    {
        if (fuelText != null)
        {
            fuelText.text = "Fuel: " + fuelSlider.value.ToString("F0");
        }
    }
}