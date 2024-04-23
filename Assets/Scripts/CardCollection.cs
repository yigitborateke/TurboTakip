using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CardCollection", menuName = "Card Collection")]
public class CardCollection : ScriptableObject
{
    [System.Serializable]
    public class Card
    {
        public string cardName;
        public int fuelCost; 
        public int movement;
        public string description;
        public Sprite cardSprite;
        public enum CardType
        {
            Move,
            Acceleration,
            SpeedBlocker,
            Bomb,
            DestroyCard,
            SpeedControl,
            Protection,
            Productivity,
            WrongWay,
            EngineFault,
            TireBlowout,
            Card007,
            Double
        }
        public CardType type;
    }

    public List<Card> cards = new List<Card>();
}