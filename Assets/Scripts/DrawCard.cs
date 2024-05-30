using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DrawCard : MonoBehaviour
{
    public CardPool cardPool; // Kart havuzu referansı
    public PlayerCards pCards; // Oyuncunun sahip olduğu kartlar
    public CardManager cardManager; // Kart yönetimi
    public TextMeshProUGUI messageDisplay; // UI'da mesaj göstermek için Text bileşeni

    void Start()
    {
        if (cardPool == null)
        {
            Debug.LogError("CardPool referansı atanmadı!");
        }

        if (pCards == null)
        {
            Debug.LogError("PlayerCards referansı atanmadı!");
        }

        if (cardManager == null)
        {
            Debug.LogError("CardManager referansı atanmadı!");
        }

        if (messageDisplay == null)
        {
            Debug.LogError("MessageDisplay referansı atanmadı!");
        }
    }

    public void DrawOneCard()
    {
        if (pCards.playerCards.Count >= 5)
        {
            StartCoroutine(ShowMessageForSeconds("En fazla 5 kartınız olabilir!", 4));
            return;
        }

        CardCollection.Card drawnCard = cardPool?.DrawRandomCard();
        if (drawnCard != null)
        {
            pCards.AddCardToPlayer(drawnCard);
            if (cardManager != null)
            {
                cardManager.UpdateCardDisplay();
            }
            string message = "Bir kart çekildi: " + drawnCard.cardName;
            StartCoroutine(ShowMessageForSeconds(message, 4));
        }
        else
        {
            StartCoroutine(ShowMessageForSeconds("Kart kalmadı!!", 4));
        }
    }

    private IEnumerator ShowMessageForSeconds(string message, float seconds)
    {
        messageDisplay.text = message;
        yield return new WaitForSeconds(seconds);
        messageDisplay.text = ""; // Mesajı temizle
    }
}
