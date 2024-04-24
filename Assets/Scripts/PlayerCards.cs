using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCards : MonoBehaviour
{
    public List<CardCollection.Card> playerCards = new List<CardCollection.Card>();

    public void AddCardToPlayer(CardCollection.Card card)
    {
        if (playerCards.Count < 5)  // Maksimum 5 kart çekilebilir.
        {
            playerCards.Add(card);
        }
    }
    
}
