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
    public List<CardSelect> cardSelects;
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
        if (fuelManager.ConsumeFuel(totalFuelCost))  // Yeterli yakıt varsa
        {
            List<CardCollection.Card> remainingCards = new List<CardCollection.Card>();

            for (int i = 0; i < cardSlots.Length; i++)
            {
                CardSelect cardSelect = cardSlots[i].GetComponent<CardSelect>();
                if (cardSelect != null && cardSelect.isSelected)
                {
                    cardSelect.isSelected = false; // Kartın seçimini sıfırla
                    cardSelect.GetComponent<Image>().color = Color.white; // Varsayılan renge geri dön
                }
                else if (i < playerCards.playerCards.Count && cardSlots[i].activeSelf)
                {
                    remainingCards.Add(playerCards.playerCards[i]); // Geriye kalan kartları listeye ekle
                }
            }

            playerCards.playerCards = remainingCards; // Oyuncu kartlarını güncelle
            UpdateCardDisplay(); // Kart görünümünü güncelle
        }
        else
        {
            // Yakıt yetersiz ise tüm kartların seçimini kaldır
            foreach (GameObject slot in cardSlots)
            {
                CardSelect cardSelect = slot.GetComponent<CardSelect>();
                if (cardSelect != null)
                {
                    cardSelect.isSelected = false; // Seçimi kaldır
                    cardSelect.GetComponent<Image>().color = Color.white;
                }
            }
        }

    }
    private int CalculateTotalFuelCost()
    {
        int totalFuelCost = 0;

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < playerCards.playerCards.Count)
            {
                CardSelect cardSelect = cardSlots[i].GetComponent<CardSelect>();
                if (cardSelect != null && cardSelect.isSelected)
                {
                    totalFuelCost += playerCards.playerCards[i].fuelCost; // Her seçili kartın yakıt maliyetini topla
                }
            }
        }

        return totalFuelCost;
    }
    public void RemoveSelectedCards()
    {
        List<CardCollection.Card> remainingCards = new List<CardCollection.Card>();

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < playerCards.playerCards.Count) // playerCards listesinin sınırlarını kontrol et
            {
                CardSelect cardSelect = cardSlots[i].GetComponent<CardSelect>();
                if (cardSelect != null && !cardSelect.isSelected)
                {
                    remainingCards.Add(playerCards.playerCards[i]); // Seçilmemiş kartları yeni listeye ekle
                }
            }
        }

        playerCards.playerCards = remainingCards; // playerCards listesini, seçilmemiş kartların listesi ile güncelle
        UpdateCardDisplay();  // UI güncelleme fonksiyonunu çağır
    }

}
