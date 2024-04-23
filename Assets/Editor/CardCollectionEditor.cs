using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static CardCollection;


[CustomEditor(typeof(CardCollection))]
public class CardCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Target nesneyi CardCollection tipinde al
        CardCollection database = (CardCollection)target;

        if (GUILayout.Button("Add New Card"))
        {
            Card newCard = new Card(); // Yeni bir kart nesnesi oluştur
            database.cards.Add(newCard);
            EditorUtility.SetDirty(database); // Değişiklikleri kaydet
        }

        Card cardToRemove = null;

        // Her bir kart için düzenleme UI'ları oluştur
        for (int i = 0; i < database.cards.Count; i++)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            database.cards[i].cardName = EditorGUILayout.TextField("Card Name", database.cards[i].cardName);
            database.cards[i].fuelCost = EditorGUILayout.IntField("Fuel Cost", database.cards[i].fuelCost);
            database.cards[i].movement = EditorGUILayout.IntField("Movement", database.cards[i].movement);
            database.cards[i].description = EditorGUILayout.TextField("Description", database.cards[i].description);
            database.cards[i].type = (Card.CardType)EditorGUILayout.EnumPopup("Card Type", database.cards[i].type);
            database.cards[i].cardSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", database.cards[i].cardSprite, typeof(Sprite), false);

            if (GUILayout.Button("Remove Card"))
            {
                cardToRemove = database.cards[i];
            }

            EditorGUILayout.EndVertical();
        }

        // Kartı koleksiyondan çıkar
        if (cardToRemove != null)
        {
            database.cards.Remove(cardToRemove);
            EditorUtility.SetDirty(database);
        }

        // GUI'de değişiklik yapıldıysa, bunları kaydet
        if (GUI.changed)
        {
            EditorUtility.SetDirty(database);
        }
    }
}

