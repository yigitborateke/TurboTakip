using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class CardManager : MonoBehaviourPun
{
    public PlayerCards playerCards; // Oyuncunun sahip olduğu kartlar
    public GameObject[] cardSlots; // CardsPanel altındaki kart slotları
    public Button playButton; // Play butonu
    public FuelManager fuelManager; // Yakıt yönetimi
    public List<CardSelect> cardSelects; // Kart seçim kontrolleri
    public TextMeshProUGUI messageDisplay; // UI'da mesaj göstermek için Text bileşeni

    void Start()
    {
        playButton.onClick.AddListener(PlaySelectedCards);
        UpdateCardDisplay();
    }

    public void PlayCards(List<int> cardIndices)
    {
        List<string> playedCardNames = new List<string>();
        foreach (int cardIndex in cardIndices)
        {
            CardCollection.Card playedCard = playerCards.playerCards[cardIndex];
            playedCardNames.Add(playedCard.cardName);
        }
        // Oynanan kartları ve kullanıcı ID'sini herkese bildir
        photonView.RPC("ApplyCardEffects", RpcTarget.Others, playedCardNames.ToArray(), PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    public void ApplyCardEffects(string[] cardNames, int actorNumber)
    {
        // Kart etkilerini uygula ve UI güncellemesi yap
        if (PhotonNetwork.LocalPlayer.ActorNumber != actorNumber)
        {
            foreach (string cardName in cardNames)
            {
                if (cardName == "Bomb") // Örnek: Bomb kartı
                {
                    fuelManager.ConsumeFuel(10); // Örnek: Yakıtı azalt
                }
            }

            // Uyarı mesajını diğer oyunculara göster
            string cardNamesJoined = string.Join(", ", cardNames);
            string message = $"Oyuncu ID {actorNumber}, şu kartları oynadı: {cardNamesJoined}";
            StartCoroutine(ShowMessageForSeconds(message, 4));
        }
    }

    private IEnumerator ShowMessageForSeconds(string message, float seconds)
    {
        messageDisplay.text = message;
        yield return new WaitForSeconds(seconds);
        messageDisplay.text = ""; // Mesajı temizle
    }

    void UpdateFuelDisplay()
    {
        // UI'da yakıt göstergesini güncelle
        GameObject.Find("FuelText").GetComponent<Text>().text = $"Fuel: {fuelManager.GetCurrentFuel()}";
    }

    // Oyuncu kartlarını UI'da güncelle
    public void UpdateCardDisplay()
    {
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < playerCards.playerCards.Count)
            {
                Sprite cardSprite = playerCards.playerCards[i].cardSprite;
                cardSlots[i].GetComponent<Image>().sprite = cardSprite;
                cardSlots[i].SetActive(cardSprite != null);
            }
            else
            {
                cardSlots[i].SetActive(false);
            }
        }
    }

    public void PlaySelectedCards()
    {
        int totalFuelCost = CalculateTotalFuelCost();
        if (fuelManager.ConsumeFuel(totalFuelCost)) // Yeterli yakıt varsa
        {
            List<int> selectedCardIndices = new List<int>();
            List<CardCollection.Card> remainingCards = new List<CardCollection.Card>();

            for (int i = 0; i < cardSlots.Length; i++)
            {
                CardSelect cardSelect = cardSlots[i].GetComponent<CardSelect>();
                if (cardSelect != null)
                {
                    if (cardSelect.isSelected)
                    {
                        selectedCardIndices.Add(i); // Seçilen kartın indeksini ekle
                        cardSelect.isSelected = false; // Kartın seçimini sıfırla
                        cardSelect.GetComponent<Image>().color = Color.white; // Varsayılan renge geri dön
                    }
                    else if (i < playerCards.playerCards.Count)
                    {
                        remainingCards.Add(playerCards.playerCards[i]); // Geriye kalan kartları listeye ekle
                    }
                }
            }

            if (selectedCardIndices.Count > 0)
            {
                PlayCards(selectedCardIndices); // Seçilen kartları oyna
            }

            playerCards.playerCards = remainingCards; // Oyuncu kartlarını güncelle
            UpdateCardDisplay(); // Kart görünümünü güncelle
        }
        else
        {
            messageDisplay.text = "Yeterli yakıt yok!";
            ResetCardSelections();
        }
    }

    private void ResetCardSelections()
    {
        foreach (GameObject slot in cardSlots)
        {
            CardSelect cardSelect = slot.GetComponent<CardSelect>();
            if (cardSelect != null)
            {
                cardSelect.isSelected = false;
                cardSelect.GetComponent<Image>().color = Color.white;
            }
        }
    }

    private int CalculateTotalFuelCost()
    {
        int totalFuelCost = 0;
        for (int i = 0; i < cardSlots.Length; i++)
        {
            CardSelect cardSelect = cardSlots[i].GetComponent<CardSelect>();
            if (cardSelect != null && cardSelect.isSelected)
            {
                totalFuelCost += playerCards.playerCards[i].fuelCost; // Her seçili kartın yakıt maliyetini topla
            }
        }
        return totalFuelCost;
    }

    public void RemoveSelectedCards()
    {
        List<CardCollection.Card> remainingCards = new List<CardCollection.Card>();
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < playerCards.playerCards.Count) 
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
