using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawCard : MonoBehaviour, IPointerDownHandler
{
    public CardCollection cardCollection;
    public PlayerCards pCards;
    public Panel panelCards;
    public Sprite sprite;
    public string cardName;
    private HashSet<int> selectedCardIndices = new HashSet<int>();

    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectedCardIndices.Count >= cardCollection.cards.Count)
        {
            Debug.Log("Kart kalmadı");
            return;
        }
        if (pCards.playerCards.Count >= 5)
        {
            Debug.Log("Elinizde en fazla 5 kart olabilir.");
            return;
        }
        int randomCardIndex;
        do
        {
            randomCardIndex = Random.Range(0, cardCollection.cards.Count);
        } while (selectedCardIndices.Contains(randomCardIndex));

        selectedCardIndices.Add(randomCardIndex);

        // Oyuncu kartlarını güncelle

        if (pCards.playerCards.Count < 5)
        {
            pCards.AddCardToPlayer(cardCollection.cards[randomCardIndex]);
            
        }
        
        Debug.Log(cardCollection.cards[randomCardIndex].cardName);
    }
}
