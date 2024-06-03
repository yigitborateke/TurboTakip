using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardFromDeck : MonoBehaviour
{
    private TTPlayer _player;
    private PhotonView _pv;
    private Button _deckButton;
    // Start is called before the first frame update
    void Start()
    {
        _deckButton = GameObject.Find("Deck").GetComponent<Button>();
        if (_deckButton != null)
        {
            _deckButton.onClick.AddListener(DrawOneCard);
        }       
        else
        {
            Debug.LogError("DeckButton component is null on DrawCardFromDecks.cs");
        }

        _player = GameObject.FindWithTag(PhotonNetwork.LocalPlayer.ActorNumber.ToString()).GetComponent<TTPlayer>();
        // foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        // {
        //     Debug.Log("isntance id:"+go.GetInstanceID());
        //     _pv = go.GetComponent<PhotonView>();
        //     if (_pv.IsMine)
        //     {
        //         Debug.Log("Draw card from deck component player: " + PhotonNetwork.LocalPlayer.ActorNumber);
        //         _player = go.GetComponent<TTPlayer>();
        //     }
        // }
    }

    private void DrawOneCard()
    {
        Debug.Log("DrawCardPressed");
        
        if (_player != null)
        {
            _player.DrawOneCard();
        }
        else
        {
            Debug.LogError("Player component is null on DrawCardFromDecks.cs");
        }
    }
}
