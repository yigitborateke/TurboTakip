using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public PlayerCards playerCards; // Oyuncunun sahip olduğu kartlar
    public GameObject[] cardSlots; // CardsPanel altındaki kart slotları
    public Button playButton; // Play butonu
    public FuelManager fuelManager; // Yakıt yönetimi
    public CardSelect cardSelect;
    void Start()
    {
        playButton.onClick.AddListener(PlaySelectedCards);
        UpdateCardDisplay();
    }

    // Oyuncu kartlarını UI'da güncelle
    public void UpdateCardDisplay()
    {
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < playerCards.playerCards.Count)
            {
                Sprite cardSprite = playerCards.playerCards[i].cardSprite;
                if (cardSprite != null)
                {
                    cardSlots[i].GetComponent<Image>().sprite = cardSprite;
                    cardSlots[i].SetActive(true);
                }
                else
                {
                    cardSlots[i].SetActive(false);
                }
            }
            else
            {
                cardSlots[i].SetActive(false);
            }
        }
    }

    // Seçili kartları oyna ve kalan kartları yeniden düzenle
    public void PlaySelectedCards()
    {
        int totalFuelCost = CalculateTotalFuelCost();
        List<CardCollection.Card> remainingCards = new List<CardCollection.Card>();

        for (int i = 0; i < cardSlots.Length; i++)
        {
            CardSelect cardSelect = cardSlots[i].GetComponent<CardSelect>();
            if (cardSelect != null)
            {
                if (cardSelect.isSelected)
                {
                    // Seçili kartı yok et veya kullan
                    cardSelect.isSelected = false; // Kartın seçimini sıfırla
                    cardSelect.GetComponent<Image>().color = Color.white; // Varsayılan renge geri dön
                }
                else if (cardSlots[i].activeSelf)
                {
                    remainingCards.Add(playerCards.playerCards[i]); // Geriye kalan kartları listeye ekle
                }
            }
        }

        playerCards.playerCards = remainingCards; // Oyuncu kartlarını güncelle
        if (fuelManager.ConsumeFuel(totalFuelCost)) // Yeterli yakıt var mı kontrol et
        {
            RemoveSelectedCards(); // Yakıt yeterli ise kartları oynat
            UpdateCardDisplay(); // Kart görünümünü güncelle
        }

    }
    private int CalculateTotalFuelCost()
    {
        int totalFuelCost = 0;

        foreach (var card in playerCards.playerCards)
        {
            if (cardSelect.isSelected)
            {
                totalFuelCost += card.fuelCost;  // card.fuelCost, her kartın maliyetini temsil eder.
            }
        }

        return totalFuelCost;
    }
    public void RemoveSelectedCards()
    {
        // Yeni bir liste oluşturarak sadece seçilmemiş kartları bu listeye alıyoruz.
        List<CardCollection.Card> remainingCards = new List<CardCollection.Card>();
        foreach (var card in playerCards.playerCards)
        {
            if (!cardSelect.isSelected)  // Kart seçili değilse, yeni listeye ekle
            {
                remainingCards.Add(card);
            }
        }
        playerCards.playerCards = remainingCards;  // playerCards listesini, seçilmemiş kartların listesi ile güncelle

        // Kart seçimlerini temizleme ve UI güncelleme gibi işlemleri de burada yapabilirsiniz.
        UpdateCardDisplay();  // UI güncelleme fonksiyonunu çağır
    }

}
