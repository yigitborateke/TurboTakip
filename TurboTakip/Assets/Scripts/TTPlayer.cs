using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TTPlayer : MonoBehaviour
{
    private PhotonView _photonView;

    private Button _passButton;
    private Button _playCardButton;
    private GameObject _waiting4OthersText;
    private Animator _showYourTurnTextAnim;
    
    //movement
    private int steps = 0;
    private RaceTrack _currentRaceTrack;
    private int _trackPosition;
    private bool _isMoving;
    
    //turn based
    private const int OnStartPlayerCount = 3;
    private int _currentPlayerNo = 0;// 0 is the server master
    private int _myPlayerNo;
    private bool _myTurn = false;

    private void Awake()
    {
        _waiting4OthersText = GameObject.Find("WaitingOtherPlayersText");

        _currentRaceTrack = GameObject.Find("RaceTrack").GetComponent<RaceTrack>();

        _photonView = GetComponent<PhotonView>();

        _passButton = GameObject.Find("passButton").GetComponent<Button>();
        if (_passButton != null)
        {
            _passButton.onClick.AddListener(Pass);
        }
        _playCardButton = GameObject.Find("playCardButton").GetComponent<Button>();
        if (_playCardButton != null)
        {
            _playCardButton.onClick.AddListener(PlayCard);
        }

        _showYourTurnTextAnim = GameObject.Find("yourTurnText").GetComponent<Animator>();

        ChangeButtonActiveness(false);
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            _myPlayerNo = PhotonNetwork.PlayerList.Length - 1;
        }
        if (PhotonNetwork.PlayerList.Length == OnStartPlayerCount)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        if (_waiting4OthersText != null && _waiting4OthersText.activeSelf)
        {
            _waiting4OthersText.SetActive(false);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            MyTurn();
        }
    }

    private void Update()
    {
        // if (_photonView.IsMine)
        // {
        //     if (_myTurn && _isMoving == false)
        //     {
        //         StartCoroutine(Move());
        //     }
        // }
    }

    private void MyTurn()
    {
        _myTurn = true;
        ChangeButtonActiveness(true);
        if (_showYourTurnTextAnim != null)
        {
            _showYourTurnTextAnim.SetTrigger("showYourTurn");
        }
    }

    [PunRPC]
    void RPC_PassTurnToNextPlayer(int currentPlayerNo)
    {
        _currentPlayerNo = currentPlayerNo;
        if (_photonView.IsMine)
        {
            if (_currentPlayerNo == _myPlayerNo)
            {
                MyTurn();
            }
        }

    }
    
    IEnumerator Move()
    {
        if (_isMoving)
        {
            yield break;
        }

        _isMoving = true;

        while (steps > 0)
        {
            _trackPosition++;
            _trackPosition %= _currentRaceTrack.childNodeList.Count;
            Vector3 nextPos = _currentRaceTrack.childNodeList[_trackPosition].position;
            while (MoveToNextNode(nextPos))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.25f);
            steps--;
        }

        _isMoving = false;
    }
    
    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 2f * Time.deltaTime));
    }

    private void ChangeButtonActiveness(bool status)
    {
        _passButton.interactable = status;
        _playCardButton.interactable = status;
    }
    
    private void Pass()
    {
        if (_photonView.IsMine)
        {
            steps = 1;
        }

        _currentPlayerNo++;
        _currentPlayerNo %= OnStartPlayerCount;
        Debug.Log("nextPlayNO" + _currentPlayerNo);
        ChangeButtonActiveness(false);
        if (_photonView.IsMine)
        {
            if (_isMoving == false)
            {
                StartCoroutine(Move());
            }
        }
        _photonView.RPC("RPC_PassTurnToNextPlayer", RpcTarget.Others, _currentPlayerNo);
    }

    private void PlayCard()
    {
        if (_photonView.IsMine)
        {
            //Card playedCard = GetCurrentCard();
            //if playedCard == moveCard
            //steps = playedCard.moveSteps
            //else
            //play other type of card
           
            steps = Random.Range(0, 7);
            if (_photonView.IsMine)
            {
                if (_isMoving == false)
                {
                    StartCoroutine(Move());
                }
            }
        }
    }

    private void FinishTurn()
    {
        // if (_photonView.IsMine)
        // {
        //     _photonView.RPC("RPC_PassTurnToNextPlayer", RpcTarget.Others);
        // }
    }
}
