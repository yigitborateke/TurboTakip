using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DrawCard : MonoBehaviour, IPointerDownHandler
{
    public CardCollection cardCollection;
    public PlayerCards pCards;
    public GameObject cardPanel;
    private HashSet<int> selectedCardIndices = new HashSet<int>();
    public CardManager cardManager;
    public TextMeshProUGUI messageDisplay;
    public void OnPointerDown(PointerEventData eventData)
    {
        
        if (selectedCardIndices.Count >= cardCollection.cards.Count)
        {
            messageDisplay.text = "Kart kalmadı!!";
            return;
        }
        if (pCards.playerCards.Count >= 5)
        {
            messageDisplay.text = "En fazla 5 kartınız olabilir!";
            return;
        }

        int randomCardIndex = Random.Range(0, cardCollection.cards.Count);
        if (!selectedCardIndices.Contains(randomCardIndex))
        {
            selectedCardIndices.Add(randomCardIndex);
            pCards.AddCardToPlayer(cardCollection.cards[randomCardIndex]);
            if (cardManager != null)
            {
                cardManager.UpdateCardDisplay(); 
            }
        }
    }

}
