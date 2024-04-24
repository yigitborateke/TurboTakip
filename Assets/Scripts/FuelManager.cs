using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuelManager : MonoBehaviour
{
    public Slider fuelSlider; // Benzin slider'ı
    public TextMeshProUGUI warningMessage; // Uyarı mesajı için TextMeshPro elementi

    private void Start()
    {
        InitializeFuel(30); // Başlangıç yakıt miktarını ayarla
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
}
