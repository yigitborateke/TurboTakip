using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TTPlayer : MonoBehaviour, IOnEventCallback
{
    private PhotonView _photonView;

    //UI - HUD
    private Button _passButton;
    private Button _playCardButton;
    private Button _drawCardButton;
    private Button _tradeButton;
    private GameObject _waiting4OthersText;
    private Animator _showYourTurnTextAnim;
    private TextMeshProUGUI _messageDisplay; // UI'da mesaj göstermek için Text bileşeni
    private GameObject _cardsUI;
    private Slider _fuelSlider;
    private TextMeshProUGUI _fuelText;
    
    //movement
    private int _steps = 0;
    private RaceTrack _currentRaceTrack;
    private int _trackPosition = 0;
    private bool _isMoving;
    private int _blockedRounds = 0;

    //turn based
    private const int OnStartPlayerCount = 2;
    private int _currentPlayerNo = 0; // 0 is the server master
    private int _myPlayerNo;

    //Cards
    public List<CardCollection.Card> playerCards = new List<CardCollection.Card>();
    private GameObject[] _cardSlots = new GameObject[5]; // CardsPanel altındaki kart slotları
    public List<CardSelect> cardSelects; // Kart seçim kontrolleri
    private CardPool _cardPool; // Kart havuzu referansı
    //private bool _hasDrawnCard = false;
    
    private bool _hasPlayedCardsThisTurn = false;
    private bool _hasShield = false;
    private bool _engineFailure = false;
    private bool _blowout = false;
    private bool _destroyRandCard = false;
    private bool _speedControler = false;
    private bool _wrongWay = false;
    
    //Fuel
    public int currentFuel = 50;
    public int maxFuelAmount = 50;
    
    //SoundFX
    private AudioSource _audioSource;
    
    //EventCodes
    private const byte BombCreated = 2;
    private const byte BombExploded = 3;
    private const byte DestroyRandomCardEvent = 4;
    private const byte SpeedControllerEvent = 5;
    private const byte WrongWayEvent = 6;
    private const byte Card007Event = 7;
    
    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    
    private void Awake()
    {
        _waiting4OthersText = GameObject.Find("WaitingOtherPlayersText");
        _currentRaceTrack = GameObject.Find("RaceTrack").GetComponent<RaceTrack>();
        _photonView = GetComponent<PhotonView>();
        _passButton = GameObject.Find("passButton").GetComponent<Button>();
        if (_passButton != null)
        {
            _passButton.onClick.AddListener(PassTurn);
        }
        _playCardButton = GameObject.Find("playCardButton").GetComponent<Button>();
        if (_playCardButton != null)
        {
            _playCardButton.onClick.AddListener(PlayCard);
        }

        _tradeButton = GameObject.Find("tradeButton").GetComponent<Button>();
        if (_tradeButton != null)
        {
            _tradeButton.onClick.AddListener(TradeCards);
        }

        _drawCardButton = GameObject.Find("Deck").GetComponent<Button>();
        
        _showYourTurnTextAnim = GameObject.Find("yourTurnText").GetComponent<Animator>();
        _messageDisplay = GameObject.Find("Message Display").GetComponent<TextMeshProUGUI>();
        _cardsUI = GameObject.Find("CardsCanvas");
        _fuelSlider = GameObject.Find("Fuel Slider").GetComponent<Slider>();
        _fuelText = GameObject.Find("Fuel Text").GetComponent<TextMeshProUGUI>();
        _cardSlots[0] = GameObject.Find("Card-1");
        _cardSlots[1] = GameObject.Find("Card-2");
        _cardSlots[2] = GameObject.Find("Card-3");
        _cardSlots[3] = GameObject.Find("Card-4");
        _cardSlots[4] = GameObject.Find("Card-5");
        _cardsUI.GetComponent<Canvas>().enabled = false;
        _audioSource = GetComponent<AudioSource>(); 
        ChangeButtonActiveness(false);

        _cardPool = GameObject.Find("CardPool").GetComponent<CardPool>();
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
        _cardsUI.GetComponent<Canvas>().enabled = true;
        UpdateCardDisplay();
        InitializeFuel(currentFuel);
        
        if (PhotonNetwork.IsMasterClient)
        {
            _currentPlayerNo = PhotonNetwork.LocalPlayer.ActorNumber;
            MyTurn();
        }
    }

    private void MyTurn()
    {
        _hasPlayedCardsThisTurn = false;
        Debug.Log("CardCount on my Turn : " + playerCards.Count);
        ChangeButtonActiveness(true);
        if (_showYourTurnTextAnim != null)
        {
            _showYourTurnTextAnim.SetTrigger("showYourTurn");
        }
        
        if (playerCards.Count > 0 && _destroyRandCard)
        {
            _destroyRandCard = false;
            int randomCard = Random.Range(0, playerCards.Count);
            Debug.Log("destroyed card: " + playerCards[randomCard].cardName);
            playerCards.RemoveAt(randomCard);
            UpdateCardDisplay();
            ShowMessageForSeconds("Rakip oyuncu bir random kartını yok etti!", 2);
        }
        
    }

    [PunRPC]
    void RPC_PassTurnToNextPlayer(int currentPlayerNo)
    {
        int pn = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        _currentPlayerNo = currentPlayerNo;
        if (_currentPlayerNo == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            if (_blockedRounds != 0)
            {
                Debug.Log("Blocked Rounds for " + _blockedRounds);
                _blockedRounds--;
                
                PassTurn();
            
            }
            else
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

        while (_steps != 0)
        {
            if (_steps > 0)
            {
                _trackPosition++;
            }
            else
            {
                _trackPosition--;
                if (_trackPosition < 0)
                {
                    _trackPosition = 0;
                    _steps = 0;
                    yield break;
                }
            }
            if (_trackPosition == _currentRaceTrack.childNodeList.Count)
            {
                StartCoroutine(ShowMessageForSeconds("Oyunu kazandın! Tebriklerrrr!", 999));
                _steps = 0;
                ChangeButtonActiveness(false);
                _photonView.RPC("GameFinished", RpcTarget.Others);
            }
            
            Vector3 nextPos = _currentRaceTrack.childNodeList[_trackPosition].position;
            transform.LookAt(nextPos);
            transform.rotation = Quaternion.Inverse(transform.rotation);
            _audioSource.Play();
            while (MoveToNextNode(nextPos))
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(0.25f);
            if (_trackPosition == _currentRaceTrack.childNodeList.Count)
            {
                StartCoroutine(ShowMessageForSeconds("Oyunu kazandın! Tebriklerrrr!", 999));
                _steps = 0;
                ChangeButtonActiveness(false);
                _photonView.RPC("GameFinished", RpcTarget.Others);
            }
            else
            {
                _blockedRounds = _currentRaceTrack.IsObstacleOnPath(_trackPosition);
                if (_blockedRounds > 0)//sirani engelleyecek bir durumu var
                {
                    if (_hasShield)
                    {
                        _hasShield = false;
                        if (_steps > 0)
                        {
                            _steps--;
                        
                        }
                        else if(_steps < 0)
                        {
                            _steps++;
                        }
                    }
                    else
                    {
                        _steps = 0;
                        _blockedRounds--;

                    }
                }
                else
                {
                    if (_steps > 0)
                    {
                        _steps--;
                    }
                    else if(_steps < 0)
                    {
                        _steps++;
                    }
                }
            }
            Hashtable customPlayerProperties = new Hashtable
            {
                { "trackPosition", _trackPosition }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(customPlayerProperties);
            _isMoving = false;
        }
    }
    
    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 25f * Time.deltaTime));
    }

    [PunRPC]
    void GameFinished()
    {
        ChangeButtonActiveness(false);
        StartCoroutine(ShowMessageForSeconds("Oyunu kaybettin!", 999));
    }

    private void ChangeButtonActiveness(bool status)
    {
        _passButton.interactable = status;
        _playCardButton.interactable = status;
        _drawCardButton.interactable = status;
        _tradeButton.interactable = status;
    }

    private void PassTurn()
    {
        if (_photonView.IsMine)
        {
            if (_hasPlayedCardsThisTurn == false && _engineFailure == false && _blockedRounds == 0)
            {
                _steps = 1;
                if (_photonView.IsMine)
                {
                    if (_isMoving == false)
                    {
                        StartCoroutine(Move());
                    }
                }
            }
            else
            {
                _hasPlayedCardsThisTurn = false;
                if (_engineFailure)
                {
                    _engineFailure = false;
                }

            }

            int nextId = PhotonNetwork.LocalPlayer.GetNext().ActorNumber;
            ChangeButtonActiveness(false);
            _photonView.RPC("RPC_PassTurnToNextPlayer", RpcTarget.All, nextId);
        }

    }

    private void PlayCard()
    {
        List<int> selectedCardIndices = new List<int>();
        
        int totalFuelCost = CalculateFuelCostOfSelectedCards();
        if (_blowout)
        {
            totalFuelCost += 2;
        }
        if (currentFuel >= totalFuelCost)
        {
            List<CardCollection.Card> remainingCards = new List<CardCollection.Card>();

            for (int i = 0; i < _cardSlots.Length; i++)
            {
                CardSelect cardSelect = _cardSlots[i].GetComponent<CardSelect>();
                if (cardSelect != null)
                {
                    if (cardSelect.isSelected)
                    {
                        selectedCardIndices.Add(i); // Seçilen kartın indeksini ekle
                        cardSelect.isSelected = false; // Kartın seçimini sıfırla
                        cardSelect.GetComponent<Image>().color = Color.white; // Varsayılan renge geri dön
                    }
                    else if (i < playerCards.Count)
                    {
                        remainingCards.Add(playerCards[i]); // Geriye kalan kartları listeye ekle
                    }
                }
            }
            if (selectedCardIndices.Count > 0)
            {
                _hasPlayedCardsThisTurn = true;
                currentFuel -= totalFuelCost;
                PlayCards(selectedCardIndices); // Seçilen kartları oyna
                UpdateFuelText();
                playerCards = remainingCards; // Oyuncu kartlarını güncelle
                UpdateCardDisplay(); // Kart görünümünü güncelle
            }
        }
        else
        {
            _messageDisplay.text = "Yeterli yakıt yok!";
            ResetCardSelections();  
        }
        
        PassTurn();
    }
    
    private void PlayCards(List<int> cardIndices)
    {
        List<int> playedCardNames = new List<int>();
        Vector3 curPos = transform.position;
        int movementCount = 0;
        bool doubleMovementFlag = false;
        bool efficienyFlag = false;
        bool engineFailure = false;
        foreach (int cardIndex in cardIndices)
        {
            CardCollection.Card playedCard = playerCards[cardIndex];
            if (playedCard.type == CardCollection.Card.CardType.Move)
            {
                movementCount += playedCard.movement;
            }
            else
            {
                if (playedCard.type == CardCollection.Card.CardType.Bomb)
                {
                    GameObject go = PhotonNetwork.Instantiate("Bomb", curPos, Quaternion.identity);
                    go.GetComponent<Bomb>().TrackPosition = _trackPosition;
                    //playedCardNames.Add((int)CardCollection.Card.CardType.Bomb);
                    Debug.Log("Dispatch BombCreated");
                    object[] content = new object[] {_trackPosition, 1, go.GetInstanceID()};
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(BombCreated, content, raiseEventOptions, SendOptions.SendReliable);
                }
                else if(playedCard.type == CardCollection.Card.CardType.SpeedBlocker)
                {
                    GameObject go = PhotonNetwork.Instantiate("Cone", curPos, Quaternion.identity);
                    playedCardNames.Add((int)CardCollection.Card.CardType.SpeedBlocker);
                }
                else if (playedCard.type == CardCollection.Card.CardType.Protection)
                {
                    if (_photonView.IsMine)
                    {
                        _hasShield = true;
                    }
                }
                else if (playedCard.type == CardCollection.Card.CardType.DestroyCard)
                {
                    int nextPlayer = PhotonNetwork.LocalPlayer.GetNext().ActorNumber;
                    
                    Debug.Log("Dispatch DestroyCard");
                    object[] content = new object[] {nextPlayer};
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                    PhotonNetwork.RaiseEvent(DestroyRandomCardEvent, content, raiseEventOptions, SendOptions.SendReliable);
                }
                else if (playedCard.type == CardCollection.Card.CardType.Double)
                {
                    doubleMovementFlag = true;
                }
                else if (playedCard.type == CardCollection.Card.CardType.Productivity)
                {
                    efficienyFlag = true;
                }
                else if (playedCard.type == CardCollection.Card.CardType.Acceleration)
                {
                    Hashtable playerProperties =  PhotonNetwork.LocalPlayer.GetNext().CustomProperties;

                    // Check if the properties contain specific keys and retrieve the values
                    if (playerProperties.ContainsKey("trackPosition"))
                    {
                        int trackPosition = (int)playerProperties["trackPosition"];
                        if (trackPosition > _trackPosition)
                        {
                            movementCount = trackPosition - _trackPosition + 1;
                        }
                    }
                }
                else if (playedCard.type == CardCollection.Card.CardType.Card007)
                {
                    Hashtable playerProperties =  PhotonNetwork.LocalPlayer.GetNext().CustomProperties;

                    // Check if the properties contain specific keys and retrieve the values
                    if (playerProperties.ContainsKey("trackPosition"))
                    {
                        int trackPosition = (int)playerProperties["trackPosition"];
                        if (_trackPosition < trackPosition && trackPosition - _trackPosition <= 10)
                        {
                            int nextPlayer = PhotonNetwork.LocalPlayer.GetNext().ActorNumber;
                            object[] content = new object[] { nextPlayer };
                            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                            PhotonNetwork.RaiseEvent(Card007Event, content, raiseEventOptions, SendOptions.SendReliable);
                        }
                    }
                }
                else if (playedCard.type == CardCollection.Card.CardType.TireBlowout)
                {
                    _blowout = false;
                }
                else if (playedCard.type == CardCollection.Card.CardType.WrongWay)
                {
                    int nextPlayer = PhotonNetwork.LocalPlayer.GetNext().ActorNumber;
                    object[] content = new object[] {nextPlayer};
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                    PhotonNetwork.RaiseEvent(WrongWayEvent, content, raiseEventOptions, SendOptions.SendReliable);
                }
                else if (playedCard.type == CardCollection.Card.CardType.SpeedControl)
                {
                    int nextPlayer = PhotonNetwork.LocalPlayer.GetNext().ActorNumber;
                    object[] content = new object[] {nextPlayer};
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                    PhotonNetwork.RaiseEvent(SpeedControllerEvent, content, raiseEventOptions, SendOptions.SendReliable);
                }
            }
        }

        if (playedCardNames.Count != 0)
        {
            _photonView.RPC("ApplyCardEffects", RpcTarget.Others, playedCardNames.ToArray(), PhotonNetwork.LocalPlayer.ActorNumber);
        }

        if (movementCount > 0)
        {
            if (efficienyFlag)
            {
                currentFuel += movementCount;
            }
            if (doubleMovementFlag)
            {
                movementCount *= 2;
            }

            if (_wrongWay)
            {
                movementCount *= -1;
                _wrongWay = false;

                StartCoroutine(ShowMessageForSeconds("Rakipiniz geçen tur ters yön kartını oynadı!", 2));
            }
            else if (_speedControler)
            {
                Hashtable playerProperties =  PhotonNetwork.LocalPlayer.GetNext().CustomProperties;

                // Check if the properties contain specific keys and retrieve the values
                if (playerProperties.ContainsKey("trackPosition"))
                {
                    int trackPosition = (int)playerProperties["trackPosition"];
                    if (_trackPosition + movementCount >= trackPosition)
                    {
                        movementCount = trackPosition - _trackPosition;
                    }
                }

                _speedControler = false;
            }
            _steps = movementCount;
            StartCoroutine(Move());
        }
    }

    private void TradeCards()
    {
        if (_photonView)
        {
            List<int> selectedCardIndices = new List<int>();
            List<CardCollection.Card> remainingCards = new List<CardCollection.Card>();
            for (int i = 0; i < _cardSlots.Length; i++)
            {
                CardSelect cardSelect = _cardSlots[i].GetComponent<CardSelect>();
                if (cardSelect != null)
                {
                    if (cardSelect.isSelected)
                    {
                        selectedCardIndices.Add(i); // Seçilen kartın indeksini ekle
                        cardSelect.isSelected = false; // Kartın seçimini sıfırla
                        cardSelect.GetComponent<Image>().color = Color.white; // Varsayılan renge geri dön
                    }
                    else if(i < playerCards.Count)
                    {
                        remainingCards.Add(playerCards[i]); // Geriye kalan kartları listeye ekle
                    }
                }
            }

            currentFuel += selectedCardIndices.Count;
            if (currentFuel > 50)
            {
                currentFuel = 50;
            }
            playerCards = remainingCards;
            UpdateFuelText(); // Kart görünümünü güncelle 
            UpdateCardDisplay();
        }
    }
    
    [PunRPC]
    public void ApplyCardEffects(int[] cardTypes, int actorNumber)
    {
        // Kart etkilerini uygula ve UI güncellemesi yap
        // if (PhotonNetwork.LocalPlayer.ActorNumber != actorNumber)
        // {
        //     foreach (int cardType in cardTypes)
        //     {
        //         if (cardType == (int)CardCollection.Card.CardType.Bomb)
        //         {
        //             RaceTrack.Obstacle obs;
        //             obs.Type = (int)CardCollection.Card.CardType.Bomb;
        //             obs.BlockingRounds = 1;
        //             obs.OwnerID = PhotonNetwork.LocalPlayer.ActorNumber;
        //             obs.NodePosition = _trackPosition;
        //             _currentRaceTrack.nodesWithObstacles.Add(obs);
        //         }
        //         else if (cardType == (int)CardCollection.Card.CardType.SpeedBlocker)
        //         {
        //             RaceTrack.Obstacle obs;
        //             obs.Type = (int)CardCollection.Card.CardType.SpeedBlocker;
        //             obs.BlockingRounds = 1;
        //             obs.OwnerID = PhotonNetwork.LocalPlayer.ActorNumber;
        //             obs.NodePosition = _trackPosition;
        //             _currentRaceTrack.nodesWithObstacles.Add(obs);
        //         }
        //     }
        // }
    }

    [PunRPC]
    public void DestroyRandomCard(int nextPlayerId)
    {
        Debug.Log("on Destroyrandomcard id: " + nextPlayerId + " my id : " + PhotonNetwork.LocalPlayer.ActorNumber + " Card Count : " + playerCards.Count) ;

        Debug.Log(" Card Count : " + playerCards.Count);
        if (PhotonNetwork.LocalPlayer.ActorNumber == nextPlayerId)
        { 
            if (playerCards.Count > 0)
            {
                int randomCard = Random.Range(0, playerCards.Count);
                Debug.Log("destroyed card: " + playerCards[randomCard].cardName);
                playerCards.RemoveAt(randomCard);
                UpdateCardDisplay();
                ShowMessageForSeconds("Rakip oyuncu bir random kartını yok etti!", 2);
            }
        }
    }
    
    public void Played007Card()
    {
        _blockedRounds = 2;
        StartCoroutine(ShowMessageForSeconds("Diğer Oyuncu 007 kartını oynadı ve sizi vurdu. " + _blockedRounds + " tur bekleyin.", 2));
    }
    
    private int CalculateFuelCostOfSelectedCards()
    {
        int totalFuelCost = 0;
        for (int i = 0; i < _cardSlots.Length; i++)
        {
            CardSelect cardSelect = _cardSlots[i].GetComponent<CardSelect>();
            if (cardSelect != null && cardSelect.isSelected)
            {
                totalFuelCost += playerCards[i].fuelCost; // Her seçili kartın yakıt maliyetini topla
            }
        }
        return totalFuelCost;
    }

    private void UpdateCardDisplay()
    {
        for (int i = 0; i < _cardSlots.Length; i++)
        {
            if (i < playerCards.Count)
            {
                Sprite cardSprite = playerCards[i].cardSprite;
                _cardSlots[i].GetComponent<Image>().sprite = cardSprite;
                _cardSlots[i].SetActive(cardSprite != null);
            }
            else
            {
                _cardSlots[i].SetActive(false);
            }
        }
        
    }
    private void InitializeFuel(int initialFuelAmount)
    {
        _fuelSlider.maxValue = maxFuelAmount;
        _fuelSlider.value = initialFuelAmount;
        UpdateFuelText();
    }

    void UpdateFuelText()
    {
        if (_fuelText != null)
        {
            _fuelText.text = "Fuel: " + currentFuel.ToString("F0");
            _fuelSlider.value = currentFuel;
        }
        else
        {
            Debug.LogError("fuel text null");
        }
    }
    
    public void DrawOneCard()
    {
        if (playerCards.Count >= 5)
        {
            StartCoroutine(ShowMessageForSeconds("En fazla 5 kartınız olabilir!", 4));
            return;
        }

        CardCollection.Card drawnCard = _cardPool?.DrawRandomCard();

        if (drawnCard != null)
        {
            if (drawnCard.type == CardCollection.Card.CardType.EngineFault)
            {
                StartCoroutine(ShowMessageForSeconds("Motor arıza kartını çektiniz. 1 Tur bekleyin", 4));
                _engineFailure = true;
                PassTurn();
            }
            else
            {
                if (drawnCard.type == CardCollection.Card.CardType.TireBlowout)
                {
                    StartCoroutine(ShowMessageForSeconds("Lastiniğiz patladı. Tamir etmediğiniz her tur ekstra 2 benzin harcayacaksınız!", 4));
                    _blowout = true;
                }

                _drawCardButton.interactable = false;
                playerCards.Add(drawnCard);
                UpdateCardDisplay();
                string message = "Bir kart çekildi: " + drawnCard.cardName;
                StartCoroutine(ShowMessageForSeconds(message, 4));
            }
        }
        else
        {
            StartCoroutine(ShowMessageForSeconds("Kart kalmadı!!", 4));
        }
    }

    private IEnumerator ShowMessageForSeconds(string message, float seconds)
    {
        _messageDisplay.text = message;
        yield return new WaitForSeconds(seconds);
        _messageDisplay.text = ""; // Mesajı temizle
    }
    
    private void ResetCardSelections()
    {
        foreach (GameObject slot in _cardSlots)
        {
            CardSelect cardSelect = slot.GetComponent<CardSelect>();
            if (cardSelect != null)
            {
                cardSelect.isSelected = false;
                cardSelect.GetComponent<Image>().color = Color.white;
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == BombCreated)
        {
            Debug.Log("OnEvent::BombCreated");
            object[] data = (object[])photonEvent.CustomData;
            int trackPosition = (int)data[0];
            int blockingRounds = (int)data[1];
            int obstacleInstanceID = (int)data[2];
            RaceTrack.Obstacle obs;
            obs.BlockingRounds = blockingRounds;
            obs.NodePosition = trackPosition;
            obs.InstanceID = obstacleInstanceID;
            _currentRaceTrack.nodesWithObstacles.Add(obs);
        }
        else if (eventCode == BombExploded)
        {
            Debug.Log("OnEvent::BombExploded");
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Bomb"))
            {
                object[] data = (object[])photonEvent.CustomData;
                int trackPosition = (int)data[0];
                int obstacleInstanceID = (int)data[1];
                if (go.GetInstanceID() == obstacleInstanceID)
                {
                    if (go.GetComponent<Bomb>().TrackPosition == trackPosition)
                    {

                        PhotonNetwork.Destroy(go);
                    }
                }
            }
        }
        else if (eventCode == DestroyRandomCardEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            int nextPlayerID = (int)data[0];

            if (PhotonNetwork.LocalPlayer.ActorNumber == nextPlayerID)
            {
                _destroyRandCard = true;
            }
        }
        else if (eventCode == SpeedControllerEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            int nextPlayerID = (int)data[0];
            if (PhotonNetwork.LocalPlayer.ActorNumber == nextPlayerID)
            {
                _speedControler = true;
            }
        }
        else if (eventCode == WrongWayEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            int nextPlayerID = (int)data[0];
            if (PhotonNetwork.LocalPlayer.ActorNumber == nextPlayerID)
            {
                _wrongWay = true;
            }
        }
        else if (eventCode == Card007Event)
        {
            object[] data = (object[])photonEvent.CustomData;
            int nextPlayerID = (int)data[0];
            if (PhotonNetwork.LocalPlayer.ActorNumber == nextPlayerID)
            {
                Played007Card();
            }
        }
    }

}
