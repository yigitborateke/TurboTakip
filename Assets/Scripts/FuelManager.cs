using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuelManager : MonoBehaviour
{
    public Slider fuelSlider; // Benzin slider'ı
    public TextMeshProUGUI warningMessage; // Uyarı mesajı için TextMeshPro elementi
    public TextMeshProUGUI fuelText;
    private void Start()
    {
        InitializeFuel(50); // Başlangıç yakıt miktarını ayarla
    }
    private void Update()
    {
        UpdateFuelText();
    }

    // Yakıtı başlangıç değeri ile başlat
    public void InitializeFuel(int initialFuel)
    {
        fuelSlider.maxValue = initialFuel;
        fuelSlider.value = initialFuel;
        warningMessage.text = ""; // Başlangıçta uyarı mesajını temizle
    }

    // Verilen maliyeti yakıttan düş
    public bool ConsumeFuel(int fuelCost)
    {
        if (fuelSlider.value >= fuelCost)
        {
            fuelSlider.value -= fuelCost; // Yakıtı azalt
            warningMessage.text = ""; // Herhangi bir uyarı mesajını temizle
            return true;
        }
        else
        {
            ShowWarning("Benzininiz yok o yüzden bu kartı kullanamazsınız"); // Yetersiz yakıt uyarısı
            return false;
        }
    }

    // Uyarı mesajı göster
    private void ShowWarning(string message)
    {
        warningMessage.text = message;
    }
    void UpdateFuelText()
    {
        fuelText.text = "Fuel: " + fuelSlider.value.ToString("F0"); // Değeri yuvarlak sayı olarak gösterir
    }
}
