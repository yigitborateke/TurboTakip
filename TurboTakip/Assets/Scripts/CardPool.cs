using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;

public class CardPool : MonoBehaviourPun
{
    public CardCollection cardCollection; 
    private List<CardCollection.Card> availableCards;

    void Start()
    {
        InitializeCardPool();
    }

    public void InitializeCardPool()
    {
        if (cardCollection == null)
        {
            return;
        }

        availableCards = new List<CardCollection.Card>(cardCollection.cards);

        if (availableCards == null || availableCards.Count == 0)
        {
            Debug.LogError("Kart havuzu başlatılamadı veya kart kalmadı!");
        }
        else
        {
            Debug.Log("Kart havuzu başlatıldı. Kart sayısı: " + availableCards.Count);
        }
    }

    public CardCollection.Card DrawRandomCard()
    {
        if (availableCards == null || availableCards.Count == 0)
        {
            Debug.LogError("Kart havuzu başlatılmadı veya kart kalmadı!");
            return null;
        }

        int randomIndex = Random.Range(0, availableCards.Count);
        CardCollection.Card drawnCard = availableCards[randomIndex];
        availableCards.RemoveAt(randomIndex);

        //photonView.RPC("RemoveCardFromPool", RpcTarget.Others, drawnCard.cardName);
        return drawnCard;
    }

    [PunRPC]
    public void RemoveCardFromPool(string cardName)
    {
        if (availableCards == null)
        {
            Debug.LogError("availableCards referansı null!");
            return;
        }

        CardCollection.Card cardToRemove = availableCards.Find(card => card.cardName == cardName);
        if (cardToRemove != null)
        {
            availableCards.Remove(cardToRemove);
            Debug.Log("Removed the " + cardName);
        }
    }
}
