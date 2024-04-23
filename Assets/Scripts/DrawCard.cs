using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawCard : MonoBehaviour, IPointerDownHandler
{
    public CardCollection cardCollection;

    public void OnPointerDown(PointerEventData eventData)
    {
        for (int i = 0; i < cardCollection.cards.Count; i++)
        {
            int randomCard = Random.Range(1, cardCollection.cards.Count);
            GetComponent<Image>().sprite = cardCollection.cards[randomCard].cardSprite;
            print(cardCollection.cards[randomCard].cardName);
            cardCollection.cards.RemoveAt(randomCard);
        }


    }

}
